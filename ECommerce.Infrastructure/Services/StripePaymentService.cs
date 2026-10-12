using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services
{
    public class StripePaymentService:IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IRepository<Order> _orders;
        private readonly IUnitOfWork _uow;

        public StripePaymentService(IConfiguration config, IRepository<Order> orders, IUnitOfWork uow)
        {
            _config = config;
            _orders = orders;
            _uow = uow;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(int orderId)
        {
            var order = await _orders.GetOneAsync(
                o => o.Id == orderId,
                includes: new Expression<Func<Order, object>>[] { o => o.Items })
                ?? throw new InvalidOperationException("Order not found.");

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = _config["Stripe:SuccessUrl"],
                CancelUrl = _config["Stripe:CancelUrl"],
                ClientReferenceId = order.Id.ToString(),
                Metadata = new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
                LineItems = order.Items.Select(i => new SessionLineItemOptions
                {
                    Quantity = i.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)Math.Round(i.UnitPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = i.ProductName }
                    }
                }).ToList()
            };

            var session = await new SessionService().CreateAsync(options);

            order.StripeSessionId = session.Id;
            await _uow.CommitAsync();

            return session.Url;
        }

        public async Task<bool> HandleWebhookAsync(string payload, string signature)
        {
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    payload,
                    signature,
                    _config["Stripe:WebhookSecret"],
                    throwOnApiVersionMismatch: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Stripe webhook] {ex.GetType().Name}: {ex.Message}");
                return false;
            }

            if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
            {
                var order = await _orders.GetOneAsync(o => o.StripeSessionId == session.Id);

                if (order is not null && order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Paid;
                    await _uow.CommitAsync();
                }
            }
            return true;
        }
    }
}
