using System;
using System.Collections.Generic;
using System.Text;

namespace WPNinjas.AADDeviceAuthentication.Common
{
    public class DeviceAuthToken
    {
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public Guid DeviceId { get; set; }
        public string CertThumbprint { get; set; }


    }
}
