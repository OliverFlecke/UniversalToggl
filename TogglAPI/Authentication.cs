using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;


namespace TogglAPI
{
    public class Authentication
    {
        static string token = "1b04fad3e55278ec4f2193d2ae66eaa2";
        static string tokenAPI = token + ":api_token";
        static string userpass = "oliverfl@live.dk:ZAq1XSw2";
        static string url = @"https://www.toggl.com/api/v8/";
        // The encoding of this variable is not accepted by the httpClient.
        //static string userpassB64 = Convert.ToBase64String(Encoding.Unicode.GetBytes(tokenAPI)); 
        // This is my api token + ":api_token" converted with an online tool. Need to find a way to do this in C#
        static string userpassB64 = "MWIwNGZhZDNlNTUyNzhlYzRmMjE5M2QyYWU2NmVhYTI6YXBpX3Rva2Vu";

        HttpClient client = new HttpClient();


        public Authentication()
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userpassB64);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> Logon()
        {
            string response = null;
            JObject content = new JObject();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, @"/api/v8/me");

            // This is important when sending POST requests
            //request.Content = new StringContent("{\"time_entry\":{ \"description\":\"Meeting with possible clients,\"tags\":[\"billed\"],\"duration\":1200,\"start\":\"2013-03-05T07:58:58.000Z\",\"pid\":123,\"created_with\":\"curl\"}}", Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await client.SendAsync(request);

            response = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                response += "\nAt least something happend";
            }


            return response;
        }

        public async Task PutTest()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, @"/api/v8/me");

            request.Content = new StringContent("{\"user\":{\"fullname\":\"John Smith\"}}");//, Encoding.UTF8, "application/json");
            

            HttpResponseMessage response = await client.PutAsync(@"/api/v8/me", request.Content);
            if (response.IsSuccessStatusCode)
            {
                // do something
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<string> GetWorkspaces()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url + "workspaces");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return ExtractContent(response);
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<string> GetMe()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url + "me");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return ExtractContent(response);
            }
            else
            {
                throw new Exception(); // Come up with a better exception
            }
        }

        private static string ExtractContent(HttpResponseMessage response)
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

        public async Task<IEnumerable<RecipeDataItemDto>> GetRecipeDataItemsAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://contosorecipes8.blob.core.windows.net/");
            var jsonTypeFormatter = new JsonMediaTypeFormatter();

            jsonTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application / octet - stream"));

            var response = await client.GetAsync(("AzureRecipesRP"));

            return await response.Content.ReadAsAsync<List<RecipeDataItemDto>>(new[] { jsonTypeFormatter });

        }

    }
    public class RecipeDataItemDto
    {
        [JsonProperty("key")]
        public string UniqueId
        { get; set; }

        public string Title
        { get; set; }
    }
}