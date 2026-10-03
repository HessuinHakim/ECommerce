namespace ECommerce.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product? Product { get; set; } 
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
