using System.Collections.Generic;

namespace OpenJournalist.Service.Model
{
    public class PaginatedVirtualScrollResult<T> : ResultBase
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalResultCount { get; private set; }
        public int TotalPageCount { get; private set; }
        public int PageSize { get; private set; }
        public string NextPageToken { get; private set; }

        public PaginatedVirtualScrollResult(IEnumerable<T> items,
                                            string nextPageToken,
                                            int totalResultCount,
                                            int totalPageCount,
                                            int pageSize,
                                            bool success) : base(success)
        {
            this.Items = items;
            this.NextPageToken = nextPageToken;
            this.TotalResultCount = totalResultCount;
            this.TotalPageCount = totalPageCount;
            this.PageSize = pageSize;
        }
    }
}
