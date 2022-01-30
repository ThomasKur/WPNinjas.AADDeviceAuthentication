using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WPNinjas.AADDeviceAuthentication.Client;
using WPNinjas.AADDeviceAuthentication.Common;
using WPNinjas.AADDeviceAuthentication.Server;

namespace WPNinjas.AADDeviceAuthentication.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello WPNinja Community!");

            Console.WriteLine("");
            Console.WriteLine("Generate Client Token! the result would normally be submitted to a server application...");
            string tokenJson = ClientSide();
            Console.WriteLine("");
            Console.WriteLine(tokenJson);

            Console.WriteLine("Now the server will validate the created token...");

            var result = ServerSide(tokenJson);
            result.Wait();
            Console.WriteLine("Token verification Status: " + result.Result);
            Console.ReadLine();
        }

        static string ClientSide()
        {
            AADDeviceAuthClient client = new AADDeviceAuthClient();

            // Get Token, recommended to all of the date you plan to submit to the server or at least
            // the important part to ensure it was not modified.
            DeviceAuthToken token = client.GetToken("Test123");

            // Now we can send the token as part of the body to the WebServer, in this example we will
            // just return it to the Main Program

            // RestClient rc = new RestClient("https://localhost");
            // var request = new RestRequest("inventory") { Method = Method.Post };
            // request.AddJsonBody(token);
            // var result = rc.PostAsync(request);

            return JsonSerializer.Serialize(token);
        }
        static Task<bool> ServerSide(string tokenJson)
        {
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "common";

            // Values from app registration
            var clientId = "YOUR_CLIENT_ID";
            var clientSecret = "YOUR_CLIENT_SECRET";

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // Generate https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);
            var graphClient = new GraphServiceClient(clientSecretCredential);
            AADDeviceAuthServer server = new AADDeviceAuthServer(graphClient);


            // Validate token

            return server.Authenticate(JsonSerializer.Deserialize<DeviceAuthToken>(tokenJson));

        }
    }
}
