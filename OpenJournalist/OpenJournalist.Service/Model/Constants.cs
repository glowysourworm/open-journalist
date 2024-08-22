namespace OpenJournalist.Service.Model
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
}
