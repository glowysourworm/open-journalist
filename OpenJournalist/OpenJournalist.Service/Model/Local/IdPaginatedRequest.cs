
using OpenJournalist.Service.Model.Message;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.Service.Model.Local
{
    /// <summary>
    /// Uses Id field to narrow search results. Return set would still be paginated
    /// </summary>
    public class IdPaginatedRequest<T> : PaginatedRequest
    {
        public T Id { get; private set; }

        public IdPaginatedRequest(IdPaginatedRequest<T> previousPageRequest)
            : base(previousPageRequest.CurrentPageIndex,
                   previousPageRequest.PagingBehavior,
                   previousPageRequest.MessageHandler,
                   previousPageRequest.PageSize)
        {
            this.Id = previousPageRequest.Id;
        }

        public IdPaginatedRequest(T id,
                                  PagingBehavior pagingBehavior,
                                  SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                  int pageSize)
            : base(0, pagingBehavior, messageHandler, pageSize)
        {
            this.Id = id;
        }

        public IdPaginatedRequest(T id,
                                  int currentPageIndex,
                                  PagingBehavior pagingBehavior,
                                  SimpleEventHandler<MessagingCallbackEventArgsBase> messageHandler,
                                  int pageSize)
            : base(currentPageIndex, pagingBehavior, messageHandler, pageSize)
        {
            this.Id = id;
        }
    }
}
