using System.Collections.Generic;

using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeVideoDetailsRequest : PaginatedVirtualScrollRequest
    {
        public IEnumerable<string> VideoIds { get; private set; }

        public YoutubeVideoDetailsRequest(IEnumerable<string> videoIds,
                                          string pageToken,
                                          PagingBehavior pageBehavior,
                                          SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(pageToken, pageBehavior, messageHandler, YoutubeConstants.MaxResults)
        {
            this.VideoIds = videoIds;
        }

        public YoutubeVideoDetailsRequest(YoutubeVideoDetailsRequest previousPageRequest, string nextPageToken)

            : base(nextPageToken, previousPageRequest.PagingBehavior, previousPageRequest.MessageHandler, previousPageRequest.PageSize)
        {
            this.VideoIds = previousPageRequest.VideoIds;
        }
    }
}
