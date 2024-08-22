using System.Collections.Generic;

using Google.Apis.YouTube.v3.Data;

using WpfCustomUtilities.SimpleCollections.Collection;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeChannelDetailsServiceResponse
    {
        public Channel Channel { get; set; }

        public IList<Playlist> Playlists { get; set; }

        public IList<Video> Videos { get; set; }

        /// <summary>
        /// Playlist items per playlist id
        /// </summary>
        public IDictionary<string, List<PlaylistItem>> PlaylistItemDict { get; set; }

        /// <summary>
        /// Comment Thread entities per video id
        /// </summary>
        public IDictionary<string, CommentThread> CommentThreadDict { get; set; }

        /// <summary>
        /// Comment Thread replies per comment thread id
        /// </summary>
        public IDictionary<string, List<Comment>> CommentRepliesDict { get; set; }

        public YoutubeChannelDetailsServiceResponse()
        {
            this.Playlists = new List<Playlist>();
            this.Videos = new List<Video>();
            this.PlaylistItemDict = new SimpleDictionary<string, List<PlaylistItem>>();
            this.CommentThreadDict = new SimpleDictionary<string, CommentThread>();
            this.CommentRepliesDict = new SimpleDictionary<string, List<Comment>>();
        }
    }
}
