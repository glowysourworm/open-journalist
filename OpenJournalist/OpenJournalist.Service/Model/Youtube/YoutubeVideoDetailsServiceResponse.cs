using System.Collections.Generic;

using Google.Apis.YouTube.v3.Data;

using WpfCustomUtilities.SimpleCollections.Collection;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeVideoDetailsServiceResponse
    {
        public Video Video { get; set; }

        /// <summary>
        /// Comment Thread entities per video id
        /// </summary>
        public IDictionary<string, CommentThread> CommentThreadDict { get; set; }

        /// <summary>
        /// Comment Thread replies per comment thread id
        /// </summary>
        public IDictionary<string, List<Comment>> CommentRepliesDict { get; set; }

        public YoutubeVideoDetailsServiceResponse()
        {
            this.Video = null;
            this.CommentThreadDict = new SimpleDictionary<string, CommentThread>();
            this.CommentRepliesDict = new SimpleDictionary<string, List<Comment>>();
        }
    }
}
