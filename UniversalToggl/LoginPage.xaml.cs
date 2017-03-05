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
using TogglAPI;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void Login_button_Click(object sender, RoutedEventArgs e)
        {
            string email = this.Email_box.Text;
            string password = this.Password_box.Password;
            try
            {
                App.user = await User.Logon(email, password);

                this.Frame.Navigate(typeof(MainPage));
            } catch (AuthenticationException)
            {
                this.ErrorMessage.Visibility = Visibility.Visible;
                this.ErrorMessage.Text = "Email or password is invalid";
                this.Password_box.Password = "";
                Connection.Reset();
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
                Password_box.PasswordRevealMode = PasswordRevealMode.Visible;
            else
                Password_box.PasswordRevealMode = PasswordRevealMode.Hidden;
        }

        /// <summary>
        /// When user hits enter in the password box, execute the login command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                Login_button_Click(this, new RoutedEventArgs());
        }
    }
}
