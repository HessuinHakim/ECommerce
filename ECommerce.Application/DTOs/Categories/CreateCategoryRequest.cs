using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Categories
{
    public record CreateCategoryRequest
    {

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;


    }
}
