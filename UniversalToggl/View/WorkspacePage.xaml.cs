﻿using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkspacePage : Page
    {
        private ObservableCollection<Workspace> workspaces = new ObservableCollection<Workspace>();

        public ObservableCollection<Workspace> Workspaces
        {
            get { return workspaces; }
            set { workspaces = value; }
        }

        public WorkspacePage()
        {
            this.InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            foreach (Workspace workspace in App.user.Workspaces)
            {
                workspaces.Add(workspace);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView view = sender as ListView;
            Workspace workspace = view.SelectedItem as Workspace;

            App.currentWorkspace = workspace;
        }
    }
}