using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using OpenJournalist.Data;
using OpenJournalist.Data.Interface;
using OpenJournalist.Data.Model.Youtube;
using OpenJournalist.Service.Model;
using OpenJournalist.Service.Model.Local;
using OpenJournalist.Service.Model.Message;
using OpenJournalist.Service.Model.Youtube;
using OpenJournalist.ViewModel;
using OpenJournalist.ViewModel.Local;
using OpenJournalist.ViewModel.SearchResult;
using OpenJournalist.ViewModel.Youtube;

using WpfCustomUtilities.Extensions;
using WpfCustomUtilities.Extensions.Collection;

namespace OpenJournalist
{
    /// <summary>
    /// Primary component for querying and storing data from social media web services
    /// </summary>
    public class Controller
    {
        readonly string _apiKey;
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _localDatabaseConnectionString;

        public Controller(ConfigurationViewModel configuration)
        {
            // Youtube service connection
            _apiKey = configuration.ApiKey;
            _clientId = configuration.ClientID;
            _clientSecret = configuration.ClientSecret;
            _localDatabaseConnectionString = configuration.LocalDatabaseConnectionString;
        }

        private IUnitOfWork CreateConnection()
        {
            return new UnitOfWork(_apiKey, _clientId, _clientSecret, _localDatabaseConnectionString);
        }

