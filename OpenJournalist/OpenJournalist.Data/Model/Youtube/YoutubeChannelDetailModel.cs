using System.Collections.Generic;

namespace OpenJournalist.Data.Model.Youtube
{
    public class YoutubeChannelDetailModel
    {
        /// <summary>
        /// Contains data about when the channel was last updated
        /// </summary>
        public ChannelView View { get; set; }

        /// <summary>
        /// Data from youtube (taken last update)
        /// </summary>
        public Youtube_Channel Channel { get; set; }

        /// <summary>
        /// Video data for channel. This could still potential be outdated.
        /// </summary>
        public IEnumerable<YoutubeVideoDetailModel> Videos { get; set; }
    }
}
