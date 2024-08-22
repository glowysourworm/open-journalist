using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenJournalist.View
{
    public partial class ChannelView : UserControl
    {
        public event EventHandler SelectedChannelRefreshEvent;

        public ChannelView()
        {
            InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedChannelRefreshEvent != null)
                this.SelectedChannelRefreshEvent(sender, new EventArgs());
        }
    }
}
