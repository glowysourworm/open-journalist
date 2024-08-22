using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WpfCustomUtilities.Extensions.Event;

using OpenJournalist.Event;
using OpenJournalist.ViewModel;
using OpenJournalist.ViewModel.SearchResult;

namespace OpenJournalist.View
{
    public partial class SearchResultView : UserControl
    {
        public static readonly RoutedEvent SearchResultDoubleClickEvent =
            EventManager.RegisterRoutedEvent("SearchResultDoubleClick",
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(SearchResultView));

        public event RoutedEventHandler SearchResultDoubleClick
        {
            add { AddHandler(SearchResultDoubleClickEvent, value); }
            remove { RemoveHandler(SearchResultDoubleClickEvent, value); }
        }

        public SearchResultView()
        {
            InitializeComponent();
        }

        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (sender as ListBoxItem).DataContext as YoutubeSearchResultViewModel;

            RaiseEvent(new CustomRoutedEventArgs<YoutubeSearchResultViewModel>(SearchResultDoubleClickEvent, viewModel));
        }
    }
}
