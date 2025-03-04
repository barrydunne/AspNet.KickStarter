# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY [".editorconfig", "."]
COPY ["src/AspNet.KickStarter/AspNet.KickStarter.csproj", "AspNet.KickStarter/"]
COPY ["src/AspNet.KickStarter.AddIn.AdditionalConfiguration/AspNet.KickStarter.AddIn.AdditionalConfiguration.csproj", "AspNet.KickStarter.AddIn.AdditionalConfiguration/"]
COPY ["src/AspNet.KickStarter.AddIn.ApplicationConfiguration/AspNet.KickStarter.AddIn.ApplicationConfiguration.csproj", "AspNet.KickStarter.AddIn.ApplicationConfiguration/"]
COPY ["src/AspNet.KickStarter.AddIn.Endpoints/AspNet.KickStarter.AddIn.Endpoints.csproj", "AspNet.KickStarter.AddIn.Endpoints/"]
COPY ["src/AspNet.KickStarter.AddIn.FluentValidation/AspNet.KickStarter.AddIn.FluentValidation.csproj", "AspNet.KickStarter.AddIn.FluentValidation/"]
COPY ["src/AspNet.KickStarter.AddIn.HealthHandler/AspNet.KickStarter.AddIn.HealthHandler.csproj", "AspNet.KickStarter.AddIn.HealthHandler/"]
COPY ["src/AspNet.KickStarter.AddIn.Mappings/AspNet.KickStarter.AddIn.Mappings.csproj", "AspNet.KickStarter.AddIn.Mappings/"]
COPY ["src/AspNet.KickStarter.AddIn.OpenTelemetry/AspNet.KickStarter.AddIn.OpenTelemetry.csproj", "AspNet.KickStarter.AddIn.OpenTelemetry/"]
COPY ["src/AspNet.KickStarter.AddIn.Serilog/AspNet.KickStarter.AddIn.Serilog.csproj", "AspNet.KickStarter.AddIn.Serilog/"]
COPY ["src/AspNet.KickStarter.AddIn.Services/AspNet.KickStarter.AddIn.Services.csproj", "AspNet.KickStarter.AddIn.Services/"]
COPY ["src/AspNet.KickStarter.AddIn.Swagger/AspNet.KickStarter.AddIn.Swagger.csproj", "AspNet.KickStarter.AddIn.Swagger/"]
COPY ["src/AspNet.KickStarter.Core/AspNet.KickStarter.Core.csproj", "AspNet.KickStarter.Core/"]
COPY ["src/AspNet.KickStarter.CQRS/AspNet.KickStarter.CQRS.csproj", "AspNet.KickStarter.CQRS/"]
COPY ["src/AspNet.KickStarter.Tracing/AspNet.KickStarter.Tracing.csproj", "AspNet.KickStarter.Tracing/"]
COPY ["demo/AspNet.KickStarter.Demo/AspNet.KickStarter.Demo.csproj", "AspNet.KickStarter.Demo/"]
RUN dotnet restore "AspNet.KickStarter.Demo/AspNet.KickStarter.Demo.csproj"

# Copy the source and build the project
COPY ["src", "."]
COPY ["demo", "."]
RUN dotnet build "AspNet.KickStarter.Demo/AspNet.KickStarter.Demo.csproj" -c Release -o /app/build

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/AspNet.KickStarter.Demo"
RUN dotnet publish "AspNet.KickStarter.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN echo "$(date +'%Y-%m-%d %H:%M:%S')" > /app/publish/version.txt

# Final image from AspNet runtime base image - Skipped for Visual Studio debugging
FROM base AS final
# Install curl as root to provide health check to verify container is running OK
USER root
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -yq curl
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s \
  CMD curl -f http://localhost:8080/health/status || exit 1
# Switch to non-root user to run app
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNet.KickStarter.Demo.dll"]
