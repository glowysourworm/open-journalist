using System.Windows;
using System.Windows.Controls;

using WpfCustomUtilities.Extensions.Event;

namespace OpenJournalist.View
{
    /// <summary>
    /// Interaction logic for YoutubeSearchView.xaml
    /// </summary>
    public partial class YoutubeSearchView : UserControl
    {
        public event SimpleEventHandler<string> YoutubeBasicSearchEvent;

        private const string MessageBoxTitle = "Youtube Service Request";
        private const string SearchWarning = "The following search {0} will be charged to your Youtube account and cost 100 'quota'. Do you want to proceed?";

        public YoutubeSearchView()
        {
            InitializeComponent();
        }

        private void OnYoutubeResultDoubleClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnYoutubeLocalResultDoubleClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnYoutubeSearchButton(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.YoutubeSearchTB.Text))
            {
                if (MessageBox.Show(string.Format(SearchWarning, this.YoutubeSearchTB.Text), 
                                    MessageBoxTitle, 
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (this.YoutubeBasicSearchEvent != null)
                        this.YoutubeBasicSearchEvent(this.YoutubeSearchTB.Text);
                }
            }

            else
                MessageBox.Show("Please enter search text", MessageBoxTitle);
        }
    }
}
