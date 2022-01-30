using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace WPNinjas.AADDeviceAuthentication.Server
{
    public interface IGraphClient
    {
        Task<Device> GetDevice(Guid id);
        
    }
}
