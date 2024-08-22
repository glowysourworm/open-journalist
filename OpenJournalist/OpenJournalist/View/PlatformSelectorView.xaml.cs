using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using OpenJournalist.ViewModel;

namespace OpenJournalist.View
{
    public partial class PlatformSelectorView : UserControl
    {
        public PlatformSelectorView()
        {
            InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
            this.Loaded += PlatformSelectorView_Loaded;
        }

        private void PlatformSelectorView_Loaded(object sender, RoutedEventArgs e)
        {
            this.PlatformCB.SelectedIndex = 0;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldContext = e.OldValue as YoutubeJournalistViewModel;
            var newContext = e.NewValue as YoutubeJournalistViewModel;

            if (oldContext != null)
            {
                oldContext.PropertyChanged -= OnPlatformValueChanged;
            }
            if (newContext != null)
            {
                newContext.PropertyChanged += OnPlatformValueChanged;
            }
        }

        private void OnPlatformValueChanged(object sender, PropertyChangedEventArgs e)
        {
            var dataContext = this.DataContext as YoutubeJournalistViewModel;

            // Initialization
            if (dataContext == null)
                return;

            if (e.PropertyName == "SelectedPlatform")
            {
                switch (dataContext.SelectedPlatform)
                {
                    case PlatformType.LocalDB:
                        this.PlatformCB.SelectedIndex = 0;
                        break;
                    case PlatformType.Youtube:
                        this.PlatformCB.SelectedIndex = 1;
                        break;
                    case PlatformType.Rumble:
                        this.PlatformCB.SelectedIndex = 2;
                        break;
                    default:
                        break;
                }
            }
        }

        private void PlatformCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = this.DataContext as YoutubeJournalistViewModel;

            // Initialization
            if (dataContext == null)
                return;

            var selectedIndex = this.PlatformCB.SelectedIndex;

            switch (selectedIndex)
            {
                case 0:
                    dataContext.SelectedPlatform = PlatformType.LocalDB;
                    break;
                case 1:
                    dataContext.SelectedPlatform = PlatformType.Youtube;
                    break;
                case 2:
                    dataContext.SelectedPlatform = PlatformType.Rumble;
                    break;
                default:
                    throw new Exception("Unhandled Platform:  PlatformSelectorView.PlatformCB_EnumValueChanged");
            }
        }
    }
}
