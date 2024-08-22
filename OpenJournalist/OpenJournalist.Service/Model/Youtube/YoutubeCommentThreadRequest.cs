using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    /// <summary>
    /// Youtube request that selects either a comment thread for specific video(s), or ALL threads related to
    /// a single channel.
    /// </summary>
    public class YoutubeCommentThreadRequest : PaginatedVirtualScrollRequest
    {
        public string VideoId { get; set; }

        public YoutubeCommentThreadRequest(string videoId,
                                           string pageToken,
                                           PagingBehavior pageBehavior,
                                           SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(pageToken, pageBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.VideoId = videoId;
        }

        public YoutubeCommentThreadRequest(YoutubeCommentThreadRequest lastPageRequest, string pageToken)

            : base(pageToken, lastPageRequest.PagingBehavior, lastPageRequest.MessageHandler, YoutubeConstants.MaxResults)
        {
            this.VideoId = lastPageRequest.VideoId;
        }
    }
}
