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

        public DateTimeOffset StartDate { get { return DateTime.SpecifyKind(Entry.Start, DateTimeKind.Local); } }
        public DateTimeOffset EndDate { get { return DateTime.SpecifyKind(Entry.Stop, DateTimeKind.Local); } }

        public TimeEntryDetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Entry = e.Parameter as TimeEntry;
        }
    }
}
