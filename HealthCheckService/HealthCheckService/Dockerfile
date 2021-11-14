#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["HealthCheckService/HealthCheckService.csproj", "HealthCheckService/"]
RUN dotnet restore "HealthCheckService/HealthCheckService.csproj"
COPY . .
WORKDIR "/src/HealthCheckService"
RUN dotnet build "HealthCheckService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthCheckService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthCheckService.dll"]
