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
        private TimeEntry entry;

        public TimeEntry Entry
        {
            get { return entry; }
            set
            {
                entry = value;
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
                    return timer.Elapsed.ToString("hh\\:mm\\:ss");
                }
            }
        }

        public TextBlock TimeDisplay { get; internal set; }

        private void TimerSetup()
        {
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
