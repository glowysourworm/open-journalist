
using OpenJournalist.Core.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Core.Service.Model
{
    public class PaginatedRequest : RequestBase<MessagingCallbackEventArgsBase>
    {
        /// <summary>
        /// Index of current page
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// Max page size - see platform constants
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Behavior of paging for request / response of service
        /// </summary>
        public PagingBehavior PagingBehavior { get; private set; }

        public PaginatedRequest(int currentPageIndex,
                                PagingBehavior pagingBehavior,
                                SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                int pageSize)
            : base(messageHandler)
        {
            this.CurrentPageIndex = currentPageIndex;
            this.PageSize = pageSize;
            this.PagingBehavior = pagingBehavior;
        }
    }
}
