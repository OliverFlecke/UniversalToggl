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

        public static async Task<List<Workspace>> GetWorkspaces()
        {
            string content = await Connection.GetAsync("workspaces");
            return JsonConvert.DeserializeObject<List<Workspace>>(content);
        }

        public static async Task<List<Project>> GetWorkspaceProjects(int workspaceID)
        {
            string content = await Connection.GetAsync("workspaces/" + workspaceID + "/projects");
            return JsonConvert.DeserializeObject<List<Project>>(content);
        }
    }
}
