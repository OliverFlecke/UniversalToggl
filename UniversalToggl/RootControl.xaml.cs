using System;
using UniversalToggl.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalToggl
{
    public sealed partial class RootControl : UserControl
    {
        private Type currentPage;

        public RootControl()
        {
            this.InitializeComponent();
            RootFrame.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            currentPage = e.SourcePageType;
        }

        public Frame RootFrame
        {
            get
            {
                return rootFrame;
            }
        }

        private void OnHomeClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Navigate(typeof(MainPage));
        }


        private void Navigate(Type pageSourceType)
        {
            if (currentPage != pageSourceType)
            {
                splitView.IsPaneOpen = false;
                RootFrame.Navigate(pageSourceType);
            }
        }

        /// <summary>
        /// Open and close the pane manu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigate(typeof(MainPage));
        }

        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigate(typeof(SettingsPage));
        }

        private void ProjectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigate(typeof(ProjectPage));
        }

        private void WorkspacesButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigate(typeof(WorkspacePage));
        }

        private void TagsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigate(typeof(TagsPage));
        }

        /// <summary>
        /// Synchronice with the newest data from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.data.Synchronice();
        }
    }
}
