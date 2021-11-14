using Microsoft.AspNetCore.Mvc;
using HealthCheckService.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthCheckService.Entities;

namespace HealthCheckService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerStatusController : ControllerBase
    {
        private  IServerHostingDataAccess ServerHostingDataAccess { get;  }
        public ServerStatusController(IServerHostingDataAccess serverHostingDataAccess)
        {
            ServerHostingDataAccess = serverHostingDataAccess;
        }

        public async Task<IEnumerable<ServerStatus>> Get()
        {
            return await ServerHostingDataAccess.GetServerStatus();
        }
    }
}