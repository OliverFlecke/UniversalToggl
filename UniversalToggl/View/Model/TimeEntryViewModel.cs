using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TogglAPI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalToggl.View.Model
{
    public class TimeEntryViewModel : INotifyPropertyChanged
    {
        private TimeSpan offset;
        private TimeEntry entry;

        public TimeEntry Entry
        {
            get { return entry; }
            set
            {
                entry = value;
                if (value != null)
                    TimerSetup();
                this.OnPropertyChaged();
            }
        }


        // Time for the entry
        private DispatcherTimer dt;
        private Stopwatch timer;

        public string Time
        {
            get
            {
                if (timer == null)
                    return string.Empty;
                else
                {
                    var span = timer.Elapsed + offset;
                    return string.Format("{0:00}:{1:00}:{2:00}", span.Days * 24 + span.Hours, span.Minutes, span.Seconds);
                }
            }
        }

        public TextBlock TimeDisplay { get; internal set; }

        /// <summary>
        /// Setup the timer
        /// </summary>
        private void TimerSetup()
        {
            offset = new TimeSpan(DateTime.Now.ToUniversalTime().Ticks - entry.Start.ToUniversalTime().Ticks);
            timer = new Stopwatch();
            timer.Start();

            dt = new DispatcherTimer();
            dt.Tick += dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 1);
            dt.Start();
        }

        private void dt_Tick(object sender, object e)
        {
            if (timer.IsRunning)
            {
                TimeDisplay.Text = Time;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChaged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
