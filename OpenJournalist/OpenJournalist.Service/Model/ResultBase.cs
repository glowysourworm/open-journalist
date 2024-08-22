namespace OpenJournalist.Service.Model
{
    public abstract class ResultBase
    {
        public bool Success { get; private set; }

        public ResultBase(bool success)
        {
            this.Success = success;
        }
    }
}
