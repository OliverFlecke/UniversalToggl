using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using TogglAPI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl
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

    /// <summary>
    /// Class to represent the view of the projects in each workspace for this page
    /// </summary>
    public class WorkspaceViewModel
    {
        private Workspace workspace;

        public string Name
        {
            get { return workspace.Name; }
        }

        private ObservableCollection<Project> projects = new ObservableCollection<Project>();

	    public ObservableCollection<Project> Projects
	    {
		    get { return projects;}
		    set { projects = value;}
	    }
	
        public WorkspaceViewModel(Workspace workspace)
        {
            this.workspace = workspace;
        }
    }
}
