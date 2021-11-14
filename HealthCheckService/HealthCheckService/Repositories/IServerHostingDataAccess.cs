using System.Collections.Generic;
using System.Threading.Tasks;
using HealthCheckService.Entities;

namespace HealthCheckService.Repositories
{
    public interface IServerHostingDataAccess
    {
        public void AddServerStatus(IEnumerable<ServerStatus> serverStatuses);

         public Task<IEnumerable<ServerStatus>> GetServerStatus();
    }
}