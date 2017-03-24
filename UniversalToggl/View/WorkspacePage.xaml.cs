using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{
    /// <summary>
    /// A page to display all the workspace a user have.
    /// </summary>
    public sealed partial class WorkspacePage : Page
    {

        public ObservableCollection<Workspace> Workspaces { get { return App.data.Workspaces; } }

        public WorkspacePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView view = sender as ListView;
            Workspace workspace = view.SelectedItem as Workspace;

            App.currentWorkspace = workspace;
        }
    }
}
