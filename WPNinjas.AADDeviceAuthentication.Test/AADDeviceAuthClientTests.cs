using WPNinjas.AADDeviceAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using WPNinjas.AADDeviceAuthentication.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Graph;
using WPNinjas.AADDeviceAuthentication.Server;
using Moq;

namespace WPNinjas.AADDeviceAuthentication.Tests
{
    [TestClass()]
    public class AADDeviceAuthClientTests
    {
        private AADDeviceAuthClient client;
        private Guid guid;
        private X509Certificate2 cert;

        public void Setup()
        {
            cert = new X509Certificate2("TestCert.pfx", "password", X509KeyStorageFlags.MachineKeySet);
            guid = Guid.NewGuid();
            client = new AADDeviceAuthClient(cert,guid);
            
            
        }
        [TestMethod()]
        public void AADDeviceAuthClientTest()
        {
            Setup();
            List<AlternativeSecurityId> SecIds = new List<AlternativeSecurityId>();

            SecIds.Add(new AlternativeSecurityId() { Key = System.Text.Encoding.UTF8.GetBytes(CryptoService.GetVerifyValue(cert.GetPublicKey(), cert.Thumbprint)) });

            Device device = new Device()
            {
                DeviceId = guid.ToString(),
                AlternativeSecurityIds = SecIds
            };

            TestGraphClient graphClient = new TestGraphClient(device);
            AADDeviceAuthServer server = new AADDeviceAuthServer(graphClient);

            DeviceAuthToken token = client.GetToken("Test123");

            Assert.AreEqual(token.Content, "Test123");
            Assert.AreEqual(token.DeviceId, guid);
            var result = server.Authenticate(token);
            result.Wait();
            
            Assert.IsTrue(result.Result);
        }
        [TestMethod()]
        public void AADDeviceAuthClientTestFailed()
        {
            Setup();
            List<AlternativeSecurityId> SecIds = new List<AlternativeSecurityId>();

            SecIds.Add(new AlternativeSecurityId() { Key = System.Text.Encoding.UTF8.GetBytes(CryptoService.GetVerifyValue(cert.GetPublicKey(), cert.Thumbprint.Replace(cert.Thumbprint.Substring(0,1),"2"))) });

            Device device = new Device()
            {
                DeviceId = guid.ToString(),
                AlternativeSecurityIds = SecIds
            };

            TestGraphClient graphClient = new TestGraphClient(device);
            AADDeviceAuthServer server = new AADDeviceAuthServer(graphClient);

            DeviceAuthToken token = client.GetToken("Test123");

            Assert.AreEqual(token.Content, "Test123");
            Assert.AreEqual(token.DeviceId, guid);
            var result = server.Authenticate(token);
            result.Wait();

            Assert.IsFalse(result.Result);
        }

    }
}