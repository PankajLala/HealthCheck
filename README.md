Please provide a solution to the problem stated below.
=================================================================
====
Part 1:
You are a developer for a company (ServerHosting Co) and have been tasked with developing a web-service that will provide server status data for another company (XYZ Co) to consume.
You need to provide a functioning web-service to serve the data for consumption by XYZ Co. The data should include a list of servers and whether they were online or offline at a particular 
time (this data can be faked but should be stored & retrieved from some form of storage).
Part 2:
You are a developer for a company (XYZ Co) and you are writing a small web application that shows the status of the servers that host your software. 
You need to integrate with the Server API provided by (ServerHosting Co) to get this information. The information should be presented in an application that can be accessed on a wide variety of devices and screen sizes. Ideally this data should stay as up to date as possible.

**Summary of the solution**

Part 1:
<br />
*Thoughts:*
I have used AspNetCore.Diagnostics.HealthChecks for implementing the web-service that will provide the server status for all the dependent services. 
<br />
<br />
*Implementation:*
HealthCheckService is a monitoring service which constantly provides the health status of dependent to the interested clients. For the purpose of the challange I have configured  the depdendencies on Sql Server DB, another dependent service (ProfileService) and showcased how easy it is to configure health check using the AspNetCore.Diagnostics.HealthChecks package.
	
<br />
Dependent Service: 
<br />
ProfileService: I wanted to showcase, ProfileService having dependency on Redis & thus  AspNetCore.HealthChecks.Redis package should be used when configuring the health checks.
For the purpose of challange and to simulate the intermittent failure scenario I've used a custom implementation to modify behaviour of /health endpoint to randomly return different health status

Adding AspNetCore.Diagnostics.HealthChecks provides a /health endpoint which returns the status of the configured health checks. An expected aspect is that the client monitoring
UI/App can get the health check status by regularly calling the said endpoint.

Instead of adding responsibility solely on client app/UI to regularly poll the /health endpoint - I've taken the approach of HealthCheckService pushing the health check notification to the interested application and at the same time providing the option for getting latest status using /health endpoint.

To persist the health check results for dependent service I've used simple repository layer which is using Dapper for data persistence to a sql store.

I've also introduced ServerStatusController which implements the route to get the historical health check data - currently it return the last 15 min of the activity - at the same time it can be enhanced to return data based on filters, date range, status etc.

Instead of baking a custom Timer to regulaly collate the health status, transform it to domain entity for persistence and send notifications, I hooked it in the IHealthCheckPublisher and configure it timeperiod for 10 Sec from a default of 30 Sec.

For the challange in order to publish the results, I've hosted SignalR within the HealthCheckService, created route for clients to register with service to regularly receive health check via push notifications. For a high performant, scalable and available implementation Az SignalR service should be the preferred option. 

Part 2: 
<br />
*Thoughts:*
There is already an option in  AspNetCore.HealthChecks.UI to have a UI which shows the data for dependent service - which offers a way to show the health status - this is  acheived by UI regularly polling the /health endpoint.

For the challange I took the opportunity to showcase how to use the push model for reporting health status - I've used an Angular SPA  to showcase same, however same approach 
should work for other JS SPA frameworks.
<br /><br />

*Implementation:*
I've created a singalR service to set up communication with the hub hosted in the HealthCheckService - I wanted to implement the retry setup if the UI fails to connect with signalR hub
(intial thoughts are to use Rxjx's .retryWhen ) - however same is not the part of the submitted challange

Along with the realtime health status nofication, UI also using the ServerStatusController to get the data, which can be refreshed using "Refresh" event.

<br /><br />
*Technology stack used*
<br />
Dotnet Core 3.1 
<br />
Dapper
<br />
SingalR (service hosted)
<br />
Sql Server
<br />
Angualr
