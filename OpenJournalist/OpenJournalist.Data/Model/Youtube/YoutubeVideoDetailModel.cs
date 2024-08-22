using System.Collections.Generic;

namespace OpenJournalist.Data.Model.Youtube
{
    public class YoutubeVideoDetailModel
    {
        /// <summary>
        /// Data from when video was last updated
        /// </summary>
        public VideoView View { get; set; }

        /// <summary>
        /// Detailed data from youtube service, taken last update
        /// </summary>
        public Youtube_Video Video { get; set; }

        /// <summary>
        /// Comment thread views, could potentially be outdated
        /// </summary>
        public IEnumerable<YoutubeCommentThreadDetailModel> CommentThreads { get; set; }
    }
}
