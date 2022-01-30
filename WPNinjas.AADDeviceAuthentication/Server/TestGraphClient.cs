using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace WPNinjas.AADDeviceAuthentication.Server
{
    public class TestGraphClient : IGraphClient
    {
        private Device _device;
        public TestGraphClient(Device device)
        {
            _device = device;
        }
        public async Task<Device> GetDevice(Guid id)
        {
            return _device;
        }
    }
}
