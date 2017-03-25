using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TogglAPI;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TogglAPITests
{
    [TestClass]
    public class WorkspaceTests
    {
        internal static int workspaceId = 1888208;

        [TestInitialize]
        public void BeforeEach()
        {
            Connection.Reset();
            User.Logon("e1f948fc1c6b2a09186309e61bfb4743").Wait();
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetWorkspaces()
        {
            var task = Workspace.GetWorkspaces();
            task.Wait();
            List<Workspace> workspaces = task.Result;

            Assert.AreEqual(2, workspaces.Count);

            Workspace first = workspaces[0];
            Assert.AreEqual(1888208, first.Id);
            Assert.AreEqual("Toggltestmail's workspace", first.Name);

            Workspace second = workspaces[1];
            Assert.AreEqual(1888354, second.Id);
            Assert.AreEqual("TestWorkspace", second.Name);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetWorkspaceProjcetsTest()
        {
            var task = Workspace.GetWorkspaceProjects(workspaceId);
            task.Wait();
            var projects = task.Result;

            Assert.AreEqual(2, projects.Count);
            Project first = projects[0];
            Assert.AreEqual("Second project", first.Name);
            Assert.IsTrue(0 < first.ID);
            Trace.Write(first.ID);

            Project second = projects[1];
            Assert.AreEqual("Toggl Testing", second.Name);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetWorkspaceTagsTest()
        {
            var task = Workspace.GetWorkspaceTags(workspaceId);
            task.Wait();
            List<Tag> tags = task.Result;
            Assert.AreEqual(5, tags.Count);

            Tag first = tags.ToArray()[0];
            Assert.AreEqual("Second", first.Name);
        }

        [TestMethod]
        [TestCategory("Web API")]
        [TestCategory("GET")]
        public void GetWorkspaceTagsWithNoTagsTest()
        {
            var task = Workspace.GetWorkspaceTags(1888354);
            task.Wait();
            var tags = task.Result;

            Assert.IsNotNull(tags);
            Assert.AreEqual(0, tags.Count);
        }
    }
}
