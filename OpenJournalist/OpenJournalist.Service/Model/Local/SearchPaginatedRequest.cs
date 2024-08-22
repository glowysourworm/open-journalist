
using OpenJournalist.Service.Model.Message;
using OpenJournalist.Service.Model.Youtube;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Local
{
    public class SearchPaginatedRequest : PaginatedRequest
    {
        /// <summary>
        /// Search string for local requests. Behavior of search filtering is entity specific; and depends
        /// on the UnitOfWork implementation for retrieving entities.
        /// </summary>
        public string SearchTerm { get; private set; }

        public SearchPaginatedRequest(SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler, string searchTerm = "")
            : base(0, PagingBehavior.RunSynchronouslyToPageLimit, messageHandler, YoutubeConstants.MaxResults)
        {
            this.SearchTerm = searchTerm;
        }

        public SearchPaginatedRequest(string searchTerm,
                                     int currentPageIndex,
                                     PagingBehavior pagingBehavior,
                                     SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                     int pageSize)
            : base(currentPageIndex, pagingBehavior, messageHandler, pageSize)
        {
            this.SearchTerm = searchTerm;
        }
    }
}
