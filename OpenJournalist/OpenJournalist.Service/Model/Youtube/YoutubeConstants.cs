using System;

namespace OpenJournalist.Service.Model.Youtube
{
    public static class YoutubeConstants
    {
        /// <summary>
        /// Max results for most youtube services
        /// </summary>
        public static int MaxResults = 50;

        /// <summary>
        /// Video part names:  contentDetails,fileDetails*,id,liveStreamingDetails,localizations,player,processingDetails*,recordingDetails,snippet,
        ///                    statistics,status,suggestions*,topicDetails
        ///                    
        ///                    *NOTE: These fields require enhanced permissions
        /// </summary>
        public static string VideoParts = @"contentDetails,id,liveStreamingDetails,localizations,player,recordingDetails,snippet,statistics,status,topicDetails";

        /// <summary>
        /// Channel part names:  auditDetails*, brandingSettings, contentDetails, contentOwnerDetails,
        ///                      id, localizations, snippet, statistics,
        ///                      status, topicDetails
        ///                      
        ///                      *NOTE: Some of these fields require enhanced scope permissions
        /// </summary>
        public static string ChannelParts = @"brandingSettings,contentDetails,contentOwnerDetails,id,localizations,snippet,statistics,status,topicDetails";

        /// <summary>
        /// Playlist part names
        /// </summary>
        public static string PlaylistParts = @"contentDetails,id,localizations,player,snippet,status";

        /// <summary>
        /// Playlist item part names
        /// </summary>
        public static string PlaylistItemParts = @"contentDetails,id,snippet,status";

        /// <summary>
        /// Search part names:  etag, id, kind, snippet
        /// </summary>
        public static string SearchParts = "id, snippet";

        /// <summary>
        /// Comment Thread part names: id,replies,snippet
        /// </summary>
        public static string CommentThreadParts = "id,replies,snippet";

        /// <summary>
        /// Comment part names: id,snippet
        /// </summary>
        public static string CommentParts = "id,snippet";

        public static string SearchTypeChannel = "channel";
        public static string SearchTypePlaylist = "playlist";
        public static string SearchTypeVideo = "video";
        public static string SearchCommentThread = "commentThreads";

        public static string ResponseKindChannel = "youtube#channel";
        public static string ResponseKindPlaylist = "youtube#playlist";
        public static string ResponseKindVideo = "youtube#video";
        public static string ResponseKindCommentThread = "youtube#commentThread";
    }
}
