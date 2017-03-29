using System;
using TogglAPI;
using Windows.UI.Xaml.Controls;
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
    }
}
