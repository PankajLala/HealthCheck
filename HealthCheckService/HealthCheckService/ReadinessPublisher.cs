using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using HealthCheckService.Entities;
using HealthCheckService.Hub;
using HealthCheckService.Repositories;

namespace HealthCheckService
{
    public class ReadinessPublisher : IHealthCheckPublisher
    {


        /// <summary>
        /// The latest health report which got published
        /// </summary>
        public static IEnumerable<ServerStatus> LatestServerStatus { get; set; }
        private IHubContext<ServerStatusHub> _hubContext;
        public IServiceProvider _services { get; }
        private IServerHostingDataAccess _serverHostingDataAccess;

        public ReadinessPublisher(IHubContext<ServerStatusHub> hubContext, IServiceProvider services )
        {
            _hubContext = hubContext;
            _services = services;
        }

        public async Task PublishAsync(HealthReport report,
            CancellationToken cancellationToken)
        {
            

            var serverStatuses = report.Entries.Select(service => new ServerStatus {Date = DateTimeOffset.Now, ServerName = service.Key, HealthStatus = service.Value.Status.ToString()}).ToList();

            LatestServerStatus = serverStatuses;
            cancellationToken.ThrowIfCancellationRequested();

            await _hubContext.Clients.All.SendAsync("serverstatusdata", serverStatuses);

            using var scope = _services.CreateScope();
            _serverHostingDataAccess = scope.ServiceProvider.GetRequiredService<IServerHostingDataAccess>();
            _serverHostingDataAccess.AddServerStatus(serverStatuses);
            
        }
    }
}
