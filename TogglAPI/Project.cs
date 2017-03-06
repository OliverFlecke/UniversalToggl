using Newtonsoft.Json;

namespace TogglAPI
{
    public class Project
    {
        #region Properties
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int ProjectID { get; set; }

        #endregion
    }
}
