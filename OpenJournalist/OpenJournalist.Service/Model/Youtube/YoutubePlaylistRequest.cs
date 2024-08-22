using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubePlaylistRequest : PaginatedVirtualScrollRequest
    {
        /// <summary>
        /// YOUTUBE ISSUE!!!  THE PLAYLIST ID REFERS TO THE ContentDetails.RelatedPlaylists.Uploads FIELD, OR
        ///                   THE ACTUAL PLAYLIST ID, (WHICH MEANS IT WAS ALREADY RETRIEVED). THIS WAS HARD TO
        ///                   ACCESS.....
        ///                   
        ///                   The "Uploads" playlist is assumed to be the primary channel playlist, that contains
        ///                   all videos for the channel. Awfully hard to get to.....
        ///                   
        ///                   1) Channel -> Videos -> Comment Threads (nope!)
        ///                   2) Channel -> Playlists -> Comment Threads & Videos (nope!)
        ///                   3) Channel -> Channel.ContentDetails.RelatedPlaylists.Uploads (string),  (yes.)
        ///                   
        ///                   Then, get playlists, then comment threads, and videos (if desired)
        /// </summary>
        public string PlaylistId { get; set; }

        public YoutubePlaylistRequest(string playlistId,
                                      string pageToken,
                                      PagingBehavior pageBehavior,
                                      SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(pageToken, pageBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.PlaylistId = playlistId;
        }

        public YoutubePlaylistRequest(YoutubePlaylistRequest previousPageRequest, string nextPageToken)

            : base(nextPageToken, previousPageRequest.PagingBehavior, previousPageRequest.MessageHandler, previousPageRequest.PageSize)
        {
            this.PlaylistId = previousPageRequest.PlaylistId;
        }
    }
}
