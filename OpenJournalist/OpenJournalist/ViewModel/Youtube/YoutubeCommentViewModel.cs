using System;
using System.Windows.Input;

using WpfCustomUtilities.Extensions;
using WpfCustomUtilities.Extensions.Command;

using OpenJournalist.Utility;

namespace OpenJournalist.ViewModel.Youtube
{
    public class YoutubeCommentViewModel : ViewModelBase
    {
        ICommand _hyperlinkCommand;

        string _authorChannelId;
        string _authorUrl;
        string _authorDisplayName;
        string _authorImageUrl;
        long _likeCount;
        string _moderationStatus;
        DateTime _publishedDate;
        DateTime _updatedAtDate;
        string _display;

        public ICommand HyperlinkCommand
        {
            get { return _hyperlinkCommand; }
            set { this.RaiseAndSetIfChanged(ref _hyperlinkCommand, value); }
        }
        public string AuthorChannelId
        {
            get { return _authorChannelId; }
            set { this.RaiseAndSetIfChanged(ref _authorChannelId, value); }
        }
        public string AuthorUrl
        {
            get { return _authorUrl; }
            set { this.RaiseAndSetIfChanged(ref _authorUrl, value); }
        }
        public string AuthorDisplayName
        {
            get { return _authorDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _authorDisplayName, value); }
        }
        public string AuthorImageUrl
        {
            get { return _authorImageUrl; }
            set { this.RaiseAndSetIfChanged(ref _authorImageUrl, value); }
        }
        public long LikeCount
        {
            get { return _likeCount; }
            set { this.RaiseAndSetIfChanged(ref _likeCount, value); }
        }
        public string ModerationStatus
        {
            get { return _moderationStatus; }
            set { this.RaiseAndSetIfChanged(ref _moderationStatus, value); }
        }
        public DateTime PublishedDate
        {
            get { return _publishedDate; }
            set { this.RaiseAndSetIfChanged(ref _publishedDate, value); }
        }
        public string Display
        {
            get { return _display; }
            set { this.RaiseAndSetIfChanged(ref _display, value); }
        }
        public DateTime UpdatedAtDate
        {
            get { return _updatedAtDate; }
            set { this.RaiseAndSetIfChanged(ref _updatedAtDate, value); }
        }

        public YoutubeCommentViewModel()
        {
            this.HyperlinkCommand = new SimpleCommand<string>(url => HyperlinkUtility.OpenUrl(url));
        }
    }
}
