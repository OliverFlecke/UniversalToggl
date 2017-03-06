using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void ClearLoginDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            string username = App.localStorage.Values["username"] as string;
            if (username != null)
            {
                App.vault.Remove(App.vault.Retrieve(App.AppName, username));
                App.localStorage.Values["username"] = null;
            }
            this.ClearedDetailsTextBlock.Text = "Your details have now been cleared";
            this.ClearedDetailsTextBlock.Visibility = Visibility.Visible;
        }
    }
}
