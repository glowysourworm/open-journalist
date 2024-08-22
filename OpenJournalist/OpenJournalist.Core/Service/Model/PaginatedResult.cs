using System.Collections.Generic;

namespace OpenJournalist.Core.Service.Model
{
    public class PaginatedResult<T> : ResultBase
    {
        public IEnumerable<T> Items { get; private set; }
        public int TotalResultCount { get; private set; }
        public int TotalPageCount { get; private set; }
        public int PageSize { get; private set; }
        public int CurrentPageIndex { get; private set; }

        public PaginatedResult(IEnumerable<T> items,
                               int totalResultCount,
                               int totalPageCount,
                               int pageSize,
                               int currentPageIndex,
                               bool success) : base(success)
        {
            this.Items = items;
            this.TotalResultCount = totalResultCount;
            this.TotalPageCount = totalPageCount;
            this.PageSize = pageSize;
            this.CurrentPageIndex = currentPageIndex;
        }
    }
}
