using System;

namespace OpenJournalist.Service.Model.Message
{
    /// <summary>
    /// Class that contains data for a messaging callback attachment to a service
    /// </summary>
    public class PaginatedCallbackEventArgs : MessagingCallbackEventArgsBase
    {
        /// <summary>
        /// Number "from" of first result (of paged collection)
        /// </summary>
        public int ResultFrom { get; private set; }

        /// <summary>
        /// Number "to" of first result (of paged collection)
        /// </summary>
        public int ResultTo { get; private set; }   

        /// <summary>
        /// Number "total" of entire collection (paged)
        /// </summary>
        public int ResultTotal { get; private set; }

        /// <summary>
        /// Adds the paging behavior data to the message arguments to help taylor the message
        /// </summary>
        public PagingBehavior PagingBehavior { get; private set; }

        public PaginatedCallbackEventArgs(int from, int to, int total, PagingBehavior pagingBehavior) 
        {
            this.ResultFrom = from;
            this.ResultTo = to;
            this.ResultTotal = total;
            this.PagingBehavior = pagingBehavior;
        }
    }
}
