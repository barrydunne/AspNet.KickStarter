using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AspNet.KickStarter
{
    /// <summary>
    /// Simplifies the repetitive  steps in creating a WebApplication for an AspNet API.
    /// </summary>
    public class ApiBuilder
    {
        private bool _withSerilog;
        private Action<string>? _withSerilogDebugOutput;
        private bool _withSwagger;
        private bool _withSwaggerOnlyInDevelopment;
        private Action<WebApplicationBuilder>? _withServices;
        private Action<WebApplication>? _withEndpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBuilder"/> class.
        /// </summary>
        public ApiBuilder()
        {
            _withSerilog = false;
            _withSerilogDebugOutput = null;
            _withSwagger = false;
            _withSwaggerOnlyInDevelopment = false;
            _withServices = null;
            _withEndpoints = null;
        }

        /// <summary>
        /// Use Serilog for logging in the API.
        /// </summary>
        /// <param name="debugOutput">An optional action to invoke with Serilog self-log messages.</param>
        /// <returns>The current instance.</returns>
        public ApiBuilder WithSerilog(Action<string>? debugOutput = null)
        {
            _withSerilog = true;
            _withSerilogDebugOutput = debugOutput;
            return this;
        }

        /// <summary>
        /// Enable Swagger/OpenApi in the API.
        /// </summary>
        /// <param name="onlyInDevelopment">Whether to only include swagger page when running in development mode.</param>
        /// <returns>The current instance.</returns>
        public ApiBuilder WithSwagger(bool onlyInDevelopment = false)
        {
            _withSwagger = true;
            _withSwaggerOnlyInDevelopment = onlyInDevelopment;
            return this;
        }

        /// <summary>
        /// Provide the action that will register the services for the API.
        /// </summary>
        /// <param name="registerServices">The action that will register the services for the API.</param>
        /// <returns>The current instance.</returns>
        public ApiBuilder WithServices(Action<WebApplicationBuilder> registerServices)
        {
            _withServices = registerServices;
            return this;
        }

        /// <summary>
        /// Provide the action that will map the endpoints for the API.
        /// </summary>
        /// <param name="mapEndpoints">The action that will map the endpoints for the API.</param>
        /// <returns>The current instance.</returns>
        public ApiBuilder WithEndpoints(Action<WebApplication> mapEndpoints)
        {
            _withEndpoints = mapEndpoints;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="WebApplication"/>.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The configured <see cref="WebApplication"/>.</returns>
        public WebApplication Build(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (_withSerilog)
            {
                if (_withSerilogDebugOutput != null)
                    Serilog.Debugging.SelfLog.Enable(_ => _withSerilogDebugOutput.Invoke(_));
                builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration));
            }

            if (_withSwagger)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }

            _withServices?.Invoke(builder);

            var app = builder.Build();

            if (_withSwagger && _withSwaggerOnlyInDevelopment)
                _withSwagger = !app.Environment.IsDevelopment();
            if (_withSwagger)
            {
                app.UseSwagger(_ => _.RouteTemplate = "{documentname}/swagger.json");
                app.UseSwaggerUI(_ => _.RoutePrefix = "");
            }

            _withEndpoints?.Invoke(app);

            return app;
        }
    }
}
