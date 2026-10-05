using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "User";
        public static readonly string[] All = { Admin, Customer };
    }
}
