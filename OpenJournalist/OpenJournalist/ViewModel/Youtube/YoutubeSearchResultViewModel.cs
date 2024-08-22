using System;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel.SearchResult
{
    public class YoutubeSearchResultViewModel : ViewModelBase
    {
        string _thumbnail;
        string _channelId;
        string _title;
        string _description;
        DateTime _created;

        public string Thumbnail
        {
            get { return _thumbnail; }
            set { this.RaiseAndSetIfChanged(ref _thumbnail, value); }
        }
        public string ChannelId
        {
            get { return _channelId; }
            set { this.RaiseAndSetIfChanged(ref _channelId, value); }
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
        public DateTime Created
        {
            get { return _created; }
            set { this.RaiseAndSetIfChanged(ref _created, value); }
        }

        public YoutubeSearchResultViewModel()
        {
            this.Thumbnail = "";
            this.ChannelId = "";
            this.Title = "";
            this.Description = "";
            this.Created = DateTime.Now;
        }
    }
}
