namespace OpenJournalist.Core.Service.Model
{
    public class SingleResult<T> : ResultBase
    {
        public T Item { get; private set; }

        public SingleResult(T item, bool success) : base(success)
        {
            this.Item = item;
        }
    }
}
