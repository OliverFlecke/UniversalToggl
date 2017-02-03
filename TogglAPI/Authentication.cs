using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;


namespace TogglAPI
{
    public class Authentication
    {
        /// <summary>
        /// Insures that the HttpClient is setup correctly in order to talk to the Web API
        /// </summary>
        public Authentication()
        {
            //client.BaseAddress = new Uri(url);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", user.GetAuthenticationToken());
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //public async Task<string> Logon()
        //{
        //    string response = null;
        //    JObject content = new JObject();

        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, @"/api/v8/me");

        //    // This is important when sending POST requests
        //    //request.Content = new StringContent("{\"time_entry\":{ \"description\":\"Meeting with possible clients,\"tags\":[\"billed\"],\"duration\":1200,\"start\":\"2013-03-05T07:58:58.000Z\",\"pid\":123,\"created_with\":\"curl\"}}", Encoding.UTF8, "application/json");

        //    HttpResponseMessage httpResponse = await client.SendAsync(request);

        //    response = await httpResponse.Content.ReadAsStringAsync();
        //    if (httpResponse.IsSuccessStatusCode)
        //    {
        //        response += "\nAt least something happend";
        //    }


        //    return response;
        //}

        //public async Task PutTest()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, @"/api/v8/me");

        //    request.Content = new StringContent("{\"user\":{\"fullname\":\"John Smith\"}}");//, Encoding.UTF8, "application/json");
            

        //    HttpResponseMessage response = await client.PutAsync(@"/api/v8/me", request.Content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // do something
        //    }
        //    else
        //    {
        //        throw new Exception();
        //    }
        //}

        //public async Task<string> GetWorkspaces()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url + "workspaces");
        //    HttpResponseMessage response = await client.SendAsync(request);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return ExtractContent(response);
        //    }
        //    else
        //    {
        //        throw new Exception();
        //    }
        //}

        //public async Task<string> GetMe()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Connection.Url + "me");
        //    HttpResponseMessage response = await client.SendAsync(request);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return ExtractContent(response);
        //    }
        //    else
        //    {
        //        throw new Exception(); // Come up with a better exception
        //    }
        //}
    }
}