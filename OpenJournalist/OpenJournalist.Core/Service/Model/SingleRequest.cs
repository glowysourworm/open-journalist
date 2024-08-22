
using OpenJournalist.Core.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Core.Service.Model
{
    public class SingleRequest : RequestBase<MessagingCallbackEventArgsBase>
    {
        public SingleRequest(SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler) : base(messageHandler)
        {
        }
    }
}
