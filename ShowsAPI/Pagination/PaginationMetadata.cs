using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowsAPI.Pagination
{
    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string HasPreviousPage { get; set; }
        public string HasNextPage { get; set; }
    }
}
