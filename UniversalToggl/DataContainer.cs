using System;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.View.Model;

namespace UniversalToggl
{
    public class DataContainer
    {
        private TimeEntryViewModel runningTimeEntry = new TimeEntryViewModel();
        public TimeEntryViewModel RunningTimeEntry { get { return runningTimeEntry; } }

        private ObservableCollection<TimeEntry> timeEntries = new ObservableCollection<TimeEntry>();
        public ObservableCollection<TimeEntry> TimeEntries { get { return timeEntries; } }

        private ObservableCollection<Workspace> workspaces = new ObservableCollection<Workspace>();
        public ObservableCollection<Workspace> Workspaces { get { return workspaces; } }

        private ObservableCollection<Project> projects = new ObservableCollection<Project>();
        public ObservableCollection<Project> Projects { get { return projects; } }

        /// <summary>
        /// Synchronice with the Toggl server
        /// 
        /// Note: at the moment, only data is recived from the server, as there is no offline tracking
        /// </summary>
        public async void Synchronice()
        {
            // Reset the list of content
            workspaces.Clear();
            projects.Clear();
            timeEntries.Clear();

            var spaces = await Workspace.GetWorkspaces();
            foreach (Workspace workspace in spaces)
            {
                workspaces.Add(workspace);
                var ps = await Workspace.GetWorkspaceProjects(workspace.Id);
                foreach (Project project in ps)
                    projects.Add(project);
            }

            var entries = await TimeEntry.GetTimeEntriesInRange();
            // If there is a running entry, make sure it does not show up in the list of time entries
            if (runningTimeEntry.Entry != null)
            {
                try
                {
                    var entry = entries.Find(x => x.Id == runningTimeEntry.Entry.Id);
                    entries.Remove(entry);
                }
                catch (Exception) { }
            }
            entries.Reverse();
            foreach (TimeEntry entry in entries)
            {
                // TODO Find a cleaner way to do this
                foreach (Project project in projects)
                {
                    if (project.ID == entry.ProjectId)
                        entry.ProjectName = project.Name;
                }
                timeEntries.Add(entry);
            }
        }
    }
}
