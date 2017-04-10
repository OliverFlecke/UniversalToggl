using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TogglAPI;
using Windows.UI.Xaml;

namespace UniversalToggl.View.Model
{
    public class TimeEntryByDateViewModel
    {
        private DateTime date;

        public DateTime Date { get { return date; } }

        public string DateAsString { get { return date.ToString("dd MMM yyyy"); } }

        private ObservableCollection<TimeEntry> entries;
        public ObservableCollection<TimeEntry> Entries
        {
            get { return entries; }
        }

        private Visibility visible = Visibility.Visible;

        public Visibility IsVisible
        {
            get { return visible; }
            set { visible = value; }
        }

        public string TotalTime
        {
            get
            {
                long sum = Entries.Sum(x => x.Duration);
                return TimeEntry.DurationToString((int) sum);
            }
        }


        /// <summary>
        /// Create a new model with a date 
        /// </summary>
        /// <param name="date"></param>
        public TimeEntryByDateViewModel(DateTime date)
        {
            if (date == null)
            {
                throw new ArgumentNullException("Date cannot be null");
            }
            this.date = date;
            entries = new ObservableCollection<TimeEntry>();
        }


    }
}
