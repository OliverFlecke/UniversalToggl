﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                RootFrame.Navigate(pageSourceType);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }
    }
}
