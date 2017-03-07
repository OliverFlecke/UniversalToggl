using System.Collections.ObjectModel;
using TogglAPI;

namespace UniversalToggl.View.Model
{
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
            get { return projects; }
            set { projects = value; }
        }

        public WorkspaceViewModel(Workspace workspace)
        {
            this.workspace = workspace;
        }
    }
}
