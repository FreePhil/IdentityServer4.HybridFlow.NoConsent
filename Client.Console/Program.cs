using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;

namespace Client.Console
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            System.Console.WriteLine("Getting access token...");

            // Discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:42705/");

            // Request token with client credentials
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client.console", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                System.Console.WriteLine($"Error: {tokenResponse.Error}.");
                return;
            }

            System.Console.WriteLine($"Access token: {tokenResponse.Json}");
            System.Console.WriteLine("\n\n");

            System.Console.WriteLine("Calling protected API with access token...");

            // Call API
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:34599/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine($"API error: {response.StatusCode}");
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                System.Console.WriteLine($"API result:\n\n{content}");
            }

            System.Console.ReadLine();
        }
    }
}
