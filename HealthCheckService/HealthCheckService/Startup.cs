using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HealthCheckService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using HealthCheckService.Entities;
using HealthCheckService.Hub;
using HealthCheckService.Repositories;

namespace HealthCheckService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<IServerHostingDataAccess, ServerHostingDataAccess>();
            ConfigureCors(services);
            services.AddSignalR();
            services.AddControllers();
            services.AddHealthChecks()
                .AddSqlServer(Configuration["ConnectionStrings:SqlConnection"],null,"HealthCheckServiceSqlDB",HealthStatus.Unhealthy)
                .AddUrlGroup(new Uri("https://profileservicedeploy.azurewebsites.net/health"), "ProflieService", HealthStatus.Unhealthy);



            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Period = TimeSpan.FromSeconds(10);
               
            });

            services.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();

        }

        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins(new[] { "http://localhost:4200",
                        "https://serverstatusui.azurewebsites.net" })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

         
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                Predicate = _ => false,
                ResponseWriter = (context, _) => WriteHealthCheckUIResponse(context, ReadinessPublisher.LatestServerStatus)
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ServerStatusHub>("/serverstatushub");
            });
        }

        private Task WriteHealthCheckUIResponse(HttpContext context, IEnumerable<ServerStatus> latestServerStatus)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            return Task.FromResult(latestServerStatus);
        }
    }
}