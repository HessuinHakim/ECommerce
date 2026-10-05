using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Identities.Requests
{
    public record RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required"),MaxLength(100, ErrorMessage = "Full name should not exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required"),EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required"),MinLength(8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}
