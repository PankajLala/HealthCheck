using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProfileService
{
    internal class RedisServiceCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetRandomHealthSwitch ? HealthCheckResult.Healthy("ProfileService is healthy") : HealthCheckResult.Unhealthy("ProfileService is unhealthy"));
        }

        private bool GetRandomHealthSwitch
        {
            get
            {
                var rnd = new Random().Next(1, 5);
                return rnd % 2 != 0;
            }
        }

    }
}