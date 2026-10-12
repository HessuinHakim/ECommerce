using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool IsActive { get; set; } = true;    
        public int MaxUsage { get; set; } = 100;
        public int UsedCount { get; set; } = 0;
    }
}
