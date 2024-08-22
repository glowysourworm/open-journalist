using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeBasicSearchRequest : PaginatedVirtualScrollRequest
    {
        /// <summary>
        /// Filter parameter - wild card (search string)
        /// </summary>
        public string Search { get; protected set; }

        public YoutubeBasicSearchRequest(string search,
                                         SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)
            : base(null, PagingBehavior.SinglePageResult, messageHandler, YoutubeConstants.MaxResults)
        {
            this.Search = search;
        }

        public YoutubeBasicSearchRequest(string search,
                                         string pageToken,
                                         PagingBehavior pagingBehavior,
                                         SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)
            : base(pageToken, pagingBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.Search = search;
        }

        public YoutubeBasicSearchRequest(YoutubeBasicSearchRequest previousPageRequest, string nextPageToken)
            : base(nextPageToken, previousPageRequest.PagingBehavior, previousPageRequest.MessageHandler, YoutubeConstants.MaxResults)
        {
            this.Search = previousPageRequest.Search;
        }
    }
}
