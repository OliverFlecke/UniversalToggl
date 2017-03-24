using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.Dialogs;
using UniversalToggl.View.Model;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;
using Windows.System;

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        public TimeEntryViewModel RunningTimeEntry { get { return App.data.RunningTimeEntry; } }

        public ObservableCollection<TimeEntry> TimeEntries { get { return App.data.TimeEntries; } }
        public ObservableCollection<Workspace> Workspaces { get { return App.data.Workspaces; } }
        public ObservableCollection<Project> Projects { get { return App.data.Projects; } }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            RunningTimeEntry.TimeDisplay = this.TimeDisplay;

            LoginAndUpdateData();
        }

        /// <summary>
        /// Make sure the loggin is valid, and update the data if needed
        /// </summary>
        public async void LoginAndUpdateData()
        {
            if (App.user == null)
            {
                LoginDialog loginDialog = new LoginDialog();
                string username = App.localStorage.Values["username"] as string;
                if (username == null)
                {
                    await loginDialog.ShowAsync();
                }
                else
                {
                    try
                    {
                        var credentials = App.vault.Retrieve(App.AppName, username);
                        loginDialog.LoginWithCredentials(credentials);
                    }
                    catch (Exception)
                    {
                        await loginDialog.ShowAsync();
                    }
                }
            }

            UpdateRunningTimeEntry();
            if (!TimeEntries.Any())
                App.data.Synchronice();
        }

        /// <summary>
        /// Update the running time entry 
        /// </summary>
        public async void UpdateRunningTimeEntry()
        {
            // Get the running entry
            var runningEntry = await TimeEntry.GetRunningTimeEntry();
            if (runningEntry != null)
            {
                this.RunningTimeEntry.Entry = runningEntry;
                this.RunningTimeEntryDisplay.Visibility = Visibility.Visible;
            }
            else
                this.RunningTimeEntryDisplay.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Start the time entry entered into the flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            string description = this.TimeEntryDescriptionBox.Text;
            string projectName = this.TimeEntryProjectBox.Text;

            try
            {
                // If project name is non-empty, the project must already exists
                if (projectName != string.Empty)
                    Projects.First(x => x.Name.ToLower() == projectName.ToLower());
                this.ProjectBoxErrorMessage.Visibility = Visibility.Collapsed;
                StartTimeEntry(description, projectName);

                // Clear the flyout 
                AddButton.Flyout.Hide();
                TimeEntryDescriptionBox.Text = string.Empty;
                TimeEntryProjectBox.Text = string.Empty;
            }
            catch (Exception)
            {
                this.ProjectBoxErrorMessage.Text = "Invalid project";
                this.ProjectBoxErrorMessage.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Start a new time entry 
        /// </summary>
        /// <param name="description">The description of the time entry</param>
        /// <param name="projectName"></param>
        private async void StartTimeEntry(string description, string projectName)
        {
            TimeEntry entry;
            try
            {
                Project project = Projects.First(x => x.Name == projectName);
                entry = await TimeEntry.StartTimeEntry(description, project.ID);
                entry.ProjectName = projectName;
            } 
            catch (Exception)
            {
                entry = await TimeEntry.StartTimeEntry(description);
            }
            this.RunningTimeEntry.Entry = entry;
            this.RunningTimeEntryDisplay.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Start a time entry with the same project and description as the argument
        /// </summary>
        /// <param name="entry">Entry to start a copy of</param>
        private void StartTimeEntry(TimeEntry entry)
        {
            this.StartTimeEntry(entry.Description, entry.ProjectName);
        }

        /// <summary>
        /// Event handler for the stop button. Stops the running time entry and addes it to the list of entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StopRunningTimeEntryButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            TimeEntry entry = await TimeEntry.StopTimeEntry(this.RunningTimeEntry.Entry.Id);
            try
            {
                Project project = Projects.First<Project>(x => x.ID == entry.ProjectId);
                entry.ProjectName = project.Name;
            }
            catch (Exception) { }
            
            TimeEntries.Insert(0, entry);

            // Hid the running entry panel
            this.RunningTimeEntryDisplay.Visibility = Visibility.Collapsed;
            RunningTimeEntry.Entry = null;
        }

        /// <summary>
        /// Update the auto suggest button in the flyout to add a new task
        /// </summary>
        /// <param name="projectNameBox"></param>
        /// <param name="args"></param>
        private void TimeEntryProjectBox_TextChanged(AutoSuggestBox projectNameBox, AutoSuggestBoxTextChangedEventArgs args)
        {
            projectNameBox.ItemsSource = Projects.Where(x => x.Name.ToLower().StartsWith(projectNameBox.Text.ToLower())).Distinct(new ProjectNameComparer()).DefaultIfEmpty(new Project() { Name = "No results" });
        }

        /// <summary>
        /// Makes the suggestion box pop up when the project box gets in focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEntryProjectBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.TimeEntryProjectBox_TextChanged(sender as AutoSuggestBox, new AutoSuggestBoxTextChangedEventArgs());
        }

        private void TimeEntryProjectBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Project selected = args.SelectedItem as Project;
            sender.Text = selected.Name;
        }

        private void TimeEntryDescriptionBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                StartTimeEntryButton_Click(sender, new RoutedEventArgs());
        }

        private void CancelTimeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            this.AddButton.Flyout.Hide();
        }


        /// <summary>
        /// Event handler for the description autosuggestion box for adding new entries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TimeEntryDescriptionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            sender.ItemsSource = TimeEntries.Where(x => x.Description.ToLower().StartsWith(sender.Text.ToLower())).Distinct(new DescriptionComparer());
        }

        private void TimeEntryDescriptionBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var seleted = args.SelectedItem as TimeEntry;
            sender.Text = seleted.Description;
        }

        /// <summary>
        /// Event listener for the play button on each of the time entry items in the main view.
        /// </summary>
        /// <param name="sender">The icon which has been pressed, along with the seleted item</param>
        /// <param name="e"></param>
        private void PlayButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.StartTimeEntry((e.OriginalSource as TextBlock).DataContext as TimeEntry);
        }
    }
}
