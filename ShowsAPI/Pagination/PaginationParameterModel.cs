namespace ShowsAPI.Pagination
{
    public class PaginationParameterModel
    {
        const int MaxPageSize = 10;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public PaginationMetadata GetPaginationMetadata(int showsCount)
        {
            var currentPage = PageNumber;
            var pageSize = PageSize;
            var totalCount = showsCount;
            var totalPages = (showsCount + pageSize - 1) / pageSize;
            var hasPreviousPage = currentPage > 1 ? "Yes" : "No";
            var hasNextPage = currentPage < totalPages ? "Yes" : "No";

            return new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage
            };
        }
    }
}
