
using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel
{
    public class LogViewModel : ViewModelBase
    {
        private string _log;
        private bool _isError;

        public string Log
        {
            get { return _log; }
            set { this.RaiseAndSetIfChanged(ref _log, value); }
        }
        public bool IsError
        {
            get { return _isError; }
            set { this.RaiseAndSetIfChanged(ref _isError, value); }
        }

        public LogViewModel()
        {
            this.Log = "";
            this.IsError = false;
        }
    }
}
