using System;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel.Local
{
    public class ChannelViewModel : ViewModelBase
    {
        int _id;
        string _platform;
        string _title;
        string _description;
        string _iconUrl;
        DateTime _capturedDateTime;

        public int Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string Platform
        {
            get { return _platform; }
            set { this.RaiseAndSetIfChanged(ref _platform, value); }
        }
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }
        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }
        public string IconUrl
        {
            get { return _iconUrl; }
            set { this.RaiseAndSetIfChanged(ref _iconUrl, value); }
        }
        public DateTime CapturedDateTime
        {
            get { return _capturedDateTime; }
            set { this.RaiseAndSetIfChanged(ref _capturedDateTime, value); }
        }

        public ChannelViewModel()
        {

        }
    }
}
