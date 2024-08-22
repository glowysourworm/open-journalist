using System;
using System.Collections.Generic;

using OpenJournalist.Data.Model.Youtube;
using OpenJournalist.Service.Model;
using OpenJournalist.Service.Model.Local;
using OpenJournalist.Service.Model.Youtube;

namespace OpenJournalist.Data.Interface
{
    /// <summary>
    /// Primary interface from 3rd party services to the local database
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Local - Search Results, Channels, etc...
        //
        PaginatedResult<ChannelSearchResultView> GetYoutubeSearchResults(SearchPaginatedRequest request);
        PaginatedResult<ChannelView> GetChannels(SearchPaginatedRequest request);

        YoutubeCommentDetailModel GetComment(string commentId);
        YoutubeCommentThreadDetailModel GetCommentThread(string commentThreadId);
        YoutubeVideoDetailModel GetLocalVideo(string videoId);
        YoutubeChannelDetailModel GetLocalChannel(string channelId);

        // Youtube - Service Calls
        //
        PaginatedVirtualScrollResult<Youtube_SearchResult> Execute_Youtube_BasicSearch(YoutubeBasicSearchRequest request);
        SingleResult<Youtube_Channel> Execute_Youtube_Channel_List(YoutubeChannelDetailsRequest request);
        PaginatedVirtualScrollResult<Youtube_Video> Execute_Youtube_Video_List(YoutubeVideoDetailsRequest request);
        PaginatedVirtualScrollResult<Youtube_Playlist> Execute_Youtube_Playlist_List(YoutubePlaylistRequest request);
        PaginatedVirtualScrollResult<Youtube_PlaylistItem> Execute_Youtube_PlaylistItem_List(YoutubePlaylistItemRequest request);
        PaginatedVirtualScrollResult<Youtube_CommentThread> Execute_Youtube_CommentThread_List(YoutubeCommentThreadRequest request);

        // Youtube - Composite Calls
        // DoEverything
    }
}
