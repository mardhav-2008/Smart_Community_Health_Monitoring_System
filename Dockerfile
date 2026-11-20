# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Smart_Community_Health_Monitoring_System/*.csproj ./app/
WORKDIR /src/app

RUN dotnet restore

COPY src/Smart_Community_Health_Monitoring_System/. ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "Smart_Community_Health_Monitoring_System.dll"]
