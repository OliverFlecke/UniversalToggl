using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TogglAPI
{
    public class User
    {
        #region properties

        [JsonProperty]
        /// <summary>
        /// The ID of the user
        /// </summary>
        public int Id { get; set; }

        [JsonProperty]
        /// <summary>
        /// The users Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The users password
        /// </summary>
        public string Password { get; private set; }

        [JsonProperty(PropertyName = "api_token")]
        /// <summary>
        /// The users API token 
        /// </summary>
        public string Token { get; set; }
        #endregion

        /// <summary>
        /// Create a user without any details
        /// </summary>
        public User()
        {

        }

        /// <summary>
        /// Get the API token in base64 used to authenticate the user through the Web API.
        /// The API token is appended with ':api_token' and converted to base64.
        /// </summary>
        /// <returns>The API token to authenticate the user through the Web API</returns>
        public string GetAuthenticationToken()
        {
            return ConvertToBase64(Token + ":api_token");
        }

        #region staticMethods
        /// <summary>
        /// Convert a string in UTF8 to a base64 string
        /// </summary>
        /// <param name="token">String to convert to base 64</param>
        /// <returns>The input string as base 64</returns>
        public static string ConvertToBase64(string token)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        }

        /// <summary>
        /// Extracts the user data from the json string and convert it to an object
        /// </summary>
        /// <param name="json">The user data</param>
        /// <returns>The user with the data extracted from the json string</returns>
        public static User CreateUserFromJSON(string json)
        {
            return JsonConvert.DeserializeObject<User>(JObject.Parse(json).SelectToken("data").ToString());
        }

        /// <summary>
        /// Logon to the system, after the connection has been created correctly
        /// </summary>
        /// <returns>The user that has connected to the Web API</returns>
        private static async Task<User> Logon()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Connection.Url + "me");
            HttpResponseMessage response = await Connection.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return CreateUserFromJSON(Connection.ExtractContent(response));
            else
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new AuthenticationException("Email or password are not correct", (int) response.StatusCode);
                throw new Exception();
            }
        }

        /// <summary>
        /// Let the user connect with an emal and a password
        /// </summary>
        /// <param name="email">The users email</param>
        /// <param name="password">The password assosieted with the users email</param>
        /// <returns>The user which has connected to the API</returns>
        public async static Task<User> Logon(string email, string password)
        {
            Connection.SetupConnection(email, password);
            return await Logon();
        }

        /// <summary>
        /// Connect to the Web API with an api token
        /// </summary>
        /// <param name="apiToken">The users api token</param>
        /// <returns>The user connected with the api token</returns>
        public async static Task<User> Logon(string apiToken)
        {
            Connection.SetupConnection(new User() { Token = apiToken });
            return await Logon();
        }

        #endregion
    }
}
