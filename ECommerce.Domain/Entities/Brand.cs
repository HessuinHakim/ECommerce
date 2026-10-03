using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Img { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
