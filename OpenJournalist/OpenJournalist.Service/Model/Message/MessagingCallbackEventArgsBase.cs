using System;

namespace OpenJournalist.Service.Model.Message
{
    public abstract class MessagingCallbackEventArgsBase
    {
        /// <summary>
        /// Message from service for callback
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Exception indicates there was a thrown exception on the target service
        /// </summary>
        public Exception ServiceException { get; private set; }

        public MessagingCallbackEventArgsBase(Exception serviceException = null, string message = null)
        {
            this.Message = message;
            this.ServiceException = serviceException;
        }
    }
}
