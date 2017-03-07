using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TogglAPI;
using UniversalToggl.View.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectPage : Page
    {
        private ObservableCollection<WorkspaceViewModel> viewModel = new ObservableCollection<WorkspaceViewModel>();

        public ObservableCollection<WorkspaceViewModel> ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }

        public ProjectPage()
        {
            this.InitializeComponent();

            Setup();
        }

        private async void Setup()
        {
            foreach (Workspace workspace in App.user.Workspaces)
            {
                WorkspaceViewModel model = new WorkspaceViewModel(workspace);
                foreach (Project project in await Workspace.GetWorkspaceProjects(workspace.Id))
                {
                    model.Projects.Add(project);
                }
                viewModel.Add(model);
            }
        }

    }
}
