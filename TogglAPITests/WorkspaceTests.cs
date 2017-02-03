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
        [TestInitialize]
        public void BeforeEach()
        {
            Connection.Reset();
            User.Logon("e1f948fc1c6b2a09186309e61bfb4743").Wait();
        }

        [TestMethod]
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
    }
}
