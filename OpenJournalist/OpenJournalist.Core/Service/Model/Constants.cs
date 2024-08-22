namespace OpenJournalist.Core.Service.Model
{
    public enum PagingBehavior
    {
        /// <summary>
        /// Service will use default behavior for a results search. Typically, this will be the
        /// single result, or the first page of results.
        /// </summary>
        NoPaging,

        /// <summary>
        /// Service will return a single page of results
        /// </summary>
        SinglePageResult,

        /// <summary>
        /// Service will run synchronously to gather results from all pages or until page limit is reached
        /// </summary>
        RunSynchronouslyToPageLimit
    }

    public enum MessageCallbackType
    {
        /// <summary>
        /// Simple text message to be displayed to the user
        /// </summary>
        SimpleMessage = 0,

        /// <summary>
        /// Error message that should involve an Exception object returned from the service
        /// </summary>
        ErrorMessage = 1,

        /// <summary>
        /// Message with the page data to show progress to the user
        /// </summary>
        PagedResultsMessage = 2
    }
}
