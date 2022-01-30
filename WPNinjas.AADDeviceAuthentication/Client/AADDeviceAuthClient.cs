using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using WPNinjas.AADDeviceAuthentication.Common;
using WPNinjas.Dsregcmd;

namespace WPNinjas.AADDeviceAuthentication.Client
{
    public class AADDeviceAuthClient
    {
        private Guid DeviceId;
        private X509Certificate2 AadCert;
        public AADDeviceAuthClient()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (!isElevated)
                throw new Exception("Not executed as Administrator. Please start PowerShell with Administrative Privileges.");

            var DsregcmdStatus = Dsregcmd.Dsregcmd.GetInfo();

            Initialize(GetMostReliableCert(DsregcmdStatus), DsregcmdStatus.DeviceId);
            
        }
        public AADDeviceAuthClient(X509Certificate2 aadCert,Guid deviceId)
        {
            Initialize(aadCert, deviceId);
        }
        private void Initialize(X509Certificate2 aadCert, Guid deviceId)
        {
            AadCert = aadCert;
            DeviceId = deviceId;
            if (DeviceId == default)
            {
                throw new Exception("No Azure AD device id found. Check with dsregcmd if the device is joined to domain.");
            }

            if (AadCert == null)
            {
                throw new Exception("No certificate found which can be used for authentication.");
            }
        }
        public DeviceAuthToken GetToken(string signaturestring)
        {
            byte[] pubkey = AadCert.GetPublicKey();
            string signature = CryptoService.Sign(AadCert.GetRSAPrivateKey(), signaturestring+ DeviceId.ToString());
            string bPubKey = Convert.ToBase64String(pubkey);
            
            return new DeviceAuthToken { CertThumbprint = AadCert.Thumbprint, Signature = signature, PublicKey = bPubKey, DeviceId = DeviceId, Content = signaturestring };
        }

        public string Decrypt(string EncryptedText)
        {
            return CryptoService.Decrypt(EncryptedText,AadCert.GetRSAPrivateKey());
        }
        private X509Certificate2 GetMostReliableCert(DsregcmdResult dsreg)
        {

            X509Certificate2 cert = null;
            if (!(dsreg.CertInfo is null) && dsreg.CertInfo.Count > 0)
            {
                try
                {
                    cert = dsreg.CertInfo.Where(x => x.HasPrivateKey && x.NotAfter > DateTime.Now && x.NotBefore < DateTime.Now && x.Issuer.Contains("MS-Organization-Access")).OrderByDescending(x => x.NotBefore).First();
                }
                catch
                {
                    throw new Exception("Error selecting the correct AAD device certificate or authentication.");
                }
                if ((cert is null))
                {
                    throw new Exception("No valid AAD device certificate found for authentication.");
                }
                else
                {
                    return cert;
                }

            }
            else
            {
                throw new Exception("No valid AAD device certificate found for authentication.");
            }
        }
    }
}
