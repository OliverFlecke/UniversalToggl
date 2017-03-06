using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TogglAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.Security.Credentials;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalToggl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        private ObservableCollection<TimeEntry> timeEntries = new ObservableCollection<TimeEntry>();
        public ObservableCollection<TimeEntry> TimeEntries { get { return timeEntries; } }

        private ObservableCollection<Workspace> workspaces = new ObservableCollection<Workspace>();

        public ObservableCollection<Workspace> Workspaces
        {
            get { return workspaces; }
            set { workspaces = value; }
        }


        private ObservableCollection<Project> projects = new ObservableCollection<Project>();

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
            UpdateTimeEntries();
        }

        public async void Setup()
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

        public async void UpdateTimeEntries()
        {


            var workspaces = await Workspace.GetWorkspaces();
            foreach (Workspace ws in workspaces)
            {
                this.workspaces.Add(ws);
            }

            Workspace workspace = workspaces[0];
            var projects = await Workspace.GetWorkspaceProjects(workspace.Id);
            foreach (Project project in projects)
            {
                this.projects.Add(project);
            }

            var entries = await TimeEntry.GetTimeEntriesInRange();
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


        }
    }
}
