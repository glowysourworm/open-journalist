using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenJournalist.Data.Model.Youtube
{
    public class YoutubeCommentDetailModel
    {
        /// <summary>
        /// Data from last youtube capture
        /// </summary>
        public CommentView View { get; set; }

        /// <summary>
        /// Entity data from last youtube capture
        /// </summary>
        public Youtube_Comment Comment { get; set; }
    }
}
