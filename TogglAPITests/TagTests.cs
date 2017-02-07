using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TogglAPI;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TogglAPITests
{
    [TestClass]
    public class TagTests
    {
        [TestInitialize]
        public void BeforeEach()
        {
            User.Logon(UserTests.apiToken).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("POST")]
        public void CreateTagTest()
        {
            Task<Tag> task = Tag.CreateTag("New tag", WorkspaceTests.workspaceId);
            task.Wait();
            Tag tag = task.Result;

            Assert.AreNotEqual(0, tag.Id);
            Assert.AreEqual("New tag", tag.Name);
            Assert.AreEqual(1888208, tag.WorkspaceId);

            // Should delete the new tag afterwards
            Tag.DeleteTag(tag.Id);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("DELETE")]
        public void DeleteTagTest()
        {
            var task = Tag.CreateTag("Tag to be deleted", WorkspaceTests.workspaceId);
            task.Wait();
            int id = task.Result.Id;

            Tag.DeleteTag(id).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("PUT")]
        public void UpdateTagTest()
        {
            // Create a new tag, which can be updated
            var task = Tag.CreateTag("Updateable", WorkspaceTests.workspaceId);
            task.Wait();
            Tag original = task.Result;
            int id = original.Id;

            // Update the tag
            Task<Tag> tagTask = Tag.UpdateTag(id, "New tag name!");
            tagTask.Wait();
            Tag tag = tagTask.Result;

            Assert.AreNotEqual(original.Name, tag.Name);
            Assert.AreEqual("New tag name!", tag.Name);
            Assert.AreEqual(id, tag.Id);
            Assert.AreEqual(WorkspaceTests.workspaceId, tag.WorkspaceId);

            // Clean up afterwards
            Tag.DeleteTag(id).Wait();
        }

        [TestMethod]
        [TestCategory("JSON Serializing")]
        public void TagSerializingTest()
        {
            Tag tag = new Tag("Some name", 100);

            string json = tag.Serialize();

            JObject jsonObject = JObject.Parse(json);
            Assert.AreEqual("Some name", jsonObject.SelectToken("name"));
            Assert.AreEqual(100, jsonObject.SelectToken("wid")); 
        }
    }
}
