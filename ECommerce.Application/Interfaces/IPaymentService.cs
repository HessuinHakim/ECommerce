using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces
{
    public interface IPaymentService
    {
    
            Task<string> CreateCheckoutSessionAsync(int orderId);          
            Task<bool> HandleWebhookAsync(string payload, string signature);
        
    }
}
