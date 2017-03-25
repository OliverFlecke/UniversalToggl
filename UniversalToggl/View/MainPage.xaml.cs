using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.Dialogs;
using UniversalToggl.View.Model;
using Windows.UI.Xaml.Input;
using Windows.System;
using System.Collections.Generic;

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
        /// Start a new time entry 
        /// </summary>
        /// <param name="description">The description of the time entry</param>
        /// <param name="projectName"></param>
        public async void StartTimeEntry(string description, string projectName, List<Tag> tags = null)
        {
            if (tags == null) tags = new List<Tag>();
            TimeEntry entry;
            try
            {
                Project project = App.data.Projects.First(x => x.Name == projectName);
                entry = await TimeEntry.StartTimeEntry(description, project.ID, tags.ToArray());
                entry.ProjectName = projectName;
            } 
            catch (Exception)
            {
                entry = await TimeEntry.StartTimeEntry(description, tags: tags.ToArray());
            }
            this.RunningTimeEntry.Entry = entry;
            this.RunningTimeEntryDisplay.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Start a time entry with the same project and description as the argument
        /// </summary>
        /// <param name="entry">Entry to start a copy of</param>
        public void StartTimeEntry(TimeEntry entry)
        {
            if (entry.Tags == null)
                StartTimeEntry(entry.Description, entry.ProjectName);
            else
            {
                var tags = entry.Tags.Select(x => new Tag(x)).ToList();
                this.StartTimeEntry(entry.Description, entry.ProjectName, tags);
            }
        }

        internal async void CreateTimeEntry(string description, string projectName, DateTime start, DateTime end, List<Tag> tags = null)
        {
            TimeEntry entry;
            try
            {
                Project project = App.data.Projects.First(x => x.Name == projectName);
                int duration = (int) (end - start).TotalSeconds;
                entry = await TimeEntry.CreateTimeEntry(description, start, duration, project.ID);

                entry.ProjectName = project.Name;
                App.data.TimeEntries.Add(entry);
            } catch (Exception) { }
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
        /// Event listener for the play button on each of the time entry items in the main view.
        /// </summary>
        /// <param name="sender">The icon which has been pressed, along with the seleted item</param>
        /// <param name="e"></param>
        private void PlayButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StartTimeEntry((sender as Grid).DataContext as TimeEntry);
        }

        private void TimeEntryContent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Navigate to some site to update the time entry
        }

        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AddTimeEntryPopup.IsOpen = true;
        }
    }
}
