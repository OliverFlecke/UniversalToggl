using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TogglAPI;
using System.Diagnostics;

namespace TogglAPITests
{
    [TestClass]
    public class TimeEntryTests
    {
        [TestInitialize]
        public void BeforeEach()
        {
            User.Logon(UserTests.apiToken).Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
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
        }
    }
}
