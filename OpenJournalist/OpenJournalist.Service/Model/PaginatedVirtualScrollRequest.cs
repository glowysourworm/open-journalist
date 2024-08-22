using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model
{
    public class PaginatedVirtualScrollRequest : RequestBase<MessagingCallbackEventArgsBase>
    {
        /// <summary>
        /// Token used to request the next page of information
        /// </summary>
        public string PageToken { get; private set; }

        /// <summary>
        /// Max page size - see platform constants
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Behavior of paging for request / response of service
        /// </summary>
        public PagingBehavior PagingBehavior { get; private set; }

        public PaginatedVirtualScrollRequest(string pageToken,
                                             PagingBehavior pagingBehavior,
                                             SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                             int pageSize) : base(messageHandler)
        {
            this.PageToken = pageToken;
            this.PageSize = pageSize;
            this.PagingBehavior = pagingBehavior;
        }
    }
}
