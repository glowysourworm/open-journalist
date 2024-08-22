using System;
using System.Collections.ObjectModel;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel.Youtube
{
    public class YoutubeVideoViewModel : ViewModelBase
    {
        private ObservableCollection<YoutubeCommentThreadViewModel> _commentThreads;

        string _id;
        string _thumbnailUrl;
        string _title;
        string _description;
        string _categoryId;
        string _rejectionReason;
        string _uploadStatus;
        bool _isMonetized;
        bool _madeForKids;
        bool _selfDeclaredMadeForKids;
        DateTime _published;
        long _commentCount;
        long _likeCount;
        long _dislikeCount;
        long _favoriteCount;
        long _viewCount;

        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailUrl, value); }
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
        public string CategoryId
        {
            get { return _categoryId; }
            set { this.RaiseAndSetIfChanged(ref _categoryId, value); }
        }
        public string RejectionReason
        {
            get { return _rejectionReason; }
            set { this.RaiseAndSetIfChanged(ref _rejectionReason, value); }
        }
        public string UploadStatus
        {
            get { return _uploadStatus; }
            set { this.RaiseAndSetIfChanged(ref _uploadStatus, value); }
        }
        public bool IsMonetized
        {
            get { return _isMonetized; }
            set { this.RaiseAndSetIfChanged(ref _isMonetized, value); }
        }
        public bool MadeForKids
        {
            get { return _madeForKids; }
            set { this.RaiseAndSetIfChanged(ref _madeForKids, value); }
        }
        public bool SelfDeclaredMadeForKids
        {
            get { return _selfDeclaredMadeForKids; }
            set { this.RaiseAndSetIfChanged(ref _selfDeclaredMadeForKids, value); }
        }
        public DateTime Published
        {
            get { return _published; }
            set { this.RaiseAndSetIfChanged(ref _published, value); }
        }
        public long CommentCount
        {
            get { return _commentCount; }
            set { this.RaiseAndSetIfChanged(ref _commentCount, value); }
        }
        public long LikeCount
        {
            get { return _likeCount; }
            set { this.RaiseAndSetIfChanged(ref _likeCount, value); }
        }
        public long DislikeCount
        {
            get { return _dislikeCount; }
            set { this.RaiseAndSetIfChanged(ref _dislikeCount, value); }
        }
        public long FavoriteCount
        {
            get { return _favoriteCount; }
            set { this.RaiseAndSetIfChanged(ref _favoriteCount, value); }
        }
        public long ViewCount
        {
            get { return _viewCount; }
            set { this.RaiseAndSetIfChanged(ref _viewCount, value); }
        }


        public ObservableCollection<YoutubeCommentThreadViewModel> CommentThreads
        {
            get { return _commentThreads; }
            set { this.RaiseAndSetIfChanged(ref _commentThreads, value); }
        }

        public YoutubeVideoViewModel()
        {
            this.CommentThreads = new ObservableCollection<YoutubeCommentThreadViewModel>();
        }
    }
}
