using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TogglAPI
{
    public class Workspace
    {
        [JsonProperty]
        public int Id { get; set; }
        
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Get all the workspaces associated with the connected user
        /// </summary>
        /// <returns>A list of all the workspaces</returns>
        public static async Task<List<Workspace>> GetWorkspaces()
        {
            string content = await Connection.GetAsync("workspaces");
            return JsonConvert.DeserializeObject<List<Workspace>>(content);
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="workspaceID">If of the workspace</param>
        /// <returns>The workspace with the specified id</returns>
        public static async Task<List<Project>> GetWorkspaceProjects(int workspaceID)
        {
            string content = await Connection.GetAsync("workspaces/" + workspaceID + "/projects");
            return JsonConvert.DeserializeObject<List<Project>>(content);
        }

        /// <summary>
        /// Get all the tags in a workspace
        /// </summary>
        /// <param name="workspaceID">Id of the workspace to get the tags from</param>
        /// <returns>A list of tags from the workspace</returns>
        public static async Task<List<Tag>> GetWorkspaceTags(int workspaceID)
        {
            string content = await Connection.GetAsync("workspaces/" + workspaceID + "/tags");
            var tags = JsonConvert.DeserializeObject<List<Tag>>(content);

            if (tags == null) return new List<Tag>();
            else return tags;
        }
    }
}
