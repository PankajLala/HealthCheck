using System;

namespace HealthCheckService.Entities
{
    public class ServerStatus
    {
        public DateTimeOffset Date { get; set; }

        public string ServerName { get; set; }
        
        public string HealthStatus { get; set; }
    }
}