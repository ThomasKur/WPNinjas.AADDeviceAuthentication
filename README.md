# WPNinjas Device Based Authentication for Azure AD

When creating solutions in Endpoint Management it's often the case that you need to execute scripts in SYSTEM context and submit data to a webservice. As long the devices are Active Directory Joined or Azure Active Directory Hybrid Joined this is not an issue as the computer itself has an identity (Computer object) which includes a password and therefore can authenticate and identify to another system. With the usage of Azure AD the system is no longer an identity and therefore it's problematic, but in the background each device gets a unique certificate from Azure AD and the public key is stored in Azure AD within the AlternativeSecurityIds property. Jairo Cadena has written a very [detailed Blog](https://jairocadena.com/2016/02/01/azure-ad-join-what-happens-behind-the-scenes/) about this process and I can only recommend to read it. 

My solution provides a simple to use client library and server library which allows you to really identify and proof the source of a request.

![Architecture Overview and data flow](https://raw.githubusercontent.com/ThomasKur/WPNinjas.AADDeviceAuthentication/main/Doc/DeviceAuthenticationFlow.svg)

Improvements and contributions are always welcome.

# Use Cases 
I use the solution currently in the following use cases:

* Powershell Script deployed via MEM on Clients which calls a Azure Function --> [CloudLAPS](https://github.com/MSEndpointMgr/CloudLAPS)
* Small Exe in Task Scheduler whcih sends data to a web service --> Collect Inventory data 


# Examples

## Nuget package for .NET projects
The package is available on [Nuget](https://www.nuget.org/packages/WPNinjas.AADDeviceAuthentication). Simply install it by searching for 'WPNinjas.AADDeviceAuthentication'. You can then use the client as followed (*Important:* Testing needs to be done on a AAD Joined device. For testing purposes you can also provide a [certificate and device id (Line 27)](https://github.com/ThomasKur/WPNinjas.AADDeviceAuthentication/blob/main/WPNinjas.AADDeviceAuthentication.Test/AADDeviceAuthClientTests.cs).):

### Client Side

```c#

using System;
using System.Text.Json;
using WPNinjas.AADDeviceAuthentication.Client;
using WPNinjas.AADDeviceAuthentication.Common;

namespace WPNinjas.AADDeviceAuthentication.Examples
{
    class Client
    {
        static string GetToken()
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
    }
}

```

### Server Side

```c#

using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Text.Json;
using WPNinjas.AADDeviceAuthentication.Server;
using WPNinjas.AADDeviceAuthentication.Common;

namespace WPNinjas.AADDeviceAuthentication.Examples
{
    class Server
    {
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


            // Validate token, returns true when verified successfully.

            return server.Authenticate(JsonSerializer.Deserialize<DeviceAuthToken>(tokenJson));

        }
    }
}

```

## PowerShell Module
For your convinience the module is published to the [Powershell Gallery](https://www.powershellgallery.com/packages/WPNinjas.AADDeviceAuthentication/).

```powershell
Install-Module -Name WPNinjas.AADDeviceAuthentication

$token = Get-AADDeviceToken -Content "Test123"


```

