using System;

using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;

using OpenJournalist.Service.Model;
using OpenJournalist.Service.Model.Youtube;

namespace OpenJournalist.Service.Interface
{
    public interface IYoutubeService : IDisposable
    {
        /// <summary>
        /// Primary service connection to the API
        /// </summary>
        IClientService ServiceBase { get; }

        PaginatedVirtualScrollResult<SearchResult> Search(YoutubeBasicSearchRequest request);
        PaginatedVirtualScrollResult<SearchResult> SearchUser(YoutubeUserChannelsSearchRequest request);
        PaginatedVirtualScrollResult<Playlist> GetPlaylists(YoutubePlaylistRequest request);
        PaginatedVirtualScrollResult<PlaylistItem> GetPlaylistItems(YoutubePlaylistItemRequest request);
        PaginatedVirtualScrollResult<CommentThread> GetCommentThreads(YoutubeCommentThreadRequest request);
        PaginatedVirtualScrollResult<Video> GetVideoDetails(YoutubeVideoDetailsRequest request);
        SingleResult<Channel> GetChannelDetails(YoutubeChannelDetailsRequest request);
        //YoutubeChannelDetailsServiceResponse GetAllChannelInformation(YoutubeChannelDetailsRequest serviceRequest);
    }
}
