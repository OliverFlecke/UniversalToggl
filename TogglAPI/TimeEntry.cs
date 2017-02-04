using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TogglAPI
{
    public class TimeEntry
    {
        #region Properties
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "wid")]
        public int WorkspaceId { get; set; }

        [JsonProperty(PropertyName = "pid")]
        public int ProjectId { get; set; }

        [JsonProperty(PropertyName = "tid")]
        public int TaskId { get; set; }

        [JsonProperty]
        public bool Billable { get; set; }

        [JsonProperty]
        public DateTime Start { get; set; }

        [JsonProperty]
        public DateTime Stop { get; set; }

        [JsonProperty]
        public long Duration { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public List<Tag> Tags { get; set; }

        [JsonProperty(PropertyName = "at")]
        public DateTime LastUpdated { get; set; }
        #endregion

        public TimeEntry()
        {

        }

        public TimeEntry(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Create a time entry object from a string of json
        /// </summary>
        /// <param name="json">Json string with the objects data</param>
        /// <returns>A time entry object with the data from the json</returns>
        private static TimeEntry CreateTimeEntryFromJson(string json)
        {
            return JsonConvert.DeserializeObject<TimeEntry>(JObject.Parse(json).SelectToken("data").ToString());
        }

        #region ExtractDataMethods
        /// <summary>
        /// Get the running time entry
        /// </summary>
        /// <returns>The time entry object of the current running time entry</returns>
        public static async Task<TimeEntry> GetRunningTimeEntry()
        {
            string json = await Connection.GetAsync("time_entries/current");
            return CreateTimeEntryFromJson(json);
        }

        /// <summary>
        /// Get the time entry with the specified id
        /// </summary>
        /// <param name="id">Id of the time entry</param>
        /// <returns>The time entry object with the given id</returns>
        public static async Task<TimeEntry> GetTimeEntry(int id)
        {
            string json = await Connection.GetAsync("time_entries/" + id);
            return CreateTimeEntryFromJson(json);
        }

        /// <summary>
        /// Start a new time entry
        /// </summary>
        /// <param name="entry">The newly started entry</param>
        public static async Task<TimeEntry> StartTimeEntry(string description, int projectId = 0, Tag[] tags = null)
        {
            // Format the entry
            JObject entry = new JObject();
            entry.Add("description", description);
            // The API needs to know how this task was created
            entry.Add("created_with", "C#");

            // Optional fields
            if (projectId != 0) entry.Add("pid", projectId);
            if (tags != null)
            {
                //string[] tagNames = new string[tags.Length];
                //for (int i = 0; i < tags.Length; i++)
                //    tagNames[i] = tags[i].Name;
                //entry.Add("tags", JToken.(tagNames));
            }
            JObject jsonObject = new JObject();
            jsonObject.Add("time_entry", entry);

            string response = await Connection.PostAsync("time_entries/start", jsonObject.ToString());
            return CreateTimeEntryFromJson(response);
         }
        #endregion
    }
}
