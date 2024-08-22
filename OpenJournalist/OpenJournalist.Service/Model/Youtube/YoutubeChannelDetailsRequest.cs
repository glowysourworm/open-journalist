using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Youtube
{
    public class YoutubeChannelDetailsRequest : SingleRequest
    {
        public string ChannelId { get; set; }

        public YoutubeChannelDetailsRequest(string channelId,
                                            SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler)

            : base(messageHandler)
        {
            this.ChannelId = channelId;
        }
    }
}
