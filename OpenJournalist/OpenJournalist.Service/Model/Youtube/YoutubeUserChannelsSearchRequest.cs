using System;

using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeUserChannelsSearchRequest : PaginatedVirtualScrollRequest
    {
        public string UserHandle { get; private set; }
        public string UserName { get; private set; }

        /// <summary>
        /// Constructs new instance of search request
        /// </summary>
        /// <param name="userHandle">Set to user's handle. Set either this or userName</param>
        /// <param name="userName">Set to user's handle. Set either this or userHandle</param>
        public YoutubeUserChannelsSearchRequest(string userHandle,
                                                string userName,
                                                string pageToken,
                                                PagingBehavior pagingBehavior,
                                                SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                                int pageSize)
            : base(pageToken, pagingBehavior, messageHandler, pageSize)
        {
            if (string.IsNullOrWhiteSpace(userHandle) &&
                string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Must set either userHandle, or userName:  YoutubeUserChannelsSearchRequest");

            this.UserName = userName;
            this.UserHandle = userHandle;
        }
    }
}
