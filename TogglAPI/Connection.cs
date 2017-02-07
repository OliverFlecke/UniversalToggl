using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TogglAPI
{
    public static class Connection
    {
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// The client which can connect to the Web API
        /// </summary>
        static public HttpClient Client
        {
            get { return client; }
        }

        /// <summary>
        /// The base URL to the Web API
        /// </summary>
        public static string Url
        {
            get { return @"https://www.toggl.com/api/v8/"; }
        }

        /// <summary>
        /// Boolean to idicate if the client is alread connected
        /// </summary>
        static private bool isConnected = false;

        /// <summary>
        /// Make the actual connection to the Web API
        /// </summary>
        /// <param name="authenticationString"></param>
        private static void SetupConnection(string authenticationString)
        {
            if (isConnected) return;
            client.BaseAddress = new Uri(Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationString);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            isConnected = true;
        }



        /// <summary>
        /// Setup the connection to the Web API with the correct credientials
        /// </summary>
        /// <param name="user">The user with the api token</param>
        internal static void SetupConnection(User user)
        {
            SetupConnection(user.GetAuthenticationToken());
        }

        /// <summary>
        /// Setup the connection with an email and a password
        /// </summary>
        /// <param name="email">Email used to login</param>
        /// <param name="password">Password assosiated with the email</param>
        internal static void SetupConnection(string email, string password)
        {
            SetupConnection(User.ConvertToBase64(email + ":" + password));
        }


        /// <summary>
        /// Reset the Http client connection
        /// </summary>
        public static void Reset()
        {
            client = new HttpClient();
            isConnected = false;
        }

        /// <summary>
        /// Read the json content from the response and return the string
        /// </summary>
        /// <param name="response">Response to read the content from</param>
        /// <returns>The content from the response</returns>
        public static string ExtractContent(HttpResponseMessage response)
        {
            string responseValue = string.Empty;

            Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
            {
                var stream = t.Result;
                using (var reader = new StreamReader(stream))
                {
                    responseValue = reader.ReadToEnd();
                }
            });

            task.Wait();
            return responseValue;
        }

        /// <summary>
        /// Send a post request to the Web API
        /// </summary>
        /// <param name="relativeUrl">The relative Url to the API</param>
        /// <param name="json">The json content to send</param>
        /// <returns>A task object with the respond string</returns>
        internal static async Task<string> PostAsync(string relativeUrl, string json)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url + relativeUrl);
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            HttpResponseMessage response = await Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                ErrorHandling(response);
            return ExtractContent(response);
        }

        /// <summary>
        /// Send a put request to the Web API
        /// </summary>
        /// <param name="relativeUrl">The relative url to the item which should be updated</param>
        /// <param name="json">The data to update with</param>
        /// <returns>A string with the response from the update</returns>
        internal static async Task<string> PutAsync(string relativeUrl, string json = "")
        {
            HttpResponseMessage response = await Client.PutAsync(Url + relativeUrl, new StringContent(json));

            if (!response.IsSuccessStatusCode)
                ErrorHandling(response);
            return ExtractContent(response);
        }

        /// <summary>
        /// Delete from the Web API 
        /// </summary>
        /// <param name="relativeUrl">The url to the item to be deleted</param>
        internal static async Task DeleteAsync(string relativeUrl)
        {
            HttpResponseMessage response = await Client.DeleteAsync(Url + relativeUrl);

            if (response.StatusCode != HttpStatusCode.OK)
                ErrorHandling(response);
        }

        /// <summary>
        /// Send a GET request to the Web API 
        /// </summary>
        /// <returns>A string with the response</returns>
        internal static async Task<string> GetAsync(string relativeUrl)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url + relativeUrl);
            HttpResponseMessage response = await Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                ErrorHandling(response);
            return ExtractContent(response);
        }

        /// <summary>
        /// Handle errors when sending http requests
        /// </summary>
        /// <param name="response">The response from the request</param>
        private static void ErrorHandling(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthenticationException("Email or password are not correct", (int)response.StatusCode);
            throw new Exception();
        }
    }
}
