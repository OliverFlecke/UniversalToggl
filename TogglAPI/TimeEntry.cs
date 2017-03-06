using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace TogglAPI
{
    public class TimeEntry
    {
        #region Properties
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "wid")]
        public int WorkspaceId { get; set; }

        [JsonProperty(PropertyName = "pid")]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        [JsonProperty(PropertyName = "tid")]
        public int TaskId { get; set; }

        [JsonProperty(PropertyName = "billable")]
        public bool Billable { get; set; }

        [JsonProperty(PropertyName = "start")]
        public DateTime Start { get; set; }

        [JsonProperty(PropertyName = "stop")]
        public DateTime Stop { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "at")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        //public List<Tag> Tags { get; set; }
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
            JObject data = JObject.Parse(json);
            //JToken tagToken = data.SelectToken("tags");
            //if (tagToken != null) data.Remove("tags");

            TimeEntry entry = JsonConvert.DeserializeObject<TimeEntry>(data.ToString());

            // Convert the list of names into tag objects - These objects will not have correct id or workspace id
            //if (tagToken != null)
            //{
            //    List<string> tagNames = JsonConvert.DeserializeObject<List<string>>(tagToken.ToString());
            //    entry.Tags = new List<Tag>(tagNames.Count);
            //    foreach (string name in tagNames)
            //        entry.Tags.Add(new Tag(name));
            //}
            return entry;
        }

        /// <summary>
        /// Serialize an object into json
        /// </summary>
        /// <returns>The json format of the objects data</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Convert a datetime object to the ISO 8601 format, which the Wep API requrie
        /// </summary>
        /// <param name="date">The date time object to convert</param>
        /// <returns>A string with the date in ISO 8601 format</returns>
        internal static string DateToISO8601(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        #region ExtractDataMethods
        /// <summary>
        /// Get the running time entry
        /// </summary>
        /// <returns>The time entry object of the current running time entry</returns>
        public static async Task<TimeEntry> GetRunningTimeEntry()
        {
            string json = await Connection.GetAsync("time_entries/current");
            return CreateTimeEntryFromJson(JObject.Parse(json).SelectToken("data").ToString());
        }

        /// <summary>
        /// Get the time entry with the specified id
        /// </summary>
        /// <param name="id">Id of the time entry</param>
        /// <returns>The time entry object with the given id</returns>
        public static async Task<TimeEntry> GetTimeEntry(int id)
        {
            string json = await Connection.GetAsync("time_entries/" + id);
            return CreateTimeEntryFromJson(JObject.Parse(json).SelectToken("data").ToString());
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
                JArray tagNames = new JArray();
                foreach (Tag tag in tags)
                    tagNames.Add(tag.Name);

                entry.Add("tags", tagNames);
            }
            JObject jsonObject = new JObject();
            jsonObject.Add("time_entry", entry);

            string response = await Connection.PostAsync("time_entries/start", jsonObject.ToString());
            return CreateTimeEntryFromJson(JObject.Parse(response).SelectToken("data").ToString());
         }

        /// <summary>
        /// Create a new time entry. This does not start running right away
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static async Task<TimeEntry> CreateTimeEntry(string description, int workspaceId, DateTime start, int duration)
        {
            JObject entry = new JObject();
            entry.Add("description", description);
            entry.Add("duration", duration);
            entry.Add("start", DateToISO8601(start));
            entry.Add("wid", workspaceId);
            entry.Add("created_with", "C#");

            JObject jsonObject = new JObject();
            jsonObject.Add("time_entry", entry);
            
            string json = await Connection.PostAsync("time_entries", jsonObject.ToString());
            return CreateTimeEntryFromJson(JObject.Parse(json).SelectToken("data").ToString());
        }

        /// <summary>
        /// Update a time entry
        /// </summary>
        /// <param name="entry">The entry to update</param>
        /// <returns>The entry with the updated data</returns>
        public static async Task<TimeEntry> UpdateEntry(int id, string description = null, List<Tag> tags = null, DateTime startTime = default(DateTime), DateTime stopTime = default(DateTime))
        {
            JObject entry = new JObject();
            if (description != null) entry.Add("description", description);
            if (tags != null)
            {
                JArray tagNames = new JArray();
                foreach (Tag tag in tags)
                    tagNames.Add(tag.Name);
                
                entry.Add("tags", tagNames);
            }
            if (startTime != default(DateTime)) entry.Add("start", DateToISO8601(startTime));
            if (stopTime != default(DateTime)) entry.Add("stop", DateToISO8601(stopTime));

            JObject jsonObject = new JObject();
            jsonObject.Add("time_entry", entry);


            string response = await Connection.PutAsync("time_entries/" + id, jsonObject.ToString());
            return CreateTimeEntryFromJson(JObject.Parse(response).SelectToken("data").ToString());
        }

        public static async Task<List<TimeEntry>> GetTimeEntriesInRange()
        {
            string response = await Connection.GetAsync("time_entries");

            return JsonConvert.DeserializeObject<List<TimeEntry>>(response);
        }

        /// <summary>
        /// Get a list of time entries in a specific time range 
        /// </summary>
        /// <param name="startDate">The start date to get the time entries from</param>
        /// <param name="endDate">The end date to get the time entries from</param>
        /// <returns>A list of time entries in the given time range</returns>
        public static async Task<List<TimeEntry>> GetTimeEntriesInRange(DateTime startDate, DateTime endDate)
        {
            string url = "time_entries";
            if (startDate != default(DateTime))
            {
                url += "?start_date=" + WebUtility.UrlEncode(DateToISO8601(startDate));
                if (endDate != default(DateTime))
                {
                    url += "&end_date=" + WebUtility.UrlEncode(DateToISO8601(endDate)); // TODO create relative url
                }
            }

            string response = await Connection.GetAsync(url);

            // The response need to be split into lists
            List<TimeEntry> entries = JsonConvert.DeserializeObject<List<TimeEntry>>(response);
            //List<TimeEntry> entries = new List<TimeEntry>(entriesData.Count);
            //foreach (string data in entriesData)
            //    entries.Add(CreateTimeEntryFromJson(data));

            return entries;
        }

        /// <summary>
        /// Stop a time entry
        /// </summary>
        /// <param name="id">The id of the entry to be stopped</param>
        /// <returns>The entry which has been stopped with its newest data</returns>
        public static async Task<TimeEntry> StopTimeEntry(int id)
        {
            string response = await Connection.PutAsync("time_entries/" + id + "/stop");
            return CreateTimeEntryFromJson(JObject.Parse(response).SelectToken("data").ToString());
        }

        /// <summary>
        /// Delete a time entry 
        /// </summary>
        /// <param name="id">The id of the time entry to be deleted</param>
        /// <returns>A task to run this async</returns>
        public static Task DeleteEntry(int id)
        {
            return Connection.DeleteAsync("time_entries/" + id);
        }
        #endregion
    }
}
