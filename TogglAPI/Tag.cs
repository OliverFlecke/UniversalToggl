﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace TogglAPI
{
    [DataContract]
    public class Tag
    {
        #region Properties
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public int Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "wid", Required = Required.AllowNull)]
        public int WorkspaceId { get; set; }
        #endregion

        public Tag(string name, int workspaceId = 0)
        {
            Name = name;
            WorkspaceId = workspaceId;
        }

        /// <summary>
        /// Create a new tag
        /// </summary>
        /// <param name="name">Name of the new tag</param>
        /// <param name="workspaceId">The id of the workspace, which the new tag should belong to</param>
        /// <returns>The newly created tag</returns>
        public static async Task<Tag> CreateTag(string name, int workspaceId)
        {
            JObject jsonObject = new JObject();
            JObject tag = new JObject();
            tag.Add("name", name);
            tag.Add("wid", workspaceId);

            jsonObject.Add("tag", tag);

            string response = await Connection.PostAsync("tags", jsonObject.ToString());
            return CreateTagFromJSON(response);
        }

        /// <summary>
        /// Delete a tag 
        /// </summary>
        /// <param name="id">The id of the tag to be deleted</param>
        /// <returns></returns>
        public static Task DeleteTag(int id)
        {
            return Connection.DeleteAsync("tags/" + id);
        }

        /// <summary>
        /// Update a tag with a new name
        /// </summary>
        /// <param name="id">The id of the tag</param>
        /// <param name="name">The new name of the tag</param>
        /// <returns>The updated tag object</returns>
        public static async Task<Tag> UpdateTag(int id, string name)
        {
            JObject jsonObject = new JObject();
            JObject tag = new JObject();
            tag.Add("name", name);
            jsonObject.Add("tag", tag);

            string response = await Connection.PutAsync("tags/" + id, jsonObject.ToString());
            return CreateTagFromJSON(response);
        }

        /// <summary>
        /// Serialize this object to json
        /// </summary>
        /// <returns>A string of JSON with the data from the tag</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a tag object from a json data response
        /// </summary>
        /// <param name="json">The json containing the data for the object</param>
        /// <returns>A tag with the data from the json string</returns>
        private static Tag CreateTagFromJSON(string json)
        {
            return JsonConvert.DeserializeObject<Tag>(JObject.Parse(json).SelectToken("data").ToString());
        }
    }

    /// <summary>
    /// Comparer to compare time entries, which are equal if there discription is equal.
    /// </summary>
    public class TagNameComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Tag obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}