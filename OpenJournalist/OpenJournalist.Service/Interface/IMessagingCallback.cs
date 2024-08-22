using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Interface
{
    public interface IMessagingCallback<T> where T : MessagingCallbackEventArgsBase
    {
        SimpleEventHandler<T> MessageHandler { get; }
    }
}
