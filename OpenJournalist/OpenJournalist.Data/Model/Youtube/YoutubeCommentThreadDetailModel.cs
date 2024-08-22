using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenJournalist.Data.Model.Youtube
{
    public class YoutubeCommentThreadDetailModel
    {
        /// <summary>
        /// Data about last youtube capture
        /// </summary>
        public CommentThreadView View { get; set; }

        /// <summary>
        /// Data from last youtube capture
        /// </summary>
        public Youtube_CommentThread CommentThread { get; set; }

        /// <summary>
        /// Comments for expanded detail
        /// </summary>
        public IEnumerable<YoutubeCommentDetailModel> Comments { get; set; }
    }
}
