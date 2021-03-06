﻿using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.Dialogs;
using UniversalToggl.View.Model;
using Windows.UI.Xaml.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        public TimeEntryViewModel RunningTimeEntry { get { return App.Data.RunningTimeEntry; } }

        private static ObservableCollection<TimeEntryByDateViewModel> timeEntries = new ObservableCollection<TimeEntryByDateViewModel>();
        public ObservableCollection<TimeEntryByDateViewModel> TimeEntries
        {
            get { return timeEntries; }
            set { timeEntries = value; }
        }
        //public ObservableCollection<TimeEntry> TimeEntries { get { return App.data.TimeEntries; } }
        public ObservableCollection<Workspace> Workspaces { get { return App.Data.Workspaces; } }
        public ObservableCollection<Project> Projects { get { return App.Data.Projects; } }
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

            await UpdateEntries();
        }

        /// <summary>
        /// Update the entries on the page
        /// </summary>
        /// <returns></returns>
        private async Task UpdateEntries()
        {
            UpdateRunningTimeEntry();
            if (!App.Data.TimeEntries.Any())
                await App.Data.Synchronice();

            TimeEntries.Clear();
            foreach (TimeEntry entry in App.Data.TimeEntries)
            {
                TimeEntryByDateViewModel model = TimeEntries.Where(x => x.Date == entry.Start.Date).DefaultIfEmpty(new TimeEntryByDateViewModel(entry.Start.Date)).First();
                model.Entries.Add(entry);
                if (!TimeEntries.Contains(model))
                    TimeEntries.Add(model);
            }
            TimeEntries.OrderBy(x => x.Date);
        }

        /// <summary>
        /// Update the running time entry 
        /// </summary>
        public async void UpdateRunningTimeEntry()
        {
            // Get the running entry
            TimeEntry runningEntry = null;
            try
            {
                runningEntry = await TimeEntry.GetRunningTimeEntry();
            }
            catch (Exception) { }

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
                Project project = App.Data.Projects.First(x => x.Name == projectName);
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

        /// <summary>
        /// Create a new time entry with a predefined start and end time
        /// </summary>
        /// <param name="description"></param>
        /// <param name="projectName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="tags"></param>
        internal async void CreateTimeEntry(string description, string projectName, DateTime start, DateTime end, List<Tag> tags = null)
        {
            TimeEntry entry;
            try
            {
                int duration = (int)(end - start).TotalSeconds;
                Project project = null;
                try
                {
                    project = App.Data.Projects.First(x => x.Name == projectName);
                    entry = await TimeEntry.CreateTimeEntry(description, start, duration, project.ID);
                }
                catch (Exception)
                {
                    entry = await TimeEntry.CreateTimeEntry(description, start, duration);
                }

                entry.ProjectName = projectName;
                AddTimeEntry(entry);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Add a time entry to the data shared in the app.
        /// </summary>
        /// <param name="entry">The app to add</param>
        private void AddTimeEntry(TimeEntry entry)
        {
            App.Data.TimeEntries.Add(entry);
            var viewModel = this.TimeEntries.Where(x => x.Date == entry.Start.Date)
                .DefaultIfEmpty(new TimeEntryByDateViewModel(entry.Start.Date))
                .First();
            int index = 0;
            while (index < viewModel.Entries.Count && viewModel.Entries.ElementAt(index).Start > entry.Start)
            {
                index++;
            }
            viewModel.Entries.Insert(index, entry);
            if (!this.TimeEntries.Contains(viewModel))
            {
                this.TimeEntries.Insert(0, viewModel);
                this.TimeEntries.OrderByDescending(x => x.Date);
            }
            else
                viewModel.OnPropertyChanged("Entries");
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
                Project project = Projects.First(x => x.ID == entry.ProjectId);
                entry.ProjectName = project.Name;
            }
            catch (Exception) { }

            AddTimeEntry(entry);
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

        /// <summary>
        /// Event handler when selecting items in the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEntryContent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Navigate to some site to update the time entry
            var frame = Window.Current.Content as RootControl;
            frame.RootFrame.Navigate(typeof(TimeEntryDetailsPage), (sender as StackPanel).DataContext);
        }

        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AddTimeEntryPopup.IsOpen = true;
        }

        /// <summary>
        /// Collapse and expand each item in the listview when the item is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEntryListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var list = sender as ListView;
            ListViewItem item = (list.ContainerFromIndex(list.SelectedIndex)) as ListViewItem;

            var model = list.SelectedItem as TimeEntryByDateViewModel;
            if (model != null)
            {
                if (model.IsVisible == Visibility.Visible)
                {
                    model.IsVisible = Visibility.Collapsed;
                    item.ContentTemplate = Resources["CollapsedItem"] as DataTemplate;

                }
                else
                {
                    model.IsVisible = Visibility.Visible;
                    item.ContentTemplate = Resources["ExpandedItem"] as DataTemplate;
                }
            }
            if (item != null)
                item.IsSelected = false;
        }

    }
}
