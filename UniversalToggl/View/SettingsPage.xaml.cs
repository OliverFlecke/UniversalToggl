using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
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
