using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace TogglAPI
{
    public class Project
    {
        #region Properties
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceID { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
        #endregion
    }

    public class ProjectNameComparer : IEqualityComparer<Project>
    {
        public bool Equals(Project x, Project y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Project obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
