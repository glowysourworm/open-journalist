using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubePlaylistItemRequest : PaginatedVirtualScrollRequest
    {
        public string PlaylistId { get; set; }

        public YoutubePlaylistItemRequest(string playlistId,
                                          string pageToken,
                                          PagingBehavior pageBehavior,
                                          SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(pageToken, pageBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.PlaylistId = playlistId;
        }

        public YoutubePlaylistItemRequest(YoutubePlaylistItemRequest previousPageRequest, string nextPageToken)

            : base(nextPageToken, previousPageRequest.PagingBehavior, previousPageRequest.MessageHandler, previousPageRequest.PageSize)
        {
            this.PlaylistId = previousPageRequest.PlaylistId;
        }
    }
}
