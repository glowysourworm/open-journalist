using System;
using System.Linq;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using OpenJournalist.Service.Extension;
using OpenJournalist.Service.Interface;
using OpenJournalist.Service.Model;
using OpenJournalist.Service.Model.Youtube;

namespace OpenJournalist.Service
{
    public class YoutubeService : IYoutubeService
    {
        public IClientService ServiceBase { get; private set; }

        public YoutubeService(string apiKey, string clientId, string clientSecret)
        {
            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            },
            new string[]
            {
                YouTubeService.ScopeConstants.Youtube,
                YouTubeService.ScopeConstants.YoutubeForceSsl,
                YouTubeService.ScopeConstants.YoutubeReadonly
            },
            "rdolan.music.2@gmail.com",
            CancellationToken.None,
            new FileDataStore(".\\"));                          // Must have file data store for Google to cache data (bearer tokens! required!)

            this.ServiceBase = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials.Result,
                ApiKey = apiKey,
                ApplicationName = "youtube#commentThreadList"
            });
        }

        #region IYoutubeService
        public PaginatedVirtualScrollResult<SearchResult> Search(YoutubeBasicSearchRequest serviceRequest)
        {
            // SearchListRequest
            var request = (this.ServiceBase as YouTubeService).Search.List(YoutubeConstants.SearchParts.ToRepeatable());

            // These filters will dictate how the service responds - errors are likely for improper setup.
            request.Type = YoutubeConstants.SearchTypeChannel;
            request.MaxResults = serviceRequest.PageSize;
            request.Q = serviceRequest.Search;

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.Search");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<SearchResult>(
                            result.Items ?? Array.Empty<SearchResult>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }
        public PaginatedVirtualScrollResult<SearchResult> SearchUser(YoutubeUserChannelsSearchRequest serviceRequest)
        {
            // SearchListRequest
            var request = (this.ServiceBase as YouTubeService).Search.List(YoutubeConstants.SearchParts.ToRepeatable());

            request.MaxResults = serviceRequest.PageSize;
            request.Type = new Repeatable<string>(new string[] { YoutubeConstants.SearchTypeVideo, YoutubeConstants.SearchTypeChannel });

            // TODO: CHANGE THIS TO HANDLE
            request.ForMine = true;

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.SearchUser");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<SearchResult>(
                            result.Items ?? Array.Empty<SearchResult>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }
        public PaginatedVirtualScrollResult<Video> GetVideoDetails(YoutubeVideoDetailsRequest serviceRequest)
        {
            // Create Youtube video list request
            var request = (this.ServiceBase as YouTubeService).Videos.List(YoutubeConstants.VideoParts.ToRepeatable());

            if (string.IsNullOrEmpty(serviceRequest.VideoIds.ToString()))
                throw new Exception("Must set the VideoIds field for YoutubeVideoDetailsRequest");

            // Set Video Ids for the search
            request.Id = new Repeatable<string>(serviceRequest.VideoIds);

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.GetVideoDetails");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<Video>(
                            result.Items ?? Array.Empty<Video>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }
        public SingleResult<Channel> GetChannelDetails(YoutubeChannelDetailsRequest serviceRequest)
        {
            // Create Youtube channel list request
            var request = (this.ServiceBase as YouTubeService).Channels.List(YoutubeConstants.ChannelParts.ToRepeatable());

            // Try and get all public channels for a user
            // request.ForUsername

            // Set Channel Id for the search
            request.Id = serviceRequest.ChannelId;

            // No paging for this request
            request.PageToken = null;

            // Call Youtube Service
            var result = request.Execute();

            return new SingleResult<Channel>(result.Items.FirstOrDefault(), result.Items != null && result.Items.Count > 0);
        }

        public PaginatedVirtualScrollResult<Playlist> GetPlaylists(YoutubePlaylistRequest serviceRequest)
        {
            // Create Youtube channel list request
            var request = (this.ServiceBase as YouTubeService).Playlists.List(YoutubeConstants.PlaylistParts.ToRepeatable());

            request.MaxResults = serviceRequest.PageSize;

            // Set (Playlist) Id. Channel id searches have permissions issues with Youtube
            if (!String.IsNullOrWhiteSpace(serviceRequest.PlaylistId))
                request.Id = serviceRequest.PlaylistId.ToRepeatable();

            else
                throw new Exception("Must specify either PlaylistId for YoutubePlaylistRequest");

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.GetPlaylists");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<Playlist>(
                            result.Items ?? Array.Empty<Playlist>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }

        public PaginatedVirtualScrollResult<PlaylistItem> GetPlaylistItems(YoutubePlaylistItemRequest serviceRequest)
        {
            // Create Youtube channel list request
            var request = (this.ServiceBase as YouTubeService).PlaylistItems.List(YoutubeConstants.PlaylistItemParts.ToRepeatable());

            // Set Channel Id for the search
            request.PlaylistId = serviceRequest.PlaylistId;
            request.MaxResults = serviceRequest.PageSize;

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.GetPlaylistItems");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<PlaylistItem>(
                            result.Items ?? Array.Empty<PlaylistItem>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }

        public PaginatedVirtualScrollResult<CommentThread> GetCommentThreads(YoutubeCommentThreadRequest serviceRequest)
        {
            // Create Youtube video list request
            var request = (this.ServiceBase as YouTubeService).CommentThreads
                                                              .List(YoutubeConstants.CommentThreadParts.ToRepeatable());

            // Search configuration
            request.MaxResults = serviceRequest.PageSize;

            if (!string.IsNullOrEmpty(serviceRequest.VideoId))
                request.VideoId = serviceRequest.VideoId;

            else
                throw new Exception("Youtube commentThread request must specify either video id");

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.GetPlaylistItems");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<CommentThread>(
                            result.Items ?? Array.Empty<CommentThread>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }
        public PaginatedVirtualScrollResult<Comment> GetComments(YoutubeCommentsRequest serviceRequest)
        {
            // Create Youtube video list request
            var request = (this.ServiceBase as YouTubeService).Comments
                                                              .List(YoutubeConstants.CommentParts.ToRepeatable());

            // Search configuration
            request.MaxResults = serviceRequest.PageSize;

            if (!string.IsNullOrEmpty(serviceRequest.CommentId))
                request.Id = serviceRequest.CommentId;

            else
                throw new Exception("Youtube comment request must specify comment id");

            switch (serviceRequest.PagingBehavior)
            {
                case PagingBehavior.NoPaging:
                case PagingBehavior.SinglePageResult:
                    request.PageToken = null;
                    break;
                case PagingBehavior.RunSynchronouslyToPageLimit:
                    request.PageToken = serviceRequest.PageToken;
                    break;
                default:
                    throw new Exception("Unhandled paging type:  YoutubeService.GetComments");
            }

            // Call Youtube Service
            var result = request.Execute();

            return new PaginatedVirtualScrollResult<Comment>(
                            result.Items ?? Array.Empty<Comment>(),
                            result.NextPageToken,
                            result.PageInfo.TotalResults ?? 0,
                            result.PageInfo.TotalResults % result.PageInfo.ResultsPerPage ?? 0,
                            serviceRequest.PageSize,
                            result.Items != null);
        }
        /*
        public YoutubeChannelDetailsServiceResponse GetAllChannelInformation(YoutubeChannelDetailsRequest serviceRequest)
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
        }

        */
        #endregion

        public void Dispose()
        {
            this.ServiceBase.Dispose();
        }
    }
}