        /// <summary>
        /// Returns entire collection of search results from local database
        /// </summary>
        public async Task<IEnumerable<YoutubeSearchResultViewModel>> GetLocalSearchResults(SearchPaginatedRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    request.MessageHandler(new ServiceMessageEventArgs(null, "Connecting to local database"));

                    using (var unitOfWork = CreateConnection())
                    {
                        request.MessageHandler(new ServiceMessageEventArgs(null, "Retrieving local search results"));

                        return unitOfWork.GetYoutubeSearchResults(request)
                                         .Items
                                         .Select(result =>
                                         {
                                             return CreateSearchViewModel(result);
                                         })
                                         .Actualize();
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns entire set of channel entities from local database
        /// </summary>
        public async Task<IEnumerable<ChannelViewModel>> GetLocalChannels(SearchPaginatedRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        return unitOfWork.GetChannels(request)
                                         .Items
                                         .Select(result =>
                        {
                            return CreateChannelViewModel(result);

                        }).Actualize();
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns channel from local database
        /// </summary>
        public async Task<YoutubeChannelViewModel> GetChannel(string channelId)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        var channel = unitOfWork.GetLocalChannel(channelId);

                        return CreateChannelViewModel(channel);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executes basic search as a user on the Youtube platform, and stores the results in the local database
        /// </summary>
        public async Task<PaginatedVirtualScrollResult<YoutubeSearchResultViewModel>> Execute_Youtube_BasicSearch(YoutubeBasicSearchRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        var paginatedResult = unitOfWork.Execute_Youtube_BasicSearch(request);

                        var results = paginatedResult
                                        .Items
                                        .Select(x => CreateSearchViewModel(x))
                                        .Actualize();

                        return new PaginatedVirtualScrollResult<YoutubeSearchResultViewModel>(
                                    results,
                                    paginatedResult.NextPageToken,
                                    paginatedResult.TotalResultCount,
                                    paginatedResult.TotalPageCount,
                                    paginatedResult.PageSize,
                                    paginatedResult.Success);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executes search for channel details, and updates local database
        /// </summary>
        public async Task<YoutubeChannelViewModel> Execute_Youtube_ChannelList(YoutubeChannelDetailsRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        // Channel search from Youtube
                        var channel = unitOfWork.Execute_Youtube_Channel_List(request);

                        if (channel == null)
                            return null;

                        // Take detail model (local) and return to user
                        var model = unitOfWork.GetLocalChannel(channel.Item.Id);

                        return CreateChannelViewModel(model);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get will apply service request to look for specific channel, video, or playlist entities,
        /// pulling over extended detail about the entity.
        /// </summary>
        public async Task<PaginatedVirtualScrollResult<YoutubeVideoViewModel>> Execute_Youtube_Video_List(YoutubeVideoDetailsRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        // Channel search from Youtube
                        var result = unitOfWork.Execute_Youtube_Video_List(request);

                        // Mapped video results
                        var videos = result.Items.Select(x => CreateVideoViewModel(x)).Actualize();

                        return new PaginatedVirtualScrollResult<YoutubeVideoViewModel>(
                                    videos,
                                    result.NextPageToken,
                                    result.TotalResultCount,
                                    result.TotalPageCount,
                                    result.PageSize,
                                    result.Success);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Search that retrieves comment threads for:  1) An entire channel, or 2) A set of video (ids)
        /// </summary>
        public async Task<PaginatedVirtualScrollResult<YoutubeCommentThreadViewModel>> Execute_Youtube_CommentThread_List(YoutubeCommentThreadRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var unitOfWork = CreateConnection())
                    {
                        // Search for comment threads
                        var result = unitOfWork.Execute_Youtube_CommentThread_List(request);

                        var viewModels = result.Items
                                               .Select(thread =>
                        {
                            var threadComment = thread.Youtube_Comment.FirstOrDefault(x => x.CommentThreadId == thread.Id && x.IsTopLevelComment);

                            if (threadComment == null)
                                throw new FormattedException("Invalid Comment Thread Data (no top level comment) UnitOfWork.SearchCommentThreads");

                            return CreateCommentThreadViewModel(thread, threadComment);

                        }).Actualize();

                        return new PaginatedVirtualScrollResult<YoutubeCommentThreadViewModel>(
                                    viewModels,
                                    result.NextPageToken,
                                    result.TotalResultCount,
                                    result.TotalPageCount,
                                    result.PageSize,
                                    result.Success);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private YoutubeSearchResultViewModel CreateSearchViewModel(ChannelSearchResultView result)
        {
            return new YoutubeSearchResultViewModel()
            {
                Created = result.CapturedDateTime,
                Description = result.Description,
                ChannelId = result.YoutubeId,
                Thumbnail = result.ImageUrl,
                Title = result.Title
            };
        }
        private YoutubeSearchResultViewModel CreateSearchViewModel(Youtube_SearchResult result)
        {
            return new YoutubeSearchResultViewModel()
            {
                Created = result.Snippet_PublishedAtDateTimeOffset.HasValue ?
                          result.Snippet_PublishedAtDateTimeOffset.Value.UtcDateTime : DateTime.MinValue,
                Description = result.Snippet_Description,
                ChannelId = result.Id_ChannelId,
                Thumbnail = result.Snippet_ThumbnailDetails_Default__Url,
                Title = result.Snippet_Title
            };
        }
        private ChannelViewModel CreateChannelViewModel(ChannelView result)
        {
            return new ChannelViewModel()
            {
                CapturedDateTime = result.CapturedDateTime,
                Description = result.Description,
                Id = result.Id,
                IconUrl = result.ImageUrl,
                Platform = result.Platform,
                Title = result.Title
            };
        }
        private YoutubeChannelViewModel CreateChannelViewModel(YoutubeChannelDetailModel result)
        {
            return new YoutubeChannelViewModel()
            {
                Id = result.Channel.Id,
                BannerUrl = result.Channel.BannerImageUrl,
                IconUrl = result.Channel.Youtube_ChannelSnippet.ThumbnailDetails_Default__Url,
                MadeForKids = result.Channel.Status_MadeForKids ?? false,
                Owner = result.Channel.ContentOwnerDetails_ContentOwner,
                PrivacyStatus = result.Channel.Status_PrivacyStatus,
                SelfDeclaredMadeForKids = result.Channel.Status_SelfDeclaredMadeForKids ?? false,
                SubscriberCount = result.Channel.Statistics_SubscriberCount ?? 0,
                VideoCount = result.Channel.Statistics_VideoCount ?? 0,
                ViewCount = result.Channel.Statistics_ViewCount ?? 0,
                Description = result.Channel.Youtube_ChannelSnippet.Description,
                Title = result.Channel.Youtube_ChannelSnippet.Title,
                Videos = new ObservableCollection<YoutubeVideoViewModel>(
                    result.Videos.Select(video =>
                    {
                        return CreateVideoViewModel(video);
                    }))

            };
        }
        private YoutubeVideoViewModel CreateVideoViewModel(Youtube_Video result)
        {
            return new YoutubeVideoViewModel()
            {
                Id = result.Id,
                CategoryId = result.VideoSnippet_CategoryId,
                CommentCount = result.Youtube_VideoStatistics.CommentCount ?? 0,
                Description = result.VideoSnippet_Localized_Description,
                DislikeCount = result.Youtube_VideoStatistics.DislikeCount ?? 0,
                LikeCount = result.Youtube_VideoStatistics.LikeCount,
                ViewCount = result.Youtube_VideoStatistics.ViewCount,
                FavoriteCount = result.Youtube_VideoStatistics.FavoriteCount,
                IsMonetized = result.MonetizationDetails_AccessPolicy_Allowed ?? false,
                MadeForKids = result.Youtube_VideoStatus.MadeForKids ?? false,
                Published = result.Youtube_VideoStatus.PublishAtDateTimeOffset.HasValue ?
                            result.Youtube_VideoStatus.PublishAtDateTimeOffset.Value.UtcDateTime : DateTime.MinValue,
                RejectionReason = result.Youtube_VideoStatus.RejectionReason,
                SelfDeclaredMadeForKids = result.Youtube_VideoStatus.SelfDeclaredMadeForKids ?? false,
                ThumbnailUrl = result.VideoSnippet_ThumbnailDetails_Default_Url,
                Title = result.VideoSnippet_Localized_Title,
                UploadStatus = result.Youtube_VideoStatus.UploadStatus,
                CommentThreads = new ObservableCollection<YoutubeCommentThreadViewModel>(result.Youtube_CommentThread.Select(thread =>
                {
                    var threadComment = thread.Youtube_Comment.FirstOrDefault(x => x.Id == thread.Id && x.IsTopLevelComment);

                    if (threadComment == null)
                        throw new FormattedException("Invalid comment thread - no top level comment: CommentThreadId {0}", thread.Id);

                    return CreateCommentThreadViewModel(thread, threadComment);

                }).Actualize())
            };
        }
        private YoutubeVideoViewModel CreateVideoViewModel(YoutubeVideoDetailModel result)
        {
            return new YoutubeVideoViewModel()
            {
                Id = result.Video.Id,
                CategoryId = result.Video.VideoSnippet_CategoryId,
                CommentCount = result.Video.Youtube_VideoStatistics.CommentCount ?? 0,
                Description = result.Video.VideoSnippet_Localized_Description,
                DislikeCount = result.Video.Youtube_VideoStatistics.DislikeCount ?? 0,
                LikeCount = result.Video.Youtube_VideoStatistics.LikeCount,
                ViewCount = result.Video.Youtube_VideoStatistics.ViewCount,
                FavoriteCount = result.Video.Youtube_VideoStatistics.FavoriteCount,
                IsMonetized = result.Video.MonetizationDetails_AccessPolicy_Allowed ?? false,
                MadeForKids = result.Video.Youtube_VideoStatus.MadeForKids ?? false,
                Published = result.Video.Youtube_VideoStatus.PublishAtDateTimeOffset.HasValue ?
                            result.Video.Youtube_VideoStatus.PublishAtDateTimeOffset.Value.UtcDateTime : DateTime.MinValue,
                RejectionReason = result.Video.Youtube_VideoStatus.RejectionReason,
                SelfDeclaredMadeForKids = result.Video.Youtube_VideoStatus.SelfDeclaredMadeForKids ?? false,
                ThumbnailUrl = result.Video.VideoSnippet_ThumbnailDetails_Default_Url,
                Title = result.Video.VideoSnippet_Localized_Title,
                UploadStatus = result.Video.Youtube_VideoStatus.UploadStatus,
                CommentThreads = new ObservableCollection<YoutubeCommentThreadViewModel>(
                    result.CommentThreads.Select(thread => CreateCommentThreadViewModel(thread)).Actualize())
            };
        }
        private YoutubeCommentThreadViewModel CreateCommentThreadViewModel(YoutubeCommentThreadDetailModel thread)
        {
            return new YoutubeCommentThreadViewModel()
            {
                Comment = new YoutubeCommentViewModel()
                {
                    AuthorChannelId = thread.View.AuthorChannelId,
                    AuthorDisplayName = thread.View.AuthorDisplayName,
                    AuthorImageUrl = thread.View.AuthorChannelUrl,
                    AuthorUrl = thread.View.AuthorChannelUrl,
                    Display = thread.View.TextDisplay,
                    ModerationStatus = thread.View.ModerationStatus,
                    PublishedDate = thread.View.PublishedDateTime.HasValue ? thread.View.PublishedDateTime.Value : DateTime.MinValue,
                    UpdatedAtDate = thread.View.UpdatedDateTime.HasValue ? thread.View.UpdatedDateTime.Value : DateTime.MinValue
                },
                IsPublic = thread.CommentThread.IsPublic ?? false,
                TotalReplyCount = (int)(thread.CommentThread.TotalReplyCount ?? 0),
                Replies = new ObservableCollection<YoutubeCommentViewModel>(thread.Comments.Select(reply => CreateCommentViewModel(reply)).Actualize())
            };
        }
        private YoutubeCommentThreadViewModel CreateCommentThreadViewModel(Youtube_CommentThread thread, Youtube_Comment threadComment)
        {
            return new YoutubeCommentThreadViewModel()
            {
                Comment = new YoutubeCommentViewModel()
                {
                    AuthorChannelId = threadComment.AuthorChannelId_Value,
                    AuthorDisplayName = threadComment.AuthorDisplayName,
                    AuthorImageUrl = threadComment.AuthorProfileImageUrl,
                    AuthorUrl = threadComment.AuthorChannelUrl,
                    Display = threadComment.TextDisplay,
                    LikeCount = threadComment.LikeCount ?? 0,
                    ModerationStatus = threadComment.ModerationStatus,
                    PublishedDate = threadComment.PublishedAtDateTimeOffset.HasValue ? threadComment.PublishedAtDateTimeOffset.Value.UtcDateTime : DateTime.MinValue,
                    UpdatedAtDate = threadComment.UpdatedAtDateTimeOffset.HasValue ? threadComment.UpdatedAtDateTimeOffset.Value.UtcDateTime : DateTime.MinValue
                },
                IsPublic = thread.IsPublic ?? false,
                TotalReplyCount = (int)(thread.TotalReplyCount ?? 0),
                Replies = new ObservableCollection<YoutubeCommentViewModel>(thread.Youtube_Comment.Select(reply => CreateCommentViewModel(reply)).Actualize())
            };
        }
        private YoutubeCommentViewModel CreateCommentViewModel(Youtube_Comment entity)
        {
            return new YoutubeCommentViewModel()
            {
                AuthorChannelId = entity.AuthorChannelId_Value,
                AuthorDisplayName = entity.AuthorDisplayName,
                AuthorImageUrl = entity.AuthorProfileImageUrl,
                AuthorUrl = entity.AuthorChannelUrl,
                Display = entity.TextDisplay,
                LikeCount = (int)(entity.LikeCount ?? 0),
                ModerationStatus = entity.ModerationStatus,
                PublishedDate = (DateTime)(entity.PublishedAtDateTimeOffset?.UtcDateTime ?? DateTime.MinValue),
                UpdatedAtDate = (DateTime)(entity.UpdatedAtDateTimeOffset?.UtcDateTime ?? DateTime.MinValue)
            };
        }
        private YoutubeCommentViewModel CreateCommentViewModel(YoutubeCommentDetailModel model)
        {
            return new YoutubeCommentViewModel()
            {
                AuthorChannelId = model.Comment.AuthorChannelId_Value,
                AuthorDisplayName = model.Comment.AuthorDisplayName,
                AuthorImageUrl = model.Comment.AuthorProfileImageUrl,
                AuthorUrl = model.Comment.AuthorChannelUrl,
                Display = model.Comment.TextDisplay,
                LikeCount = (int)(model.Comment.LikeCount ?? 0),
                ModerationStatus = model.Comment.ModerationStatus,
                PublishedDate = (DateTime)(model.Comment.PublishedAtDateTimeOffset?.UtcDateTime ?? DateTime.MinValue),
                UpdatedAtDate = (DateTime)(model.Comment.UpdatedAtDateTimeOffset?.UtcDateTime ?? DateTime.MinValue)
            };
        }
    }
}
