using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeCommentsRequest : PaginatedVirtualScrollRequest
    {
        public string CommentId { get; set; }

        public YoutubeCommentsRequest(string commentId,
                                      string pageToken,
                                      PagingBehavior pageBehavior,
                                      SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(pageToken, pageBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.CommentId = commentId;
        }
    }
}
