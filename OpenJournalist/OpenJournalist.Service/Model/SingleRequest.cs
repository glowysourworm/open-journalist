using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model
{
    public class SingleRequest : RequestBase<MessagingCallbackEventArgsBase>
    {
        public SingleRequest(SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler) : base(messageHandler)
        {
        }
    }
}
