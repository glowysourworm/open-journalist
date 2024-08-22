using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Google.Apis.YouTube.v3.Data;

using OpenJournalist.Data.Interface;
using OpenJournalist.Data.Model.Youtube;
using OpenJournalist.Service;
using OpenJournalist.Service.Interface;
using OpenJournalist.Service.Model;
using OpenJournalist.Service.Model.Local;
using OpenJournalist.Service.Model.Message;
using OpenJournalist.Service.Model.Youtube;

using WpfCustomUtilities.Extensions;
using WpfCustomUtilities.Extensions.Collection;

using YoutubeData = Google.Apis.YouTube.v3.Data;

namespace OpenJournalist.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly OpenJournalistEntities _dbContext;
        readonly IYoutubeService _youtubeService;

        bool _disposed;

        public UnitOfWork(string apiKey, string clientId, string clientSecret, string connectionString)
        {
            // EF Database Connection
            _dbContext = new OpenJournalistEntities(connectionString);

            _youtubeService = new YoutubeService(apiKey, clientId, clientSecret);

            _disposed = false;
        }

        #region Local DB Methods
        public PaginatedResult<ChannelSearchResultView> GetYoutubeSearchResults(SearchPaginatedRequest request)
        {
            try
            {
                var entityCount = _dbContext.ChannelSearchResultViews.Count();

                return ProcessPaginatedRequest(request, entityCount, (currentIndex, takeSize) =>
                {
                    // With search term
                    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                    {
                        return _dbContext.ChannelSearchResultViews
                                         .Where(entity => entity.Title.Contains(request.SearchTerm) ||
                                                          entity.Description.Contains(request.SearchTerm))
                                         .Skip(currentIndex + 1)
                                         .Take(takeSize);
                    }
                    else
                    {
                        return _dbContext.ChannelSearchResultViews
                                         .Skip(currentIndex + 1)
                                         .Take(takeSize);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PaginatedResult<ChannelView> GetChannels(SearchPaginatedRequest request)
        {
            try
            {
                var entityCount = _dbContext.ChannelViews.Count();

                return ProcessPaginatedRequest(request, entityCount, (currentIndex, takeSize) =>
                {
                    return _dbContext.ChannelViews
                                     .Skip(currentIndex + 1)
                                     .Take(takeSize);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YoutubeCommentDetailModel GetComment(string commentId)
        {
            try
            {
                var view = _dbContext.CommentViews.First(x => x.YoutubeId == commentId);

                var comment = _dbContext.Youtube_Comment
                                              .Where(x => x.Id == commentId)
                                              .First();

                return new YoutubeCommentDetailModel()
                {
                    Comment = comment,
                    View = view
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public YoutubeCommentThreadDetailModel GetCommentThread(string commentThreadId)
        {
            try
            {
                var view = _dbContext.CommentThreadViews.First(x => x.YoutubeId == commentThreadId);

                var commentThread = _dbContext.Youtube_CommentThread
                                              .Where(x => x.Id == commentThreadId)
                                              .First();

                var comments = _dbContext.CommentViews
                                         .Where(x => x.CommentThreadId == view.Id)
                                         .Actualize();

                return new YoutubeCommentThreadDetailModel()
                {
                    CommentThread = commentThread,
                    View = view,
                    Comments = comments.Select(x => GetComment(x.YoutubeId)).Actualize()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public YoutubeVideoDetailModel GetLocalVideo(string videoId)
        {
            try
            {
                var view = _dbContext.VideoViews.First(x => x.YoutubeId == videoId);

                var video = _dbContext.Youtube_Video
                                      .Where(x => x.Id == videoId)
                                      .First();

                var commentThreads = _dbContext.CommentThreadViews
                                               .Where(x => x.VideoId == view.Id)
                                               .Actualize();

                return new YoutubeVideoDetailModel()
                {
                    View = view,
                    Video = video,
                    CommentThreads = commentThreads.Select(x => GetCommentThread(x.YoutubeId)).Actualize()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public YoutubeChannelDetailModel GetLocalChannel(string channelId)
        {
            try
            {
                var view = _dbContext.ChannelViews.First(x => x.YoutubeId == channelId);

                var channel = _dbContext.Youtube_Channel
                                        .Where(x => x.Id == channelId)
                                        .First();

                var videoViews = _dbContext.VideoViews
                                           .Where(x => x.ChannelId == view.Id)
                                           .Actualize();

                return new YoutubeChannelDetailModel()
                {
                    Channel = channel,
                    View = view,
                    Videos = videoViews.Select(x => GetLocalVideo(x.YoutubeId)).Actualize()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IYoutubeService Methods
        public PaginatedVirtualScrollResult<Youtube_SearchResult> Execute_Youtube_BasicSearch(YoutubeBasicSearchRequest request)
        {
            try
            {
                var response = ProcessVirtualScrollPaginatedRequest(request, pageToken =>
                {
                    return _youtubeService.Search(new YoutubeBasicSearchRequest(request, pageToken));
                });

                // Create separate list to return, for DB context disposal reasons
                var entityList = new List<Youtube_SearchResult>();

                // Add Entities to database
                foreach (var result in response.Items)
                {
                    // Create Entity from Response
                    var entity = _dbContext.Youtube_SearchResult.CreateObject();

                    Map_Youtube_SearchResult(result, entity);

                    // Add Entity to DB context
                    _dbContext.Youtube_SearchResult.AddObject(entity);

                    // Use other list for return value
                    entityList.Add(entity);
                }

                // Commit changes
                _dbContext.SaveChanges();

                return new PaginatedVirtualScrollResult<Youtube_SearchResult>(entityList,
                                                                              response.NextPageToken,
                                                                              response.TotalResultCount,
                                                                              response.TotalPageCount,
                                                                              response.PageSize,
                                                                              true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SingleResult<Youtube_Channel> Execute_Youtube_Channel_List(YoutubeChannelDetailsRequest request)
        {
            try
            {
                var response = _youtubeService.GetChannelDetails(request);

                if (response.Item == null)
                {
                    request.MessageHandler(new ServiceMessageEventArgs());

                    return new SingleResult<Youtube_Channel>(null, false);
                }

                var entity = response != null ? AddUpdateChannel(response.Item) : null;

                return new SingleResult<Youtube_Channel>(entity, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        // TODO
        public Youtube_Channel SearchUpdateAllChannelVideos(YoutubeChannelDetailsRequest request)
        {
            try
            {
                // Query Youtube
                var response = _youtubeService.GetAllChannelInformation(request);

                if (response.Channel == null)
                    return null;

                // Channel
                var channel = AddUpdateChannel(response.Channel);

                // Videos
                foreach (var video in response.Videos)
                    AddUpdateVideo(video);

                // Playlists
                foreach (var playlist in response.Playlists)
                    AddUpdatePlaylist(playlist);

                // Playlist Items
                foreach (var playlist in response.PlaylistItemDict)
                {
                    foreach (var playlistItem in playlist.Value)
                        AddUpdatePlaylistItem(playlistItem);
                }

                // Comment Threads + Replies
                foreach (var commentThread in response.CommentThreadDict.Values)
                    AddUpdateCommentThread(commentThread,
                                           commentThread.Snippet.TopLevelComment,
                                           response.CommentRepliesDict[commentThread.Id]);

                return channel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */
        public PaginatedVirtualScrollResult<Youtube_Video> Execute_Youtube_Video_List(YoutubeVideoDetailsRequest request)
        {
            try
            {
                var firstPageToken = request.PageToken;
                var results = new List<Youtube_Video>();

                var response = ProcessVirtualScrollPaginatedRequest(request, pageToken =>
                {
                    return _youtubeService.GetVideoDetails(new YoutubeVideoDetailsRequest(request, pageToken));
                });

                foreach (var video in response.Items)
                {
                    results.Add(AddUpdateVideo(video));
                }

                return new PaginatedVirtualScrollResult<Youtube_Video>(results,
                                                                       response.NextPageToken,
                                                                       response.TotalResultCount,
                                                                       response.TotalPageCount,
                                                                       response.PageSize, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PaginatedVirtualScrollResult<Youtube_Playlist> Execute_Youtube_Playlist_List(YoutubePlaylistRequest request)
        {
            try
            {
                var firstPageToken = request.PageToken;
                var results = new List<Youtube_Playlist>();

                var response = ProcessVirtualScrollPaginatedRequest(request, pageToken =>
                {
                    return _youtubeService.GetPlaylists(new YoutubePlaylistRequest(request, pageToken));
                });

                foreach (var playlist in response.Items)
                {
                    results.Add(AddUpdatePlaylist(playlist));
                }

                return new PaginatedVirtualScrollResult<Youtube_Playlist>(results,
                                                                          response.NextPageToken,
                                                                          response.TotalResultCount,
                                                                          response.TotalPageCount,
                                                                          response.PageSize, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PaginatedVirtualScrollResult<Youtube_PlaylistItem> Execute_Youtube_PlaylistItem_List(YoutubePlaylistItemRequest request)
        {
            try
            {
                // Query for existing playlist
                var playlistEntity = _dbContext.Youtube_Playlist.FirstOrDefault(x => x.Id == request.PlaylistId);

                if (playlistEntity == null)
                {
                    request.MessageHandler(new ServiceMessageEventArgs(new Exception("Error retrieving playlist"), string.Format("No playlist found for request:  PlaylistId: {0}", request.PlaylistId)));

                    return new PaginatedVirtualScrollResult<Youtube_PlaylistItem>(Array.Empty<Youtube_PlaylistItem>(),
                                                                                  null, 0, 0, 0, false);
                }

                var firstPageToken = request.PageToken;
                var results = new List<Youtube_PlaylistItem>();

                var response = ProcessVirtualScrollPaginatedRequest(request, pageToken =>
                {
                    return _youtubeService.GetPlaylistItems(new YoutubePlaylistItemRequest(request, pageToken));
                });

                foreach (var playlistItem in response.Items)
                {
                    results.Add(AddUpdatePlaylistItem(playlistItem));
                }

                return new PaginatedVirtualScrollResult<Youtube_PlaylistItem>(results,
                                                                              response.NextPageToken,
                                                                              response.TotalResultCount,
                                                                              response.TotalPageCount,
                                                                              response.PageSize, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PaginatedVirtualScrollResult<Youtube_CommentThread> Execute_Youtube_CommentThread_List(YoutubeCommentThreadRequest request)
        {
            try
            {
                var firstPageToken = request.PageToken;
                var results = new List<Youtube_CommentThread>();

                var response = ProcessVirtualScrollPaginatedRequest(request, pageToken =>
                {
                    return _youtubeService.GetCommentThreads(new YoutubeCommentThreadRequest(request, pageToken));
                });

                foreach (var commentThread in response.Items)
                {
                    results.Add(AddUpdateCommentThread(commentThread, commentThread.Snippet.TopLevelComment, commentThread.Replies.Comments));
                }

                return new PaginatedVirtualScrollResult<Youtube_CommentThread>(results,
                                                                               response.NextPageToken,
                                                                               response.TotalResultCount,
                                                                               response.TotalPageCount,
                                                                               response.PageSize, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// Processes paginated behavior for entity requests
        /// </summary>
        /// <param name="entityGetMethod">Callback to get entities:  (int currentIndex, int takeSize)</param>
        /// <returns>Completed paginated result</returns>
        private PaginatedResult<T> ProcessPaginatedRequest<T>(PaginatedRequest request,
                                                              int totalEntityCount,
                                                              Func<int, int, IEnumerable<T>> entityGetMethod)
        {
            var result = new List<T>();

            // Calculate last page index
            var remainder = 0;
            var divisionResult = Math.DivRem(totalEntityCount, request.PageSize, out remainder);
            var lastPageIndex = -1;

            // Increment current index to return with result
            var index = request.CurrentPageIndex;

            if (remainder > 0)
            {
                lastPageIndex = divisionResult;
            }
            else
                lastPageIndex = divisionResult - 1;


            switch (request.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                {
                    result.AddRange(entityGetMethod(index, request.PageSize));

                    request.MessageHandler(new PaginatedCallbackEventArgs(index + 1, index + request.PageSize + 1, totalEntityCount, request.PagingBehavior));

                    index++;
                }
                break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                {
                    var lastPageSize = ((lastPageIndex + 1) * request.PageSize) - totalEntityCount;

                    while (request.CurrentPageIndex <= lastPageIndex)
                    {
                        // Last page has the remainder size
                        if (index == lastPageIndex)
                            result.AddRange(entityGetMethod(index, lastPageSize == 0 ? request.PageSize : lastPageSize));

                        else
                            result.AddRange(entityGetMethod(index, request.PageSize));

                        request.MessageHandler(new PaginatedCallbackEventArgs(index + 1, index + request.PageSize + 1, totalEntityCount, request.PagingBehavior));

                        index++;
                    }
                }
                break;
                default:
                    throw new Exception("Unhandled PagingBehavior Type:  UnitOfWork.ProcessPaginatedRequest");
            }

            return new PaginatedResult<T>(result, totalEntityCount, lastPageIndex + 1, request.PageSize, index, true);
        }

        private PaginatedVirtualScrollResult<T> ProcessVirtualScrollPaginatedRequest<T>(PaginatedVirtualScrollRequest request,
                                                                                        Func<string, PaginatedVirtualScrollResult<T>> serviceMethod)
        {
            var result = new List<T>();
            var nextPageToken = "";
            var counter = 1;
            var totalResultCount = 0;
            var totalPageCount = 0;

            switch (request.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                {
                    // Get first page of results
                    var firstResult = serviceMethod(null);

                    request.MessageHandler(new PaginatedCallbackEventArgs(1, firstResult.Items.Count(), firstResult.TotalResultCount, request.PagingBehavior));

                    totalResultCount = firstResult.TotalResultCount;
                    totalPageCount = firstResult.TotalPageCount;

                    return firstResult;
                }
                case PagingBehavior.RunSynchronouslyToPageLimit:
                {
                    while (!string.IsNullOrWhiteSpace(nextPageToken) || counter == 1)
                    {
                        // Run remote service method
                        var serviceResult = serviceMethod(null);

                        // Add results to collection
                        result.AddRange(serviceResult.Items);

                        request.MessageHandler(new PaginatedCallbackEventArgs(counter, counter + serviceResult.Items.Count(), serviceResult.TotalResultCount, request.PagingBehavior));

                        counter++;
                        nextPageToken = serviceResult.NextPageToken;
                        totalResultCount = serviceResult.TotalResultCount;
                        totalPageCount = serviceResult.TotalPageCount;
                    }
                }
                break;
                default:
                    throw new Exception("Unhandled PagingBehavior Type:  UnitOfWork.ProcessPaginatedRequest");
            }

            return new PaginatedVirtualScrollResult<T>(result, nextPageToken, totalResultCount, totalPageCount, request.PageSize, true);
        }
        /*
        private void DoEverything()
        {
            // Procedure
            //
            // 1) Get SINGLE channel response from Youtube
            // 2) Get ALL Playlist id's from PAGED response from Youtube
            // 3) (Loop) Get ALL Playlist Items for ALL Playlists
            // 4) (Loop) Get ALL Videos for ALL Playlist Items (1-per)
            // 5) Get Comment Threads for ALL videos
            // 6) Get ALL Comment replies for ALL comment threads

            var channelResponse = this.GetChannelDetails(serviceRequest);

            var channel = channelResponse.Collection.FirstOrDefault();

            if (channel == null ||
                channel.ContentDetails == null ||
                channel.ContentDetails.RelatedPlaylists == null ||
                string.IsNullOrWhiteSpace(channel.ContentDetails.RelatedPlaylists.Uploads))
            {
                return new YoutubeChannelDetailsServiceResponse()
                {
                    Channel = channel ?? null
                };
            }

            var videos = new List<Video>();
            var playlists = new List<Playlist>();
            var playlistItems = new SimpleDictionary<string, List<PlaylistItem>>();
            var commentThreads = new SimpleDictionary<string, CommentThread>();
            var comments = new SimpleDictionary<string, List<Comment>>();

            // Playlists (with paging)
            var playlistResponse = this.GetPlaylists(new YoutubePlaylistRequest()
            {
                PlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads
            });

            // Playlists 1st response
            playlists.AddRange(playlistResponse.Collection);

            // Playlist (rest of pages)
            while (!string.IsNullOrWhiteSpace(playlistResponse.NextPageToken))
            {
                playlistResponse = this.GetPlaylists(new YoutubePlaylistRequest()
                {
                    PlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads,
                    PageToken = playlistResponse.NextPageToken,
                    UsePageToken = true
                });

                playlists.AddRange(playlistResponse.Collection);
            }

            // No Playlists, just return the channel
            if (!playlists.Any())
            {
                return new YoutubeChannelDetailsServiceResponse()
                {
                    Channel = channel ?? null
                };
            }

            // Playlist Items (with paging)
            foreach (var playlist in playlists)
            {
                playlistItems.Add(playlist.Id, new List<PlaylistItem>());

                // 1st response (for current playlist)
                var playlistItemResponse = this.GetPlaylistItems(new YoutubePlaylistItemRequest()
                {
                    PlaylistId = playlist.Id,
                });

                playlistItems[playlist.Id].AddRange(playlistItemResponse.Collection);

                // Playlist item pages (for current playlist)
                while (!string.IsNullOrWhiteSpace(playlistItemResponse.NextPageToken))
                {
                    playlistItemResponse = this.GetPlaylistItems(new YoutubePlaylistItemRequest()
                    {
                        PlaylistId = playlist.Id,
                        PageToken = playlistItemResponse.NextPageToken,
                        UsePageToken = true
                    });

                    playlistItems[playlist.Id].AddRange(playlistItemResponse.Collection);
                }
            }

            // Videos 1 per playlist item (with paging)
            foreach (var items in playlistItems.Values)
            {
                // Error with too many video ids, try using 50 as max result limit
                var requestAction = new Action<IEnumerable<PlaylistItem>>(items50 =>
                {
                    if (items50.Count() > 50)
                        throw new ArgumentException("Youtube API doesn't accept video requests for more than a max number of ID's at once");

                    var videoResponse = this.GetVideoDetails(new YoutubeVideoDetailsRequest()
                    {
                        VideoIds = new Repeatable<string>(items50.Where(x => !string.IsNullOrEmpty(x.ContentDetails?.VideoId))
                                                                 .Select(x => x.ContentDetails.VideoId).Actualize())
                    });

                    videos.AddRange(videoResponse.Collection);

                    // Paged responses for rest of videos
                    while (!string.IsNullOrWhiteSpace(videoResponse.NextPageToken))
                    {
                        videoResponse = this.GetVideoDetails(new YoutubeVideoDetailsRequest()
                        {
                            VideoIds = new Repeatable<string>(items.Select(x => x.ContentDetails.VideoId)),
                            PageToken = videoResponse.NextPageToken,
                            UsePageToken = true
                        });

                        videos.AddRange(videoResponse.Collection);
                    }
                });

                var index = 0;

                for (index = 0; index < items.Count; index += 50)
                {
                    var itemsSubset = items.GetRange(index, Math.Min(items.Count - index - 1, 50));

                    requestAction(itemsSubset);
                }
            }

            // Comment Threads 1-per video (with paging)
            foreach (var video in videos)
            {
                var requestFunc = new Func<Video, string, bool, YoutubeServiceResponse<CommentThread>>((currentVideo, pageToken, useToken) =>
                {
                    var commentThreadsResponse = this.GetCommentThreads(new YoutubeCommentThreadRequest()
                    {
                        VideoId = video.Id,
                        PageToken = pageToken,
                        UsePageToken = useToken
                    });

                    // Add new dictionary entries for comment threads
                    foreach (var thread in commentThreadsResponse.Collection)
                    {
                        // New
                        if (!commentThreads.ContainsKey(thread.Id))
                        {
                            commentThreads.Add(thread.Id, thread);
                            comments.Add(thread.Id, new List<Comment>());
                        }

                        if (thread.Snippet.TopLevelComment == null)
                            throw new Exception("Youtube top-leve-comment returned invalid data");

                        comments[thread.Id].Add(thread.Snippet.TopLevelComment);

                        if (thread.Replies != null)
                            comments[thread.Id].AddRange(thread.Replies.Comments);
                    }

                    return commentThreadsResponse;
                });

                // 1st Page
                var response = requestFunc(video, null, false);

                // Paging
                while (!string.IsNullOrWhiteSpace(response.NextPageToken))
                {
                    response = requestFunc(video, response.NextPageToken, true);
                }
            }

            return new YoutubeChannelDetailsServiceResponse()
            {
                Channel = channel,
                Videos = videos,
                Playlists = playlists,
                PlaylistItemDict = playlistItems,
                CommentRepliesDict = comments,
                CommentThreadDict = commentThreads
            };
        }*/

        #region Add / Update DB Context Implementations
        private Youtube_Channel AddUpdateChannel(YoutubeData.Channel channel)
        {
            // Check DB for existing entry
            var entity = _dbContext.Youtube_Channel.FirstOrDefault(channelEntity => channelEntity.Id == channel.Id);

            Youtube_ChannelSnippet snippet;
            Youtube_ChannelSettings channelSettings;

            // Locate Entity Data
            if (entity == null)
            {
                entity = _dbContext.Youtube_Channel.CreateObject();
                channelSettings = _dbContext.Youtube_ChannelSettings.CreateObject();
                snippet = _dbContext.Youtube_ChannelSnippet.CreateObject();

                _dbContext.Youtube_Channel.AddObject(entity);
                _dbContext.Youtube_ChannelSnippet.AddObject(snippet);
                _dbContext.Youtube_ChannelSettings.AddObject(channelSettings);
            }
            else
            {
                snippet = entity.Youtube_ChannelSnippet;
                channelSettings = entity.Youtube_ChannelSettings;
            }

            // Add / Update
            Map_Youtube_Channel(channel, entity, snippet, channelSettings);

            // Commit changes
            _dbContext.SaveChanges();

            return entity;
        }
        private Youtube_Video AddUpdateVideo(YoutubeData.Video video)
        {
            Youtube_Video videoEntity;
            Youtube_Channel channelEntity;
            Youtube_VideoStatistics statistics;
            Youtube_VideoStatus status;

            videoEntity = _dbContext.Youtube_Video.FirstOrDefault(x => x.Id == video.Id);
            channelEntity = _dbContext.Youtube_Channel.FirstOrDefault(x => x.Id == video.Snippet.ChannelId);

            if (channelEntity == null)
                throw new FormattedException("Channel data must first be queried for video details:  VideoId {0}", video.Id);


            // New
            if (videoEntity == null)
            {
                videoEntity = _dbContext.Youtube_Video.CreateObject();
                statistics = _dbContext.Youtube_VideoStatistics.CreateObject();
                status = _dbContext.Youtube_VideoStatus.CreateObject();

                _dbContext.Youtube_Video.AddObject(videoEntity);
                _dbContext.Youtube_VideoStatistics.AddObject(statistics);
                _dbContext.Youtube_VideoStatus.AddObject(status);
            }

            // Existing
            else
            {
                statistics = videoEntity.Youtube_VideoStatistics;
                status = videoEntity.Youtube_VideoStatus;
            }

            Map_Youtube_Video(video, videoEntity, channelEntity, statistics, status);

            // Commit changes
            _dbContext.SaveChanges();

            return videoEntity;
        }
        private Youtube_Playlist AddUpdatePlaylist(YoutubeData.Playlist playlist)
        {
            Youtube_Playlist entity;
            Youtube_Channel channel;

            entity = _dbContext.Youtube_Playlist.FirstOrDefault(x => x.Id == playlist.Id);
            channel = _dbContext.Youtube_Channel.FirstOrDefault(x => x.Id == playlist.Snippet.ChannelId);

            // Channel must be available
            if (channel == null)
                throw new FormattedException("No local channel for playlist/channel query:  {0} / {1}", playlist.Id, playlist.Snippet.ChannelId);

            // New
            if (entity == null)
            {
                entity = _dbContext.Youtube_Playlist.CreateObject();

                _dbContext.Youtube_Playlist.AddObject(entity);
            }

            // Existing
            else
            {
                // Nothing to do
            }

            Map_Youtube_Playlist(playlist, entity, channel);

            // Commit changes
            _dbContext.SaveChanges();

            return entity;
        }
        private Youtube_PlaylistItem AddUpdatePlaylistItem(YoutubeData.PlaylistItem playlistItem)
        {
            // Query for existing playlist
            var playlistEntity = _dbContext.Youtube_Playlist.FirstOrDefault(x => x.Id == playlistItem.Snippet.PlaylistId);

            if (playlistEntity == null)
                throw new FormattedException("No playlist found for playlist items request:  {0}", playlistItem.Snippet.PlaylistId);

            Youtube_PlaylistItem entity;
            Youtube_Channel channel;
            Youtube_Channel ownerChannel;

            entity = _dbContext.Youtube_PlaylistItem.FirstOrDefault(x => x.Id == playlistItem.Id);
            channel = _dbContext.Youtube_Channel.FirstOrDefault(x => x.Id == playlistItem.Snippet.ChannelId);
            ownerChannel = _dbContext.Youtube_Channel.FirstOrDefault(x => x.Id == playlistItem.Snippet.VideoOwnerChannelId);

            // Channel / Owner Channel must be available
            if (ownerChannel == null)
                throw new FormattedException("No local owner channel for playlist item/channel query:  {0} / {1}",
                                              playlistItem.Id, playlistItem.Snippet.VideoOwnerChannelId);

            if (channel == null)
                throw new FormattedException("No local channel for playlist item/channel query:  {0} / {1}",
                                              playlistItem.Id, playlistItem.Snippet.ChannelId);

            // New
            if (entity == null)
            {
                entity = _dbContext.Youtube_PlaylistItem.CreateObject();

                _dbContext.Youtube_PlaylistItem.AddObject(entity);
            }

            // Existing
            else
            {
                // Nothing to do
            }

            Map_Youtube_PlaylistItem(playlistItem, entity, playlistEntity, channel);

            // Commit changes
            _dbContext.SaveChanges();

            return entity;
        }
        private Youtube_CommentThread AddUpdateCommentThread(YoutubeData.CommentThread commentThread, YoutubeData.Comment topLevelComment, IEnumerable<YoutubeData.Comment> replies)
        {
            // Thread
            Youtube_CommentThread entity = _dbContext.Youtube_CommentThread.FirstOrDefault(x => x.Id == commentThread.Id);
            Youtube_Video video = _dbContext.Youtube_Video.FirstOrDefault(x => x.Id == commentThread.Snippet.VideoId);
            Youtube_Comment threadComment = _dbContext.Youtube_Comment.FirstOrDefault(x => x.CommentThreadId == commentThread.Id && x.IsTopLevelComment);

            // Reply
            Youtube_Comment replyEntity;

            if (video == null)
                throw new FormattedException("No video, or channel, found for comment thread {0}", commentThread.Id);

            // New
            if (entity == null)
            {
                entity = _dbContext.Youtube_CommentThread.CreateObject();
                threadComment = _dbContext.Youtube_Comment.CreateObject();

                _dbContext.Youtube_Comment.AddObject(threadComment);
                _dbContext.Youtube_CommentThread.AddObject(entity);
            }
            // Existing
            else
            {
                threadComment.Youtube_CommentThread = entity;
            }

            Map_Youtube_CommentThread(commentThread, entity, threadComment, video);

            // Commit changes
            _dbContext.SaveChanges();

            // Reply Comments
            foreach (var reply in replies)
            {
                replyEntity = _dbContext.Youtube_Comment.FirstOrDefault(x => x.Id == reply.Id);

                // New
                if (replyEntity == null)
                {
                    replyEntity = _dbContext.Youtube_Comment.CreateObject();

                    _dbContext.Youtube_Comment.AddObject(replyEntity);
                }

                // Existing
                else
                {
                    // Nothing to do
                }

                Map_Youtube_Comment(reply, replyEntity, entity);

                // Commit changes
                _dbContext.SaveChanges();
            }

            return entity;
        }
        #endregion

        #region Mapper Methods
        private void Map_Youtube_Channel(YoutubeData.Channel channel,
                                        Youtube_Channel entity,
                                        Youtube_ChannelSnippet snippet,
                                        Youtube_ChannelSettings settings)
        {
            // Primary fields
            entity.Id = channel.Id;

            // Channel Snippet, Thumbnail Details
            Map_Youtube_ChannelSnippet(channel.Snippet, snippet);

            // Content Details
            entity.ChannelContentDetails_RelatedPlaylistsData_Favorites = channel.ContentDetails.RelatedPlaylists.Favorites;
            entity.ChannelContentDetails_RelatedPlaylistsData_Likes = channel.ContentDetails.RelatedPlaylists.Likes;
            entity.ChannelContentDetails_RelatedPlaylistsData_Uploads = channel.ContentDetails.RelatedPlaylists.Uploads;
            entity.ChannelContentDetails_RelatedPlaylistsData_WatchHistory = channel.ContentDetails.RelatedPlaylists.WatchHistory;
            entity.ChannelContentDetails_RelatedPlaylistsData_WatchLater = channel.ContentDetails.RelatedPlaylists.WatchLater;

            // Content Owner Details
            entity.ContentOwnerDetails_ContentOwner = channel.ContentOwnerDetails?.ContentOwner;
            entity.ContentOwnerDetails_TimeLinkedDateTimeOffset = channel.ContentOwnerDetails?.TimeLinkedDateTimeOffset;

            // Statistics
            entity.Statistics_CommentCount = (long?)(channel.Statistics?.CommentCount ?? 0);
            entity.Statistics_HiddenSubscriberCount = channel.Statistics?.HiddenSubscriberCount;
            entity.Statistics_SubscriberCount = (long?)(channel.Statistics?.SubscriberCount ?? 0);
            entity.Statistics_VideoCount = (long?)(channel.Statistics?.VideoCount ?? 0);
            entity.Statistics_ViewCount = (long?)(channel.Statistics?.ViewCount ?? 0);

            // Status
            entity.Status_MadeForKids = channel.Status?.MadeForKids;
            entity.Status_PrivacyStatus = channel.Status?.PrivacyStatus;
            entity.Status_SelfDeclaredMadeForKids = channel.Status?.SelfDeclaredMadeForKids;

            // Branding Settings
            entity.BannerExternalUrl = channel.BrandingSettings?.Image?.BannerExternalUrl;
            entity.BannerImageUrl = channel.BrandingSettings?.Image?.BannerImageUrl;

            // Branding Settings -> Channel Settings
            Map_Youtube_ChannelSettings(channel.BrandingSettings.Channel, settings, entity);

            // Foreign Keys: Snippet, Channel Settings
            entity.Youtube_ChannelSettings = settings;
            entity.Youtube_ChannelSnippet = snippet;
        }

        private void Map_Youtube_Video(YoutubeData.Video video,
                                       Youtube_Video entity,
                                       Youtube_Channel channel,
                                       Youtube_VideoStatistics statistics,
                                       Youtube_VideoStatus status)
        {
            entity.Id = video.Id;
            entity.MonetizationDetails_AccessPolicy_Allowed = video.MonetizationDetails?.Access?.Allowed;

            entity.VideoSnippet_CategoryId = video.Snippet.CategoryId;
            entity.VideoSnippet_ChannelId = video.Snippet.ChannelId;
            entity.VideoSnippet_DefaultLanguage = video.Snippet.DefaultLanguage;
            entity.VideoSnippet_Localized_Description = video.Snippet.Localized.Description;
            entity.VideoSnippet_Localized_Title = video.Snippet.Localized.Title;
            entity.VideoSnippet_PublishedAtDateTimeOffset = video.Snippet.PublishedAtDateTimeOffset;

            entity.VideoSnippet_ThumbnailDetails_Default_Url = video.Snippet?.Thumbnails?.Default__?.Url ?? "";

            statistics.CommentCount = (long?)video.Statistics.CommentCount;
            statistics.DislikeCount = (long?)video.Statistics.DislikeCount;
            statistics.FavoriteCount = (long)(video.Statistics.FavoriteCount ?? 0);
            statistics.LikeCount = (long)(video.Statistics.LikeCount ?? 0);
            statistics.ViewCount = (long)(video.Statistics.ViewCount ?? 0);

            status.Description = "FIX THIS FIELD";
            status.License = video.Status.License;
            status.MadeForKids = video.Status.MadeForKids;
            status.PrivacyStatus = video.Status.PrivacyStatus;
            status.PublishAtDateTimeOffset = video.Status.PublishAtDateTimeOffset;
            status.RejectionReason = video.Status.RejectionReason;
            status.SelfDeclaredMadeForKids = video.Status.SelfDeclaredMadeForKids;
            status.UploadStatus = video.Status.UploadStatus;

            // Foreign Keys (These may already have been set)
            entity.Youtube_Channel = channel;
            entity.Youtube_VideoStatistics = statistics;
            entity.Youtube_VideoStatus = status;
        }

        private void Map_Youtube_ChannelSnippet(ChannelSnippet snippet, Youtube_ChannelSnippet entity)
        {
            entity.Country = snippet.Country;
            entity.CustomUrl = snippet.CustomUrl;
            entity.DefaultLanguage = snippet.DefaultLanguage;
            entity.Description = snippet.Description;
            entity.Localized_Description = snippet.Localized.Description;
            entity.Localized_Title = snippet.Localized.Title;
            entity.PublishedAtDateTimeOffset = snippet.PublishedAtDateTimeOffset;
            entity.Title = snippet.Title;

            entity.ThumbnailDetails_Default__Url = snippet.Thumbnails?.Default__?.Url ?? "";
        }
        private void Map_Youtube_ChannelSettings(ChannelSettings settings, Youtube_ChannelSettings entity, Youtube_Channel channel)
        {
            entity.Country = settings.Country;
            entity.DefaultLanguage = settings.DefaultLanguage;
            entity.Description = settings.Description;
            entity.ProfileColor = settings.ProfileColor;
            entity.Title = settings.Title;
            entity.TrackingAnalyticsAccountId = settings.TrackingAnalyticsAccountId;
            entity.Youtube_Channel = channel;
        }
        private void Map_Youtube_SearchResult(SearchResult searchResult, Youtube_SearchResult entity)
        {
            entity.Id_ChannelId = searchResult.Id.ChannelId;
            entity.Id_PlaylistId = searchResult.Id.PlaylistId;
            entity.Id_VideoId = searchResult.Id.VideoId;
            entity.Snippet_ChannelId = searchResult.Snippet.ChannelId;
            entity.Snippet_ChannelTitle = searchResult.Snippet.ChannelTitle;
            entity.Snippet_Description = searchResult.Snippet.Description;
            entity.Snippet_PublishedAtDateTimeOffset = searchResult.Snippet.PublishedAtDateTimeOffset;
            entity.Snippet_Title = searchResult.Snippet.Title;
            entity.Snippet_ThumbnailDetails_Default__Url = searchResult.Snippet?.Thumbnails?.Default__?.Url ?? "";
        }
        private void Map_Youtube_Playlist(Playlist playlist, Youtube_Playlist entity, Youtube_Channel channel)
        {
            entity.Id = playlist.Id;
            entity.PlaylistSnippet_PublishedAtDateTimeOffset = playlist.Snippet.PublishedAtDateTimeOffset;
            entity.PlaylistSnippet_Title = playlist.Snippet.Title;
            entity.PlaylistSnippet_ThumnailDetails_Default__Url = playlist.Snippet?.Thumbnails?.Default__?.Url ?? "";

            // Foreign Keys
            entity.Youtube_Channel = channel;
        }
        private void Map_Youtube_PlaylistItem(PlaylistItem playlistItem,
                                              Youtube_PlaylistItem entity,
                                              Youtube_Playlist playlistEntity,
                                              Youtube_Channel channel)
        {
            entity.Id = playlistItem.Id;
            entity.PlaylistContentDetails_Note = playlistItem.ContentDetails.Note;
            entity.PlaylistContentDetails_VideoId = playlistItem.ContentDetails.VideoId; // Nullable (Not required)
            entity.PlaylistContentDetails_VideoPublishedAtDateTimeOffset = playlistItem.ContentDetails.VideoPublishedAtDateTimeOffset;
            entity.PlaylistItemSnippet_Position = playlistItem.Snippet.Position;
            entity.PlaylistItemSnippet_PublishedAtDateTimeOffset = playlistItem.Snippet.PublishedAtDateTimeOffset;
            entity.PlaylistItemSnippet_Title = playlistItem.Snippet.Title;
            entity.PlaylistItemSnippet_VideoOwnerChannelTitle = playlistItem.Snippet.Title;
            entity.PlaylistItemStatus_PrivacyStatus = playlistItem.Status.PrivacyStatus;
            entity.PlaylistItemSnippet_ThumbnailDetails_Default_Url = playlistItem.Snippet?.Thumbnails?.Default__?.Url ?? "";
            entity.PlaylistItemSnippet_VideoOwnerChannelId = playlistItem.Snippet.VideoOwnerChannelId; // Nullable (Not required)

            // Foreign Keys
            entity.Youtube_Playlist = playlistEntity;
        }
        private void Map_Youtube_CommentThread(YoutubeData.CommentThread thread,
                                               Youtube_CommentThread entity,
                                               Youtube_Comment entityComment,   // Top Level Comment
                                               Youtube_Video video)
        {
            entity.Id = thread.Id;

            entity.IsPublic = thread.Snippet.IsPublic;
            entity.TotalReplyCount = thread.Snippet.TotalReplyCount;
            entity.VideoId = video.Id;
            entity.Youtube_Video = video;

            // Top level comment -> snippet
            entityComment.AuthorChannelId_Value = thread.Snippet.TopLevelComment.Snippet.AuthorChannelId.Value;
            entityComment.AuthorChannelUrl = thread.Snippet.TopLevelComment.Snippet.AuthorChannelUrl;
            entityComment.AuthorDisplayName = thread.Snippet.TopLevelComment.Snippet.AuthorDisplayName;
            entityComment.AuthorProfileImageUrl = thread.Snippet.TopLevelComment.Snippet.AuthorProfileImageUrl;
            entityComment.CommentThreadId = thread.Id;
            entityComment.IsTopLevelComment = true;
            entityComment.LikeCount = thread.Snippet.TopLevelComment.Snippet.LikeCount;
            entityComment.ModerationStatus = thread.Snippet.TopLevelComment.Snippet.ModerationStatus;
            entityComment.PublishedAtDateTimeOffset = thread.Snippet.TopLevelComment.Snippet.PublishedAtDateTimeOffset;
            entityComment.TextDisplay = thread.Snippet.TopLevelComment.Snippet.TextDisplay;
            entityComment.TextOriginal = thread.Snippet.TopLevelComment.Snippet.TextOriginal;
            entityComment.UpdatedAtDateTimeOffset = thread.Snippet.TopLevelComment.Snippet.UpdatedAtDateTimeOffset;

            // Foreign Keys
            entity.Youtube_Video = video;
            entityComment.Youtube_CommentThread = entity;
        }

        private void Map_Youtube_Comment(Comment comment,
                                         Youtube_Comment entity,
                                         Youtube_CommentThread thread)
        {
            entity.Id = comment.Id;

            // Snippet
            entity.AuthorChannelId_Value = comment.Snippet.AuthorChannelId.Value;
            entity.AuthorChannelUrl = comment.Snippet.AuthorChannelUrl;
            entity.AuthorDisplayName = comment.Snippet.AuthorDisplayName;
            entity.AuthorProfileImageUrl = comment.Snippet.AuthorProfileImageUrl;
            entity.LikeCount = comment.Snippet.LikeCount;
            entity.ModerationStatus = comment.Snippet.ModerationStatus;
            entity.PublishedAtDateTimeOffset = comment.Snippet.PublishedAtDateTimeOffset;
            entity.TextDisplay = comment.Snippet.TextDisplay;
            entity.TextOriginal = comment.Snippet.TextOriginal;
            entity.UpdatedAtDateTimeOffset = comment.Snippet.UpdatedAtDateTimeOffset;
            entity.CommentThreadId = thread.Id;

            // Foreign Key
            entity.Youtube_CommentThread = thread;
        }
        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _dbContext.Connection.Close();
                _dbContext.Dispose();

                _youtubeService.Dispose();

                _disposed = true;
            }
        }

        public Youtube_Channel SearchUpdateAllChannelDetails(YoutubeChannelDetailsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
