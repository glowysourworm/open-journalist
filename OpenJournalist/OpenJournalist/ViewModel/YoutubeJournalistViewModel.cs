using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using OpenJournalist.ViewModel.Local;
using OpenJournalist.ViewModel.SearchResult;
using OpenJournalist.ViewModel.Youtube;

using WpfCustomUtilities.Extensions;
using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.ViewModel
{
    public class YoutubeJournalistViewModel : ViewModelBase
    {
        ConfigurationViewModel _configuration;

        ChannelViewModel _selectedChannel;
        YoutubeCommentThreadViewModel _selectedCommentThread;

        // Platform parameters
        PlatformType _selectedPlatform;
        bool _selectedPlatformEnable;
        string _selectedPlatformBasicSearch;

        // Data collections
        ObservableCollection<ChannelViewModel> _channels;
        ObservableCollection<YoutubeSearchResultViewModel> _searchResults;
        ObservableCollection<YoutubeSearchResultViewModel> _localSearchResults;
        ObservableCollection<LogViewModel> _outputLog;

        public SimpleEventHandler<string> GetChannelDetailsEvent;

        public ConfigurationViewModel Configuration
        {
            get { return _configuration; }
            set { this.RaiseAndSetIfChanged(ref _configuration, value); }
        }
        public PlatformType SelectedPlatform
        {
            get { return _selectedPlatform; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlatform, value); }
        }
        public bool SelectedPlatformEnable
        {
            get { return _selectedPlatformEnable; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlatformEnable, value); }
        }
        public string SelectedPlatformBasicSearch
        {
            get { return _selectedPlatformBasicSearch; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlatformBasicSearch, value); }
        }
        public ChannelViewModel SelectedChannel
        {
            get { return _selectedChannel; }
            set { this.RaiseAndSetIfChanged(ref _selectedChannel, value); }
        }
        public YoutubeCommentThreadViewModel SelectedCommentThread
        {
            get { return _selectedCommentThread; }
            set { this.RaiseAndSetIfChanged(ref _selectedCommentThread, value); }
        }
        public ObservableCollection<ChannelViewModel> Channels
        {
            get { return _channels; }
            set { this.RaiseAndSetIfChanged(ref _channels, value); }
        }
        public ObservableCollection<YoutubeSearchResultViewModel> SearchResults
        {
            get { return _searchResults; }
            set { this.RaiseAndSetIfChanged(ref _searchResults, value); }
        }
        public ObservableCollection<YoutubeSearchResultViewModel> LocalSearchResults
        {
            get { return _localSearchResults; }
            set { this.RaiseAndSetIfChanged(ref _localSearchResults, value); }
        }
        public ObservableCollection<LogViewModel> OutputLog
        {
            get { return _outputLog; }
            set { this.RaiseAndSetIfChanged(ref _outputLog, value); }
        }

        public YoutubeJournalistViewModel(ConfigurationViewModel configuration)
        {
            this.Configuration = configuration;
            this.SelectedPlatform = PlatformType.LocalDB;
            this.SelectedPlatformEnable = true;
            this.SelectedPlatformBasicSearch = "";
            this.SearchResults = new ObservableCollection<YoutubeSearchResultViewModel>();
            this.LocalSearchResults = new ObservableCollection<YoutubeSearchResultViewModel>();
            this.OutputLog = new ObservableCollection<LogViewModel>();
            this.Channels = new ObservableCollection<ChannelViewModel>();

            this.SearchResults.CollectionChanged += OnSearchCollectionChanged;
        }

        private void OnSearchCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Rehook events
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<YoutubeSearchResultViewModel>())
                {
                    //item.GetChannelDetailsEvent -= OnGetChannelDetails;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<YoutubeSearchResultViewModel>())
                {
                    //item.GetChannelDetailsEvent += OnGetChannelDetails;
                }
            }
        }
        private void OnGetChannelDetails(string channelId)
        {
            if (this.GetChannelDetailsEvent != null)
                this.GetChannelDetailsEvent(channelId);
        }
    }
}
