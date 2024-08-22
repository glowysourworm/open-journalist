using System.Collections.ObjectModel;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel.Youtube
{
    public class YoutubeCommentThreadViewModel : ViewModelBase
    {
        private YoutubeCommentViewModel _comment;
        private ObservableCollection<YoutubeCommentViewModel> _replies;
        private bool _isPublic;
        private int _totalReplyCount;
        private string _videoId;
        private string _channelId;

        public YoutubeCommentViewModel Comment
        {
            get { return _comment; }
            set { this.RaiseAndSetIfChanged(ref _comment, value); }
        }
        public ObservableCollection<YoutubeCommentViewModel> Replies
        {
            get { return _replies; }
            set { this.RaiseAndSetIfChanged(ref _replies, value); }
        }
        public string VideoId
        {
            get { return _videoId; }
            set { this.RaiseAndSetIfChanged(ref _videoId, value); }
        }
        public string ChannelId
        {
            get { return _channelId; }
            set { this.RaiseAndSetIfChanged(ref _channelId, value); }
        }
        public bool IsPublic
        {
            get { return _isPublic; }
            set { this.RaiseAndSetIfChanged(ref _isPublic, value); }
        }
        public int TotalReplyCount
        {
            get { return _totalReplyCount; }
            set { this.RaiseAndSetIfChanged(ref _totalReplyCount, value); }
        }

        public YoutubeCommentThreadViewModel()
        {
            this.Comment = new YoutubeCommentViewModel();
            this.Replies = new ObservableCollection<YoutubeCommentViewModel>();
        }
    }
}
