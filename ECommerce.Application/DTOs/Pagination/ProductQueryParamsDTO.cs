using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Pagination
{
    public record ProductQueryParamsDTO
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private int _page = 1;

        public int Page { get => _page; set => _page = value < 1 ? 1 : value; }
        public int PageSize { get => _pageSize; set => _pageSize = value < 1 ? 10 : Math.Min(value, MaxPageSize); }

        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
    }
}
