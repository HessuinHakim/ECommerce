using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 5)]
        public string Name { get; set; } = string.Empty;
        [MinLength(5)]
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
    }
}
