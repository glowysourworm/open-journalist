using System.Collections.ObjectModel;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel.Youtube
{
    public class YoutubeChannelViewModel : ViewModelBase
    {
        ObservableCollection<YoutubeVideoViewModel> _videos;

        string _id;
        string _owner;
        string _title;
        string _description;
        string _bannerUrl;
        string _iconUrl;
        long _subscriberCount;
        long _videoCount;
        long _viewCount;
        bool _madeForKids;
        string _privacyStatus;
        bool _selfDeclaredMadeForKids;

        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string Owner
        {
            get { return _owner; }
            set { this.RaiseAndSetIfChanged(ref _owner, value); }
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
        public string BannerUrl
        {
            get { return _bannerUrl; }
            set { this.RaiseAndSetIfChanged(ref _bannerUrl, value); }
        }
        public string IconUrl
        {
            get { return _iconUrl; }
            set { this.RaiseAndSetIfChanged(ref _iconUrl, value); }
        }
        public long SubscriberCount
        {
            get { return _subscriberCount; }
            set { this.RaiseAndSetIfChanged(ref _subscriberCount, value); }
        }
        public long VideoCount
        {
            get { return _videoCount; }
            set { this.RaiseAndSetIfChanged(ref _videoCount, value); }
        }
        public long ViewCount
        {
            get { return _viewCount; }
            set { this.RaiseAndSetIfChanged(ref _viewCount, value); }
        }
        public bool MadeForKids
        {
            get { return _madeForKids; }
            set { this.RaiseAndSetIfChanged(ref _madeForKids, value); }
        }
        public string PrivacyStatus
        {
            get { return _privacyStatus; }
            set { this.RaiseAndSetIfChanged(ref _privacyStatus, value); }
        }
        public bool SelfDeclaredMadeForKids
        {
            get { return _selfDeclaredMadeForKids; }
            set { this.RaiseAndSetIfChanged(ref _selfDeclaredMadeForKids, value); }
        }

        public ObservableCollection<YoutubeVideoViewModel> Videos
        {
            get { return _videos; }
            set { this.RaiseAndSetIfChanged(ref _videos, value); }
        }

        public YoutubeChannelViewModel()
        {
            this.Videos = new ObservableCollection<YoutubeVideoViewModel>();
        }
    }
}
