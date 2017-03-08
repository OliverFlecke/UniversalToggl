using Newtonsoft.Json;

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
}
