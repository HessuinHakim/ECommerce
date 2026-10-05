using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to,string subject, string body);
    }
}
