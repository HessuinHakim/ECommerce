using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token,DateTime ExpiresAt) Generate(string userId, string email, IList<string> roles);
    }
}
