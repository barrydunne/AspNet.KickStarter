using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.HttpMetrics;
using Serilog;

namespace AspNet.KickStarter
{
    /// <summary>
    /// Simplifies the repetitive  steps in creating a WebApplication for an AspNet API.
    /// </summary>
    public class ApiBuilder
    {
        // Unless defined otherwise by the client application, the histogram buckets will use a custom set of values.
        // This is to override the Prometheus default histogram buckets which are Histogram.ExponentialBuckets(0.01, 2, 25)
        // resulting in values 0.01, 0.02, 0.04, 0.08, 0.16, ... 83886.08, 167772.16
        // Those default values are considered too high for millisecond timings which are assumed to be the normal metrics recorded by the applications that use this class.
        // If custom values are required then they can be configured by passing a metricsMeterAdapterOptions action to WithMetrics().
        private static readonly Action<MeterAdapterOptions> _metricsMeterAdapterDefaultOptions = (_) => _.ResolveHistogramBuckets = (_) => new[] { 0.001, 0.002, 0.003, 0.004, 0.005, 0.006, 0.007, 0.008, 0.009, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };

        private bool _withSerilog;
        private Action<string>? _withSerilogDebugOutput;
        private bool _withSwagger;
        private bool _withSwaggerBearerToken;
        private bool _withSwaggerOnlyInDevelopment;
        private Action<WebApplicationBuilder>? _withServices;
        private bool _withFluentValidation;
        private Type? _fluentValidatorType;
        private ServiceLifetime _fluentValidatorLifetime;
        private Func<AssemblyScanner.AssemblyScanResult, bool>? _fluentValidatorFilter;
        private bool _fluentValidatorIncludeInternalTypes;
        private bool _withMetrics;
        private ushort _metricsPort;
        private Action<KestrelMetricServerOptions>? _metricsPortOptionsCallback;
        private Action<HttpMiddlewareExporterOptions>? _metricsExporterOptions;
        private Action<MeterAdapterOptions> _metricsMeterAdapterOptions;
        private Action<WebApplicationBuilder>? _withAdditionalConfiguration;
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
            _withSwaggerBearerToken = false;
            _withServices = null;
            _fluentValidatorType = null;
            _fluentValidatorLifetime = ServiceLifetime.Scoped;
            _fluentValidatorFilter = null;
            _fluentValidatorIncludeInternalTypes = true;
            _withFluentValidation = false;
            _withMetrics = false;
            _metricsPortOptionsCallback = null;
            _metricsExporterOptions = null;
            _metricsMeterAdapterOptions = _metricsMeterAdapterDefaultOptions;
            _withAdditionalConfiguration = null;
            _withEndpoints = null;
        }

        /// <summary>
        /// Use Serilog for logging in the API.
        /// </summary>
        /// <param name="debugOutput">An optional action to invoke with Serilog self-log messages.</param>
        /// <returns>The current builder.</returns>
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
        /// <param name="useBearerToken">Whether to include Bearer token authorization.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithSwagger(bool onlyInDevelopment = false, bool useBearerToken = false)
        {
            _withSwagger = true;
            _withSwaggerOnlyInDevelopment = onlyInDevelopment;
            _withSwaggerBearerToken = useBearerToken;
            return this;
        }

        /// <summary>
        /// Provide the action that will register the services for the API.
        /// </summary>
        /// <param name="registerServices">The action that will register the services for the API.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithServices(Action<WebApplicationBuilder> registerServices)
        {
            _withServices = registerServices;
            return this;
        }

        /// <summary>
        /// Automatically register all validators in a specific assembly.
        /// </summary>
        /// <typeparam name="TValidator">A validator that is from the assembly to scan.</typeparam>
        /// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications).</param>
        /// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
        /// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithFluentValidationFromAssemblyContaining<TValidator>(ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null, bool includeInternalTypes = true)
        {
            _withFluentValidation = true;
            _fluentValidatorType = typeof(TValidator);
            _fluentValidatorLifetime = lifetime;
            _fluentValidatorFilter = filter;
            _fluentValidatorIncludeInternalTypes = includeInternalTypes;
            return this;
        }

