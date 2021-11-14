using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HealthCheckService.Entities;

namespace HealthCheckService.Repositories
{
    public class ServerHostingDataAccess: IServerHostingDataAccess
    {
        private readonly DapperContext _context;
        
        public ServerHostingDataAccess(DapperContext context)
        {
            _context = context;
        }

        public void AddServerStatus(IEnumerable<ServerStatus> serverStatuses)
        {
            using var connection = _context.CreateConnection();

            foreach (var item in serverStatuses)
            {
                var processQuery = "INSERT INTO ServerStatus VALUES (@ServerName, @HealthStatus, @Date)";
                connection.Execute(processQuery, new { ServerName = item.ServerName, HealthStatus = item.HealthStatus, Date = item.Date });
            }
        }

        public  async Task<IEnumerable<ServerStatus>> GetServerStatus()
        {
            var query = "SELECT Id, ServerName, HealthStatus, Cast(Date as datetime) as Date  FROM ServerStatus Where Date > DATEADD(MINUTE, -15, GETDATE()) Order By Date Desc";
            using var connection = _context.CreateConnection();
            var servers = await connection.QueryAsync<ServerStatus>(query);
            return servers.ToList();
        }

    }
}