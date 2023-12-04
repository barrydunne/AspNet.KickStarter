using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Prometheus.HttpMetrics;

namespace AspNet.KickStarter.Tests.AddIns;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInMetricsTests
{
    private readonly Fixture _fixture;

    public AddInMetricsTests() => _fixture = new Fixture();

    [Test]
    public void AddInMetrics_adds_services()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetServices(serviceCollection);
        var sut = _fixture.Create<AddInMetrics>();
        sut.Configure(builder);
        var registeredFactoryAssemblyName = serviceCollection.SingleOrDefault()?.ImplementationFactory?.Method?.DeclaringType?.Assembly?.GetName()?.Name;
        Assert.That(registeredFactoryAssemblyName, Is.EqualTo("Prometheus.AspNetCore"));
    }

    [Test]
    public void AddInMetrics_adds_middleware()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        var sut = new AddInMetrics();
        sut.Configure(app);
        var applicationBuilder = PrivateAccessors.GetApplicationBuilder(app);
        var components = PrivateAccessors.GetComponents(applicationBuilder);
        Assert.That(components, Is.Not.Empty);
    }

    [Test]
    public void WithMetrics_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithMetrics();
        var addin = apiBuilder.GetAddIn<AddInMetrics>();
        Assert.That(addin?.Port, Is.EqualTo(8081), "Incorrect Port");
        Assert.That(addin?.PortOptionsCallback, Is.Null, "Incorrect PortOptionsCallback");
        Assert.That(addin?.ExporterOptions, Is.Null, "Incorrect ExporterOptions");
        Assert.That(addin?.MeterAdapterOptions, Is.Null, "Incorrect MeterAdapterOptions");
    }

    [Test]
    public void WithMetrics_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithMetrics(1234, ListenerOptionsCallback, MetricsExporterOptions, MetricsMeterAdapterOptions);
        var addin = apiBuilder.GetAddIn<AddInMetrics>();
        Assert.That(addin?.Port, Is.EqualTo(1234), "Incorrect Port");
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(addin?.PortOptionsCallback, Is.EqualTo(ListenerOptionsCallback), "Incorrect PortOptionsCallback");
        Assert.That(addin?.ExporterOptions, Is.EqualTo(MetricsExporterOptions), "Incorrect ExporterOptions");
        Assert.That(addin?.MeterAdapterOptions, Is.EqualTo(MetricsMeterAdapterOptions), "Incorrect MeterAdapterOptions");
#pragma warning restore CS8974 // Converting method group to non-delegate type
        static void ListenerOptionsCallback(KestrelMetricServerOptions options) => throw new NotImplementedException();
        static void MetricsExporterOptions(HttpMiddlewareExporterOptions options) => throw new NotImplementedException();
        static void MetricsMeterAdapterOptions(MeterAdapterOptions options) => throw new NotImplementedException();
    }

    [Test]
    public void AddInMetrics_invokes_ExporterOptions()
    {
        bool? invoked = null;
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        var sut = new AddInMetrics { ExporterOptions = ExporterOptions };
        sut.Configure(app);
        Assert.That(invoked, Is.True);
        void ExporterOptions(HttpMiddlewareExporterOptions options) => invoked = true;
    }

    [Test]
    public void AddInMetrics_OptionsCallback_sets_port()
    {
        var port = _fixture.Create<ushort>();
        var options = new KestrelMetricServerOptions();
        var addIn = new AddInMetrics { Port = port };
        addIn.OptionsCallback(options);
        Assert.That(options.Port, Is.EqualTo(port));
    }

    [Test]
    public void AddInMetrics_OptionsCallback_invokes_callback()
    {
        var port = _fixture.Create<ushort>();
        var options = new KestrelMetricServerOptions();
        var addIn = new AddInMetrics { PortOptionsCallback = PortOptionsCallback };
        KestrelMetricServerOptions? invoked = null;
        addIn.OptionsCallback(options);
        Assert.That(invoked, Is.EqualTo(options));
        void PortOptionsCallback(KestrelMetricServerOptions options) => invoked = options;
    }
}
