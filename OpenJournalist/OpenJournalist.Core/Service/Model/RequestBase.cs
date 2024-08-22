
using OpenJournalist.Core.Service.Interface;
using OpenJournalist.Core.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Core.Service.Model
{
    public abstract class RequestBase<T> : IMessagingCallback<T> where T : MessagingCallbackEventArgsBase
    {
        /// <summary>
        /// Handler to report back messages from the service call
        /// </summary>
        public SimpleEventHandler<T> MessageHandler { get; private set; }

        public RequestBase(SimpleEventHandler<T> messageHandler)
        {
            this.MessageHandler = messageHandler;
        }
    }
}
