using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TogglAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalToggl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<TimeEntry> timeEntries = new ObservableCollection<TimeEntry>();
        public ObservableCollection<TimeEntry> TimeEntries { get { return this.timeEntries; } }

        public MainPage()
        {
            this.InitializeComponent();

            UpdateTimeEntries();
        }

        public async void UpdateTimeEntries()
        {
            var entries = await TimeEntry.GetTimeEntriesInRange();
            foreach (TimeEntry entry in entries)
                timeEntries.Add(entry);
        }
    }
}
