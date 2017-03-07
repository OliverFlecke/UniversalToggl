using System.ComponentModel;
using System.Runtime.CompilerServices;
using TogglAPI;

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
                this.OnPropertyChaged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChaged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
