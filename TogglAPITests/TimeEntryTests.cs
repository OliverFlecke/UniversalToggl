using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TogglAPI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TogglAPITests
{
    [TestClass]
    public class TimeEntryTests
    {
        private TimeEntry entry;
        private string description = "test entry";
        private int id;

        [TestInitialize]
        public void BeforeEach()
        {
            User.Logon(UserTests.apiToken).Wait();

            Task<TimeEntry> task = TimeEntry.StartTimeEntry(description);
            task.Wait();
            entry = task.Result;
            id = entry.Id;
        }

        [TestCleanup]
        public void AfterEach()
        {
            TimeEntry.DeleteEntry(entry.Id);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetRunningTimeEntryTest()
        {
            // Setup: Need to insure there is a running time entry

            var task = TimeEntry.GetRunningTimeEntry();
            task.Wait();
            TimeEntry entry = task.Result;

            Assert.IsNotNull(entry);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetTimeEntryFromIdTest()
        {
            var task = TimeEntry.GetTimeEntry(528755324);
            task.Wait();
            TimeEntry entry = task.Result;

            Assert.IsNotNull(entry);
            // Check that the ids are correct
            Assert.AreEqual(528755324, entry.Id);
            Assert.AreEqual(1888208, entry.WorkspaceId);
            Assert.AreEqual(30941810, entry.ProjectId);
            Assert.IsFalse(entry.Billable);

            Assert.AreEqual(new DateTime(2017, 2, 4, 10, 54, 22), entry.Start);
            Assert.AreEqual(5504, entry.Duration);
            Assert.AreEqual("Testing entry extraction", entry.Description);
            Trace.WriteLine(entry.LastUpdated);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("POST")]
        public void StartTimeEntryTest()
        {
            var task = TimeEntry.StartTimeEntry("New time entry");
            task.Wait();
            TimeEntry entry = task.Result;

            Assert.AreNotEqual(0, entry.Id);
            Assert.AreEqual("New time entry", entry.Description);
            Assert.IsTrue(0 > entry.Duration);

            // Clean up
            TimeEntry.DeleteEntry(entry.Id).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("POST")]
        public void StartTimeEntryWithTagsTest()
        {
            Tag[] newTags = new Tag[2] { new Tag("test"), new Tag("API")};
            var task = TimeEntry.StartTimeEntry("New time entry", tags: newTags);
            task.Wait();
            TimeEntry entry = task.Result;

            Assert.AreNotEqual(0, entry.Id);
            Assert.AreEqual("New time entry", entry.Description);
            // Check if it contains the correct tags
            Assert.AreEqual(2, entry.Tags.Count);

        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("DELETE")]
        public void DeleteTimeEntryTest()
        {
            // Create a new test entry
            var task = TimeEntry.StartTimeEntry("To be deleted");
            task.Wait();
            var entry = task.Result;

            // Delete the task
            TimeEntry.DeleteEntry(entry.Id).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("POST")]
        public void CreateTimeEntryTest()
        {
            Task<TimeEntry> task = TimeEntry.CreateTimeEntry("New time entry", WorkspaceTests.workspaceId, 
                new DateTime(2016, 9, 20, 10, 35, 17), 100);
            task.Wait();
            TimeEntry entry = task.Result;

            Assert.IsNotNull(entry);
            Assert.AreEqual("New time entry", entry.Description);

            // Clean up
            TimeEntry.DeleteEntry(entry.Id).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("PUT")]
        public void StopTimeEntryTest()
        {
            Task<TimeEntry> startTask = TimeEntry.StartTimeEntry("To be stopped");
            startTask.Wait();
            TimeEntry entry = startTask.Result;
            
            // Let the time run for a bit before stopping the timer
            Thread.Sleep(2000);

            Task<TimeEntry> stopTask = TimeEntry.StopTimeEntry(entry.Id);
            stopTask.Wait();
            entry = stopTask.Result;

            // Check if it is still stopped after a few seconds
            Thread.Sleep(2000);
            var task = TimeEntry.GetTimeEntry(entry.Id);
            task.Wait();
            TimeEntry updateEntry = task.Result;

            Assert.AreEqual(entry.Id, updateEntry.Id);
            Assert.AreEqual(entry.Duration, updateEntry.Duration);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("PUT")]
        public void UpdateTimeEntryDescriptionTest()
        {
            // Is there a difference is the entry is already running?
            Assert.AreEqual(description, entry.Description);
            
            Task<TimeEntry> task = TimeEntry.UpdateEntry(entry.Id, "Updated description!");
            task.Wait();
            entry = task.Result;

            Assert.AreEqual(id, entry.Id);
            Assert.AreEqual("Updated description!", entry.Description);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("PUT")]
        public void UpdateTimeEntryStartAndStopTest()
        {
            // Stop the test entry from running
            TimeEntry.StopTimeEntry(entry.Id).Wait();

            DateTime startTime = new DateTime(2010, 10, 10, 20, 45, 30);
            DateTime stopTime = new DateTime(2011, 11, 11, 11, 11, 11);

            Task<TimeEntry> task = TimeEntry.UpdateEntry(entry.Id, startTime: startTime, stopTime: stopTime);
            task.Wait();
            entry = task.Result;

            Assert.AreEqual(id, entry.Id);
            Assert.AreEqual(startTime, entry.Start);
            Assert.AreEqual(stopTime, entry.Stop);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("PUT")]
        public void UpdateTimeEntryTagsTest()
        {
            List<Tag> tags = new List<Tag>();
            Tag first = new Tag("First", WorkspaceTests.workspaceId);
            Tag second = new Tag("Second", WorkspaceTests.workspaceId);
            tags.Add(first);
            tags.Add(second);

            Task<TimeEntry> task = TimeEntry.UpdateEntry(entry.Id, tags: tags);
            task.Wait();
            entry = task.Result;

            Assert.AreEqual(id, entry.Id);

            // Test that the tags have the correct names
            var enumerator = entry.Tags.GetEnumerator();
            enumerator.MoveNext();
            foreach (Tag tag in tags)
            {
                Assert.AreEqual(tag.Name, enumerator.Current);
                enumerator.MoveNext();
            }
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetTimeEntriesInTimeRangeTest()
        {
            DateTime startDate = new DateTime(2017, 02, 1);
            DateTime endDate = new DateTime(2017, 02, 7);

            Task<List<TimeEntry>> task = TimeEntry.GetTimeEntriesInRange(startDate, endDate);
            task.Wait();
            List<TimeEntry> entries = task.Result;

            foreach (TimeEntry entry in entries)
                Trace.WriteLine(entry.Description);

            Assert.IsNotNull(entries);
            Assert.IsTrue(0 < entries.Count);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetLatestTimeEntries()
        {
            var task = TimeEntry.GetTimeEntriesInRange();
            task.Wait();
            var entries = task.Result;

            foreach (TimeEntry entry in entries)
            {
                Trace.WriteLine(entry.Description);
            }

            Assert.IsNotNull(entries);
            Assert.IsTrue(0 < entries.Count);
        }

        [TestMethod]
        [TestCategory("JSON Serializing")]
        public void TimeEntrySerializingTest()
        {
            DateTime start = new DateTime(2010, 10, 1);
            DateTime stop = new DateTime(2011, 11, 2);
            DateTime lastUpdated = new DateTime(2012, 12, 3);

            List<string> tags = new List<string>();
            tags.Add("First");
            tags.Add("Second");

            TimeEntry entry = new TimeEntry()
            {
                Id = 10,
                Description = "This is a test entry",
                Start = start,
                Stop = stop,
                ProjectId = 200,
                WorkspaceId = 300,
                Duration = 2000,
                Tags = tags,
                Billable = true,
                LastUpdated = lastUpdated,
                TaskId = 20
            };

            string json = entry.Serialize();

            JObject jsonObject = JObject.Parse(json);
            Assert.AreEqual(10, jsonObject.SelectToken("id"));
            Assert.AreEqual("This is a test entry", jsonObject.SelectToken("description"));
            Assert.AreEqual(200, jsonObject.SelectToken("pid"));
            Assert.AreEqual(300, jsonObject.SelectToken("wid"));
            Assert.AreEqual(2000, jsonObject.SelectToken("duration"));
            Assert.AreEqual(start, DateTime.Parse(jsonObject.SelectToken("start").ToString()));
            Assert.AreEqual(stop, DateTime.Parse(jsonObject.SelectToken("stop").ToString()));
            Assert.AreEqual(lastUpdated, DateTime.Parse(jsonObject.SelectToken("at").ToString()));
            Assert.AreEqual(20, jsonObject.SelectToken("tid"));
            Assert.AreEqual(true, jsonObject.SelectToken("billable"));

            // Test that the tags are read correctly
            List<string> jsonTags = JsonConvert.DeserializeObject<List<string>>(jsonObject.SelectToken("tags").ToString());
            Assert.AreEqual(2, jsonTags.Count);
            Assert.AreEqual("First", jsonTags.ToArray()[0]);
            Assert.AreEqual("Second", jsonTags.ToArray()[1]);
        }
    }
}
