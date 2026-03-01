


namespace StrikeDefender.Application.Common.Pagination
{
    public record RequestFilters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchValue { get; init; }
        public string? SortColumn { get; init; }
        public string? SortDirection { get; init; } = "ASC";
    }
    public record AddtionalRequestFilters
    {
        // Filters from UI
        public string? Location { get; set; }
        public bool? OpenNow { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public List<string>? Services { get; set; }
    }
}
