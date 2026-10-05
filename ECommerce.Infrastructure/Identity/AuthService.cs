using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.Constants;
using ECommerce.Application.DTOs.Identities.Requests;
using ECommerce.Application.DTOs.Identities.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Identity
{
    public class AuthService :IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRepository<ApplicationUserOTP> _otpRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<ApplicationUser> _hasher;

        private const int MaxOtpAttempts = 5;

        public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IJwtTokenGenerator jwtTokenGenerator, IRepository<ApplicationUserOTP> otpRepository, IUnitOfWork unitOfWork, IPasswordHasher<ApplicationUser> hasher)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
            _jwtTokenGenerator = jwtTokenGenerator;
            _otpRepository = otpRepository;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) is not null)
                    return new AuthResponse { Errors = new[] { "Email is already registered" } };

            var user = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                FullName = registerRequest.FullName

            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if(!result.Succeeded)
            {
                return new AuthResponse
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            
            await _userManager.AddToRoleAsync(user, Roles.Customer);
            await SendConfirmationEmailAsync(user);

            return await BuildResponseAsync(user);
      
        }


        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if(user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return new AuthResponse { Errors = new[] { "Invalid email or password" } };
            }
            if (user.EmailConfirmed == false)
            {
                return new AuthResponse { Errors = new[] { "Email is not confirmed" } };
            }

            return await BuildResponseAsync(user);
        }
        
     
        public async Task<AuthResponse> ResendEmailConfirmation(ResendEmailConfirmationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var genericResponse = new AuthResponse
            {
                Succeeded = true,
                Errors = Enumerable.Empty<string>()
            };

            if (user is null) return new AuthResponse { Errors = new[] { "User not found" } };

            if (user.EmailConfirmed) return new AuthResponse { Errors = new[] { "Email is already confirmed" } };
            
           
            await SendConfirmationEmailAsync(user);
            return genericResponse;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if (user is null) return;

            var old = await _otpRepository.GetAsync(o => o.ApplicationUserId == user.Id
                    && o.Purpose == OtpPurpose.ResetPassword
                    && !o.IsUsed
                );

            foreach (var oldotp in old)
                oldotp.IsUsed = true;

            var otp = GenerateOtp();
            await _otpRepository.CreateAsync(new ApplicationUserOTP
            {
                ApplicationUserId = user.Id,
                Purpose = OtpPurpose.ResetPassword,
                OtpHash = _hasher.HashPassword(user, otp)
            });

            await _unitOfWork.CommitAsync();
            await _emailSender.SendAsync(user.Email!, "Reset your password",
            $"<p>Enter The Code:</p><h2>{otp}</h2><p>Valid for 10 minutes. If you didn't request this, please ignore this email.</p>");
        }





        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var invalid = new AuthResponse { Errors = new[] { "Invalid or expired code." } };

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return invalid;

            var record = await _otpRepository.GetOneAsync(
                o => o.ApplicationUserId == user.Id
                     && o.Purpose == OtpPurpose.ResetPassword
                     && !o.IsUsed
                     && o.ValidTo > DateTime.UtcNow
                     && o.AttemptsCount < MaxOtpAttempts,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt));

            if (record is null) return invalid;

            var verify = _hasher.VerifyHashedPassword(user, record.OtpHash, request.Otp);
            if (verify == PasswordVerificationResult.Failed)
            {
                record.AttemptsCount++;                 
                await _unitOfWork.CommitAsync();
                return invalid;
            }



            record.IsUsed = true;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            if (!result.Succeeded)
                return new AuthResponse { Errors = result.Errors.Select(e => e.Description) };

            await _unitOfWork.CommitAsync();
            return new AuthResponse { Succeeded = true };
        }





        public async Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null) return false;

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ConfirmEmailAsync(user, decoded);
            return result.Succeeded;
        }

        
        private async Task<AuthResponse> BuildResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (token,expiresAt) = _jwtTokenGenerator.Generate(user.Email!, user.Id, roles);
            return new AuthResponse
            {
                Succeeded = true,
                Token = token,
                ExpiresAt = expiresAt,
                Email = user.Email,
                Roles = roles
            };
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = $"{_configuration["App:BaseUrl"]}/api/auth/confirm-email?userId={user.Id}&token={encoded}";

            await _emailSender.SendAsync(user.Email!, "Confirm your email",
                $"<p>Press Here To Confirm Your Email</p><a href='{link}'>Confirm Email</a>");
        }

        private static string GenerateOtp()
            => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    }
}
