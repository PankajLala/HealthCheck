using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HealthCheckService.Entities;
using HealthCheckService.Hub;
using HealthCheckService.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Xunit;

namespace HealthCheckService.Test
{
    public class ReadinessPublisherTest
    {
        private readonly Mock<IHubContext<ServerStatusHub>> mockHubContext;
        private readonly ReadinessPublisher readiNessPublisher;
        private readonly Mock<IServerHostingDataAccess> mockDataAccess;
        private IReadOnlyDictionary<string, HealthReportEntry> entries;
        private readonly TimeSpan totalDuration;
        
        public ReadinessPublisherTest()
        {
            mockHubContext = new Mock<IHubContext<ServerStatusHub>>();

            var clients = new Mock<IHubClients>();
            var clientProxy = new Mock<IClientProxy>();
            clients.Setup(x => x.All).Returns(clientProxy.Object);
            mockHubContext.Setup(mock => mock.Clients).Returns(clients.Object);
            
            var serviceCollection = new ServiceCollection();
            mockDataAccess = new Mock<IServerHostingDataAccess>();
            serviceCollection.AddSingleton<IServerHostingDataAccess>(mockDataAccess.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.SetupGet<IServiceProvider>(s => s.ServiceProvider)
                .Returns(serviceProvider);
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(s => s.CreateScope())
                .Returns(serviceScopeMock.Object);

            
            readiNessPublisher = new ReadinessPublisher(mockHubContext.Object, serviceProvider);

            entries = new Dictionary<string, HealthReportEntry>
            {
                {"service1", new HealthReportEntry(HealthStatus.Degraded,null, new TimeSpan(),null,null)},
                {"service2", new HealthReportEntry(HealthStatus.Degraded,null, new TimeSpan(),null,null)},
                {"service3", new HealthReportEntry(HealthStatus.Degraded,null, new TimeSpan(),null,null)},
            };
            
            totalDuration = new TimeSpan();

           
        }
        
        [Fact]
        public void send_notification_to_registered_client()
        {
            readiNessPublisher.PublishAsync(new HealthReport(entries, totalDuration), CancellationToken.None);
            mockHubContext.Verify(x=>x.Clients.All,Times.Once);
        }
        
        [Fact]
        public void save_servicestates_to_store()
        {
            readiNessPublisher.PublishAsync(new HealthReport(entries, totalDuration), CancellationToken.None);
            mockDataAccess.Verify(x=>x.AddServerStatus(It.IsAny<IEnumerable<ServerStatus>>()),Times.Once);
        }
        
        
        [Fact]
        public void cancelationrequested()
        {
            readiNessPublisher.PublishAsync(new HealthReport(entries, totalDuration), new CancellationToken(true));
            mockDataAccess.Verify(x=>x.AddServerStatus(It.IsAny<IEnumerable<ServerStatus>>()),Times.Never);
            mockHubContext.Verify(x=>x.Clients.All,Times.Never);
        }
    }
}