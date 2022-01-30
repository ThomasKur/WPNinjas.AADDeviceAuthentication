using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WPNinjas.AADDeviceAuthentication.Common;

namespace WPNinjas.AADDeviceAuthentication.Server
{
    public class AADDeviceAuthServer
    {
        private IGraphClient _graphServiceClient;
        public AADDeviceAuthServer(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = new GraphClient(graphServiceClient);
        }
        public AADDeviceAuthServer(IGraphClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }


        public async Task<Boolean> Authenticate(DeviceAuthToken token)
        {
            byte[] pubKey = Convert.FromBase64String(token.PublicKey);
            var device = await _graphServiceClient.GetDevice(token.DeviceId);

            string validSecid = "";
            foreach (var secId in device.AlternativeSecurityIds)
            {
                var SecIdString = BytesToStringConverted(secId.Key).Replace("\0","");
                if (SecIdString.Contains(token.CertThumbprint))
                {
                    validSecid = SecIdString;
                }
                
            }
            return CryptoService.Verify(pubKey,token.CertThumbprint, validSecid, token.Content + token.DeviceId.ToString(),token.Signature);
        }
        private string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
