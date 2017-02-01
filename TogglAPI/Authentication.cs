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
        static string userpass = token + ":api_token";
        static string url = "https://www.toggl.com/api/v8/";
        // The encoding of this variable is not accepted by the httpClient.
        static string userpassB64 = Convert.ToBase64String(Encoding.Unicode.GetBytes(userpass)); 

        HttpClient client = new HttpClient();

        public async Task<string> Logon()
        {
            string response = null;
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userpassB64);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            JObject content = new JObject();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "me");

            // This is important when sending POST requests
            //request.Content = new StringContent("{\"time_entry\":{ \"description\":\"Meeting with possible clients\",\"tags\":[\"billed\"],\"duration\":1200,\"start\":\"2013-03-05T07:58:58.000Z\",\"pid\":123,\"created_with\":\"curl\"}}", Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await client.SendAsync(request);

            response = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                response += "\nAt least something happend";
            }


            return response;
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