using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TogglAPI;
using UniversalToggl.View;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalToggl.Dialogs
{
    public sealed partial class NewTimeEntryDialog : UserControl
    {
        public NewTimeEntryDialog()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Update the auto suggest button in the flyout to add a new task
        /// </summary>
        /// <param name="projectNameBox"></param>
        /// <param name="args"></param>
        private void TimeEntryProjectBox_TextChanged(AutoSuggestBox projectNameBox, AutoSuggestBoxTextChangedEventArgs args)
        {
            projectNameBox.ItemsSource = App.Data.Projects.Where(x => x.Name.ToLower().StartsWith(projectNameBox.Text.ToLower())).Distinct(new ProjectNameComparer()).DefaultIfEmpty(new Project() { Name = "No results" });
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

            // Get a reference to the main page that created this popup
            MainPage mainpage = (((this.dialog.Parent as NewTimeEntryDialog).Parent as Popup).Parent as Grid).Parent as MainPage;
            try
            {
                // If project name is non-empty, the project must already exists
                if (projectName != string.Empty)
                    App.Data.Projects.First(x => x.Name.ToLower() == projectName.ToLower());
                this.ProjectBoxErrorMessage.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.ProjectBoxErrorMessage.Text = "Invalid project";
                this.ProjectBoxErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // If there is dates, create it with the date information
            if (DateBoxes.Visibility == Visibility.Visible)
            {
                DateTime start = StartDate.Date.Date.Add(StartTime.Time);
                DateTime end = EndDate.Date.Date.Add(EndTime.Time);
                mainpage.CreateTimeEntry(description, projectName, start, end);
            }
            else
            {
                mainpage.StartTimeEntry(description, projectName);
            }

            // Clear the flyout 
            TimeEntryDescriptionBox.Text = string.Empty;
            TimeEntryProjectBox.Text = string.Empty;
            StartDate.Date = DateTimeOffset.Now;
            EndDate.Date = DateTimeOffset.Now;
            StartTime.Time = DateTime.Now.TimeOfDay;
            EndTime.Time = DateTime.Now.TimeOfDay;
            (this.Parent as Popup).IsOpen = false;
        }

        /// <summary>
        /// Close the popup 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelTimeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Popup).IsOpen = false;
        }

        /// <summary>
        /// Event handler for the description autosuggestion box for adding new entries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TimeEntryDescriptionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            sender.ItemsSource = App.Data.TimeEntries.Where(x => x.Description.ToLower().StartsWith(sender.Text.ToLower())).Distinct(new DescriptionComparer());
        }

        private void TimeEntryDescriptionBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var seleted = args.SelectedItem as TimeEntry;
            sender.Text = seleted.Description;
        }

        /// <summary>
        /// Show more details to add to new tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoreButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (DateBoxes.Visibility == Visibility.Collapsed)
            {
                MoreButton.Text = "Less";
                DateBoxes.Visibility = Visibility.Visible;
            }
            else
            {
                MoreButton.Text = "More";
                DateBoxes.Visibility = Visibility.Collapsed;
            }
        }
    }
}
