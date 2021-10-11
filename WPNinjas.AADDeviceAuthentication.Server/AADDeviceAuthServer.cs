using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using WPNinjas.AADDeviceAuthentication.Common;

namespace WPNinjas.AADDeviceAuthentication
{
    public class AADDeviceAuthServer
    {
        private GraphServiceClient _graphServiceClient;
        public AADDeviceAuthServer(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public Boolean Authenticate(DeviceAuthToken token)
        {
            byte[] pubKey = Convert.FromBase64String(token.PublicKey);

            var device = _graphServiceClient.Devices[token.DeviceId.ToString()];

            return CryptoService.Verify(pubKey,token.CertThumbprint,)
        }
    }
}
