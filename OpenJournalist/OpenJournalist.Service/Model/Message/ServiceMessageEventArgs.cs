using System;

namespace OpenJournalist.Service.Model.Message
{
    public class ServiceMessageEventArgs : MessagingCallbackEventArgsBase
    {
        public ServiceMessageEventArgs(Exception exception = null, string message = null) : base(exception, message)
        {

        }
    }
}
