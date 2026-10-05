using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Identities.Requests;
using ECommerce.Application.DTOs.Identities.Responses;

namespace ECommerce.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<bool> ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest);
        Task<AuthResponse> ResendEmailConfirmation(ResendEmailConfirmationRequest request);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }

}
