using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WPNinjas.AADDeviceAuthentication.Common;
using WPNinjas.Dsregcmd;

namespace WPNinjas.AADDeviceAuthentication
{
    public class AADDeviceAuthClient
    {
        private DsregcmdResult DsregcmdStatus;
        private X509Certificate2 AadCert;
        public AADDeviceAuthClient()
        {
            DsregcmdStatus = Dsregcmd.Dsregcmd.GetInfo();
            if (DsregcmdStatus.DeviceId == default)
            {
                throw new Exception("No Azure AD device id found. Check with dsregcmd if the device is joined to domain.");
            }
            System.Console.WriteLine("Load AAD Join Info");
            AadCert = GetMostReliableCert(DsregcmdStatus);
            if (AadCert == null)
            {
                throw new Exception("No certificate found which can be used for authentication.");
            }
        }
        public DeviceAuthToken GetToken(string signaturestring)
        {
            byte[] pubkey = AadCert.GetPublicKey();
            string signature = CryptoService.Sign(AadCert.GetRSAPrivateKey(), signaturestring);
            string bPubKey = Convert.ToBase64String(pubkey);
            
            return new DeviceAuthToken { CertThumbprint = AadCert.Thumbprint, Signature = signature, PublicKey = bPubKey, DeviceId = DsregcmdStatus.DeviceId };
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
