using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace AspNet.KickStarter.Tests.HttpHandlers.HealthHandler
{
    internal class HealthHandlerTestsContext
    {
        private readonly Fixture _fixture;
        private readonly MockFileSystem _mockFileSystem;
        private readonly Mock<ILogger<AspNet.KickStarter.HttpHandlers.HealthHandler>> _mockLogger;
        private readonly Mock<Assembly> _mockEntryAssembly;

        internal string AssemblyVersion { get; }

        internal AspNet.KickStarter.HttpHandlers.HealthHandler Sut { get; }

        public HealthHandlerTestsContext()
        {
            _fixture = new();
            _mockFileSystem = new();
            _mockLogger = new();

            var assemblyVersion = new Version(_fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>());
            var assemblyName = new AssemblyName($"Example, Version={assemblyVersion.ToString(4)}, Culture=en-IE, PublicKeyToken=null");

            _mockEntryAssembly = new();
            _mockEntryAssembly.Setup(_ => _.Location).Returns(() => Path.GetTempPath());
            _mockEntryAssembly.Setup(_ => _.GetName()).Returns(() => assemblyName);

            AssemblyVersion = assemblyVersion.ToString(3);
            Sut = new(_mockFileSystem, _mockLogger.Object, _mockEntryAssembly.Object);
        }

        internal HealthHandlerTestsContext WithFileVersion(string version)
        {
            _mockFileSystem.AddFile(Sut.VersionFilePath, new MockFileData(version));
            return this;
        }

        internal HealthHandlerTestsContext WithGetVersionAsyncException()
        {
            _mockEntryAssembly.Setup(_ => _.GetName()).Throws(() => new ApplicationException());
            return this;
        }
    }
}
