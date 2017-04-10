using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Security.Credentials;
using TogglAPI;
using System;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.Dialogs
{
    public sealed partial class LoginDialog : ContentDialog
    {
        public LoginDialog()
        {
            this.InitializeComponent();
        }

        private void Login_button_Click(object sender, ContentDialogButtonClickEventArgs e)
        {
            Login();
        }

        /// <summary>
        /// Login with the details provided
        /// </summary>
        private async void Login(string email, string password)
        {
            try
            {
                try
                {
                    App.user = await User.Logon(email, password);
                } 
                catch (AuthenticationException)
                {
                    this.ErrorMessage.Visibility = Visibility.Visible;
                    this.ErrorMessage.Text = "Email or password is invalid";
                    this.Password_box.Password = "";
                    Connection.Reset();
                    return;
                }
            }
            catch (Exception)
            {
                User.SetupConnection(email, password);
            }

            App.localStorage.Values["username"] = email;
            App.vault.Add(new PasswordCredential(App.AppName, email, password));
            this.Hide();
        }

        private void Login()
        {
            Login(this.Email_box.Text, this.Password_box.Password);
        }

        /// <summary>
        /// Login with a Password Credential object
        /// </summary>
        /// <param name="credentials">The credentials to login</param>
        public void LoginWithCredentials(PasswordCredential credentials)
        {
            Login(credentials.UserName, credentials.Password);
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
                Login();

            if (Password_box.Password == "")
                this.IsPrimaryButtonEnabled = false;
            else
                this.IsPrimaryButtonEnabled = true;
        }
    }
}
