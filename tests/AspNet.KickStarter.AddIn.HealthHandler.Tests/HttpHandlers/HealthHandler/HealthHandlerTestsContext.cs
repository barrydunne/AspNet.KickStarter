using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Diagnostics.Metrics;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace AspNet.KickStarter.AddIn.HealthHandler.Tests.HttpHandlers.HealthHandler;

internal class HealthHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockFileSystem _mockFileSystem;
    private readonly IMeterFactory _mockMeterFactory;
    private readonly ILogger<KickStarter.HttpHandlers.HealthHandler> _mockLogger;
    private readonly Assembly _mockEntryAssembly;

    internal string AssemblyVersion { get; }

    internal KickStarter.HttpHandlers.HealthHandler Sut { get; private set; }

    public HealthHandlerTestsContext()
    {
        _fixture = new();
        _mockFileSystem = new();

        _mockMeterFactory = Substitute.For<IMeterFactory>();
        _mockMeterFactory.Create(Arg.Any<MeterOptions>()).Returns(callInfo => new Meter(callInfo.ArgAt<MeterOptions>(0).Name));
        _mockLogger = Substitute.For<ILogger<KickStarter.HttpHandlers.HealthHandler>>();

        var assemblyVersion = new Version(_fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>());
        var assemblyName = new AssemblyName($"Example, Version={assemblyVersion.ToString(4)}, Culture=en-IE, PublicKeyToken=null");

        _mockEntryAssembly = Substitute.For<Assembly>();
        _mockEntryAssembly.Location.Returns(_ => Path.GetTempPath());
        _mockEntryAssembly.GetName().Returns(assemblyName);

        AssemblyVersion = assemblyVersion.ToString(3);
        Sut = new(_mockFileSystem, _mockMeterFactory, _mockLogger, _mockEntryAssembly);
    }

    internal HealthHandlerTestsContext WithFileVersion(string version)
    {
        _mockFileSystem.AddFile(Sut.VersionFilePath, new MockFileData(version));
        return this;
    }

    internal HealthHandlerTestsContext WithGetVersionAsyncException()
    {
        _mockEntryAssembly.GetName().Throws(new ApplicationException());
        return this;
    }

    internal HealthHandlerTestsContext WithoutAssembly()
    {
        Sut = new(_mockFileSystem, _mockMeterFactory, _mockLogger, null);
        return this;
    }
}
