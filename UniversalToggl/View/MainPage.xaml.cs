using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.Dialogs;
using UniversalToggl.View.Model;

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        private TimeEntryViewModel runningTimeEntry = new TimeEntryViewModel();

        public TimeEntryViewModel RunningTimeEntry
        {
            get { return runningTimeEntry; }
            set { runningTimeEntry = value; }
        }


        private static ObservableCollection<TimeEntry> timeEntries;
        public ObservableCollection<TimeEntry> TimeEntries { get { return timeEntries; } }

        private static ObservableCollection<Workspace> workspaces;
        public ObservableCollection<Workspace> Workspaces
        {
            get { return workspaces; }
            set { workspaces = value; }
        }

        private static ObservableCollection<Project> projects;
        public ObservableCollection<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            Setup();
        }

        public async void Setup()
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
                    var credentials = App.vault.Retrieve(App.AppName, username);
                    loginDialog.LoginWithCredentials(credentials);
                }
            }

            UpdateTimeEntries();
        }

        public async void UpdateTimeEntries()
        {
            // Rest the list of content
            workspaces = new ObservableCollection<Workspace>();
            projects = new ObservableCollection<Project>();
            timeEntries = new ObservableCollection<TimeEntry>();

            var spaces = await Workspace.GetWorkspaces();
            foreach (Workspace workspace in spaces)
            {
                workspaces.Add(workspace);

                var ps = await Workspace.GetWorkspaceProjects(workspace.Id);
                foreach (Project project in ps)
                    projects.Add(project);
            }

            var entries = await TimeEntry.GetTimeEntriesInRange();
            entries.Reverse();
            foreach (TimeEntry entry in entries)
            {
                // TODO Find a cleaner way to do this
                foreach (Project project in projects)
                {
                    if (project.ProjectID == entry.ProjectId)
                        entry.ProjectName = project.Name;
                }
                timeEntries.Add(entry);
            }
            this.RunningTimeEntry.Entry = await TimeEntry.GetRunningTimeEntry();
        }

        /// <summary>
        /// Start the time entry entered into the flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            string description = this.TimeEntryDescriptionBox.Text;
            string project = this.TimeEntryProjectBox.Text;

            StartTimeEntry(description, project);
        }

        /// <summary>
        /// Start a new time entry 
        /// </summary>
        /// <param name="description">The description of the time entry</param>
        /// <param name="project"></param>
        private async void StartTimeEntry(string description, string project)
        {
            TimeEntry entry = await TimeEntry.StartTimeEntry(description);
            this.RunningTimeEntry.Entry = entry;
        }
    }
}
