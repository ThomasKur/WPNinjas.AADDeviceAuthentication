using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace WPNinjas.AADDeviceAuthentication.Server
{
    public class GraphClient : IGraphClient
    {
        private GraphServiceClient _graphServiceClient;
        public GraphClient(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }
        public async Task<Device> GetDevice(Guid id)
        {
            return await _graphServiceClient.Devices[id.ToString()].Request().GetAsync();
        }
    }
}
