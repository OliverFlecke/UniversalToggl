using System;
using System.Linq;
using TogglAPI;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeEntryDetailsPage : Page
    {
        private TimeEntry entry;

        public TimeEntry Entry
        {
            get { return entry; }
            set { entry = value; }
        }

        public DateTimeOffset StartDate
        {
            get { return DateTime.SpecifyKind(Entry.Start, DateTimeKind.Local); ; }
            set { Entry.Start = value.DateTime; }
        }
        public DateTimeOffset EndDate
        {
            get { return DateTime.SpecifyKind(Entry.Stop, DateTimeKind.Local); }
            set { Entry.Stop = value.DateTime; }
        }


        public TimeEntryDetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Entry = e.Parameter as TimeEntry;
        }

        /// <summary>
        /// Event handler for when the update button is pressed. Sends the new data to the Toggl API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateEntryButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            entry.Description = DescriptionBox.Text;
            entry.Start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Time.Hours, StartTime.Time.Minutes, StartTime.Time.Seconds);
            entry.Stop = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Time.Hours, EndTime.Time.Minutes, EndTime.Time.Seconds);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            TimeEntry.UpdateEntry(entry);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Add a tag to the entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddTagButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string name = AddTagBox.Text;
            if (name != string.Empty)
            {
                this.Entry.Tags.Add(name);
                // TODO: Find a better way to update UI for tags, than refreshing the entire page
                Frame.Navigate(typeof(TimeEntryDetailsPage), this.Entry);
                await TimeEntry.UpdateEntry(this.Entry);
            }
            AddTagBox.Text = string.Empty;
        }

        /// <summary>
        /// When enter is presesd, the tag is added to the entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTagBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                AddTagButton_Tapped(sender, new TappedRoutedEventArgs());
            }
        }

        /// <summary>
        /// Delete the tag from the entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTagButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string content = (string) (sender as Button).DataContext;
            this.Entry.Tags.Remove(content);
        }

        /// <summary>
        /// Delete the entire entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteTimeEntryButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Data.TimeEntries.Remove(this.Entry);
            await TimeEntry.DeleteEntry(this.Entry.Id);
            Frame.Navigate(typeof(MainPage));
            // TODO: Maybe display a popup dialog to confirm? 
        }

        /// <summary>
        /// Update the content of the suggestion box with the seleted item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddTagBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = (args.SelectedItem as Tag).Name;
            AddTagButton_Tapped(sender, new TappedRoutedEventArgs());
        }

        /// <summary>
        /// Filter the suggestions of to the box with the text in the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddTagBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            sender.ItemsSource = App.Data.Tags.Where(x => x.Name.ToLower().StartsWith(sender.Text.ToLower()))
                .Distinct(new TagNameComparer()).OrderByDescending(x => x.Name);
        }

        /// <summary>
        /// Call the textChanged method for the suggestion box to get a popup right away.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTagBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AddTagBox_TextChanged((AutoSuggestBox) sender, new AutoSuggestBoxTextChangedEventArgs());
        }
    }
}
