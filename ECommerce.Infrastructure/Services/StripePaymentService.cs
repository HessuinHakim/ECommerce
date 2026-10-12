using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using Order = ECommerce.Domain.Entities.Order;

namespace ECommerce.Infrastructure.Services
{
    public class StripePaymentService:IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IRepository<Order> _orders;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Domain.Entities.Product> _products;

        private readonly IRepository<Promotion> _promotions;


        public StripePaymentService(IConfiguration config, IRepository<Order> orders, IUnitOfWork uow, IRepository<Domain.Entities.Product> products, IRepository<Promotion> promotions, ILogger<StripePaymentService> logger)
        {
            _config = config;
            _orders = orders;
            _uow = uow;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            _products = products;
            _promotions = promotions;
            _logger = logger;
        }

        public async Task<string> CreateCheckoutSessionAsync(int orderId)
        {
            var order = await _orders.GetOneAsync(
                o => o.Id == orderId,
                includes: new Expression<Func<Order, object>>[] { o => o.Items })
                ?? throw new InvalidOperationException("Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order is not payable.");

            if (!string.IsNullOrEmpty(order.StripeSessionId))
            {
                var existing = await new SessionService().GetAsync(order.StripeSessionId);
                if (existing.Status == "open" && !string.IsNullOrEmpty(existing.Url))
                    return existing.Url;
            }

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = _config["Stripe:SuccessUrl"],
                CancelUrl = _config["Stripe:CancelUrl"],
                ClientReferenceId = order.Id.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(31),
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

            if (order.DiscountAmount > 0)
            {
                var coupon = await new CouponService().CreateAsync(new CouponCreateOptions
                {
                    AmountOff = (long)Math.Round(order.DiscountAmount * 100),
                    Currency = "usd",
                    Duration = "once",
                    Name = order.PromotionCode
                });
                options.Discounts = new List<SessionDiscountOptions> { new() { Coupon = coupon.Id } };
            }

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
                _logger.LogWarning(ex, "Stripe webhook signature verification failed");
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
            else if (stripeEvent.Type == "checkout.session.expired" && stripeEvent.Data.Object is Session expired)
            {
                var order = await _orders.GetOneAsync(
                    o => o.StripeSessionId == expired.Id,
                    includes: new Expression<Func<Order, object>>[] { o => o.Items });

                if (order is not null && order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Cancelled;

                    foreach (var item in order.Items)
                    {
                        var product = await _products.GetOneAsync(p => p.Id == item.ProductId);
                        if (product is not null) product.Quantity += item.Count;
                    }

                    if (!string.IsNullOrEmpty(order.PromotionCode))
                    {
                        var promo = await _promotions.GetOneAsync(p => p.Code == order.PromotionCode);
                        if (promo is not null && promo.UsedCount > 0) promo.UsedCount--;
                    }

                    await _uow.CommitAsync();
                }
            }

            return true;
        }
    }
}
