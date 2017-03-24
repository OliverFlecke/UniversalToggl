using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.View.Model;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{

    /// <summary>
    /// Page that displays all the project a user have, grouped by workspace.
    /// </summary>
    public sealed partial class ProjectPage : Page
    {
        private ObservableCollection<WorkspaceViewModel> viewModel = new ObservableCollection<WorkspaceViewModel>();

        public ObservableCollection<WorkspaceViewModel> ViewModel { get { return viewModel; } }

        public ProjectPage()
        {
            this.InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            foreach (Workspace workspace in App.data.Workspaces)
            {
                WorkspaceViewModel model = new WorkspaceViewModel(workspace);
                foreach (Project project in App.data.Projects.Where(x => x.WorkspaceID == workspace.Id))
                {
                    model.Projects.Add(project);
                }
                viewModel.Add(model);
            }
        }

    }
}