        /// <summary>
        /// Run a Prometheus metrics exporter on a separate port.
        /// </summary>
        /// <param name="metricsPort">The port the metrics HTTP listener should use. The default is 8081.</param>
        /// <param name="listenerOptionsCallback">The optional action for additional metrics HTTP listener configuration.</param>
        /// <param name="metricsExporterOptions">The optional action for additional metrics export configuration.</param>
        /// <param name="metricsMeterAdapterOptions">The optional action for additional metrics adapter configuration such as using a custom ResolveHistogramBuckets function.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithMetrics(ushort metricsPort = 8081, Action<KestrelMetricServerOptions>? listenerOptionsCallback = null, Action<HttpMiddlewareExporterOptions>? metricsExporterOptions = null, Action<MeterAdapterOptions>? metricsMeterAdapterOptions = null)
        {
            _withMetrics = true;
            _metricsPort = metricsPort;
            _metricsPortOptionsCallback = listenerOptionsCallback;
            _metricsExporterOptions = metricsExporterOptions;
            _metricsMeterAdapterOptions = metricsMeterAdapterOptions ?? _metricsMeterAdapterDefaultOptions;
            return this;
        }

        /// <summary>
        /// Provide the action that will perform custom configuration for the API.
        /// </summary>
        /// <param name="additionalConfiguration">The action that will perform custom configuration for the API.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithAdditionalConfiguration(Action<WebApplicationBuilder> additionalConfiguration)
        {
            _withAdditionalConfiguration = additionalConfiguration;
            return this;
        }

        /// <summary>
        /// Provide the action that will map the endpoints for the API.
        /// </summary>
        /// <param name="mapEndpoints">The action that will map the endpoints for the API.</param>
        /// <returns>The current builder.</returns>
        public ApiBuilder WithEndpoints(Action<WebApplication> mapEndpoints)
        {
            _withEndpoints = mapEndpoints;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="WebApplication"/>.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The configured <see cref="WebApplication"/> ready to run.</returns>
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
                builder.Services.AddSwaggerGen(_ =>
                {
                    if (_withSwaggerBearerToken)
                    {
                        var scheme = new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Description = "Enter your access token below without the 'Bearer ' prefix.",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.Http, // Does not require the "Bearer " prefix to be used, unlike ApiKey.
                            Scheme = "Bearer"
                        };
                        _.AddSecurityDefinition("Bearer", scheme);
                        _.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Name = "Bearer",
                                    In = ParameterLocation.Header,
                                    Reference = new OpenApiReference
                                    {
                                        Id = "Bearer",
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                Array.Empty<string>()
                            }
                        });
                    }
                });
            }

            _withServices?.Invoke(builder);

            if (_withFluentValidation)
                builder.Services.AddValidatorsFromAssemblyContaining(_fluentValidatorType, _fluentValidatorLifetime, _fluentValidatorFilter, _fluentValidatorIncludeInternalTypes);

            if (_withMetrics)
            {
                Metrics.ConfigureMeterAdapter(_metricsMeterAdapterOptions);
                builder.Services.AddMetricServer(options =>
                {
                    options.Port = _metricsPort;
                    _metricsPortOptionsCallback?.Invoke(options);
                });
            }

            _withAdditionalConfiguration?.Invoke(builder);

            var app = builder.Build();

            if (_withSwagger && _withSwaggerOnlyInDevelopment)
                _withSwagger = !app.Environment.IsDevelopment();
            if (_withSwagger)
            {
                app.UseSwagger(_ => _.RouteTemplate = "{documentname}/swagger.json");
                app.UseSwaggerUI(_ => _.RoutePrefix = "");
            }

            _withEndpoints?.Invoke(app);

            if (_withMetrics)
            {
                if (_metricsExporterOptions is null)
                    app.UseHttpMetrics();
                else
                    app.UseHttpMetrics(_metricsExporterOptions);
            }

            return app;
        }
    }
}
