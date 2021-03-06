﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TogglAPI;
using System.Threading.Tasks;
using System.Threading;

namespace TogglAPITests
{
    [TestClass]
    public class UserTests
    {
        internal static string apiToken = "e1f948fc1c6b2a09186309e61bfb4743";
        private static string email = "toggltestmail@outlook.com";
        private static int id = 2700947;


        [TestInitialize]
        public void BeforeEach()
        {
            Connection.Reset();
        }

        [TestMethod]
        public void ConvertAPITokenToBase64Test()
        {
            string token = apiToken + ":api_token";
            Assert.AreEqual("ZTFmOTQ4ZmMxYzZiMmEwOTE4NjMwOWU2MWJmYjQ3NDM6YXBpX3Rva2Vu", User.ConvertToBase64(token));
        }

        [TestMethod]
        [TestCategory("Web API")]
        public void LoginWithEmailAndPasswordTest()
        {
            Task<User> userTask = User.Logon(email, "12345678");
            userTask.Wait();
            User user = userTask.Result;

            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(apiToken, user.Token);
            Assert.AreEqual(id, user.Id);
            Assert.AreEqual("Toggltestmail", user.Fullname);
        }

        [TestMethod]
        [TestCategory("Web API")]
        public void LoginWithAPItokenTest()
        {
            Task<User> task = User.Logon(apiToken);
            task.Wait();
            User user = task.Result;

            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(apiToken, user.Token);
            Assert.AreEqual(id, user.Id);
        }

        [TestMethod]
        [TestCategory("Web API")]
        public void LoginWithWrongPasswordTest()
        {
            try
            {
                var task = User.Logon(email, "not the password");
                task.Wait();
                User user = task.Result;
            } 
            catch(AggregateException ex)
            {
                AuthenticationException authenEx = (AuthenticationException)ex.InnerException;
                Assert.AreEqual(403, authenEx.ResposeCode);
                Assert.AreEqual("Email or password are not correct", authenEx.Message);
            }
        }

        [TestMethod]
        [TestCategory("Web API")]
        public void CheckCorrectParsingOfDataFromJsonTest()
        {
            var task = User.Logon(apiToken);
            task.Wait();
            User user = task.Result;

            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(apiToken, user.Token);
            Assert.AreEqual(id, user.Id);
            Assert.AreEqual("Toggltestmail", user.Fullname);

            Assert.IsNotNull(user.Workspaces);
            Assert.IsTrue(user.Workspaces.Count >= 1);
        }
    }
}
