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
HealthCheckService is a monitoring service which constantly polls the dependent services. For the purpose of the challange I have created 
	a service which has dependency on Sql Server - whose health status is returned as part of health checks
<br />
Dependent Service: 
<br />
	ProfileService: having dependency on Redis thus AspNetCore.HealthChecks.Redis package should be used when configuring the health check
					 For the purpose of challange and to simulate the failure scenario I've used a custom implementation to modify behaviour of /health endpoint to randomly return different health status
	

Adding AspNetCore.Diagnostics.HealthChecks provides a /health endpoint which returns the status of the configured health checks. An expected aspect is that the client monitoring
UI will regularly call to get health status. 

Instead of adding responsibility on regularly polling the /health endpoint - I've taken the approach of pushing the health check notification to the dashboard application. 

To persist the health check results for dependent service I've used simple repository layer which is using Dapper for data persistence to a sql store.
ServerStatusController implements the route to get the historical health check data - currently it return the last 15 min of the activity.

For persisting the health check data and pushing it to subscribed client application - I've provided the implementation of IHealthCheckPublisher with the peroid set to 10 sec.

I've created a ServerStatus hub which allowed interested client applications to subscribe to hosted SignalR service to receive the health check notifications.

Part 2: 
<br />
*Thoughts:*
There is already an option in  AspNetCore.HealthChecks.UI to have a UI which shows the data for dependent service - which offers a real time (with configurable option delay) to show the health status.
For the challange I took the opportunity to showcase how to use the push model for reporting health status - I've used angluar application (using cli) to showcase same, however same approach 
should work for other JS SPA frameworks.
<br /><br />

*Implementation:*
I've created a singalr service to set up communication with the hub hosted in the HealthCheckService - I wanted to implement the retry setup if the UI fails to connect with signalr hub
(hubconnection's catch with exponential backoff retry with a sealing of max retry) - however same is not the part of the submitted challange

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