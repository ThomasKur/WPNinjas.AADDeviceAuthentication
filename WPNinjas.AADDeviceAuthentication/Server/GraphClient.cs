﻿using System;
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
            var result = await _graphServiceClient.Devices.Request().Filter("deviceId eq '" + id.ToString()+"'").Top(1).GetAsync();
            return result.CurrentPage.First();
        }
    }
}
