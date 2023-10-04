using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Reflection;

namespace AspNet.KickStarter.HttpHandlers
{
    /// <summary>
    /// Provides standard health check functionality.
    /// </summary>
    public class HealthHandler
    {
        private const string _versionFile = "version.txt";

        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly Assembly? _entryAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthHandler"/> class.
        /// </summary>
        /// <param name="fileSystem">The testable file system wrapper.</param>
        /// <param name="logger">The logger to write to.</param>
        /// <param name="entryAssembly">The application entry assembly to read the version from.</param>
        public HealthHandler(IFileSystem fileSystem, ILogger<HealthHandler> logger, Assembly? entryAssembly = null)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _entryAssembly = entryAssembly ?? Assembly.GetEntryAssembly();
            VersionFilePath = Path.Combine(Path.GetDirectoryName(_entryAssembly?.Location ?? string.Empty) ?? ".", _versionFile);
        }

        /// <summary>
        /// Gets the file path that will be checked for a text file containing the version.
        /// </summary>
        public string VersionFilePath { get; }

        /// <summary>
        /// Get the status of the API.
        /// </summary>
        /// <returns>OK "GOOD".</returns>
        public IResult GetStatus() => Results.Ok("GOOD");

        /// <summary>
        /// Get the version of the API.
        /// This can come from the content of a file named version.txt located in the same directory as the entry assembly, or if not found then from the entry assembly version.
        /// </summary>
        /// <returns>The API version number as a string.</returns>
        public async Task<IResult> GetVersionAsync()
        {
            var version = "UNKNOWN";
            try
            {
                _logger.LogDebug("Checking for version file at {VersionPath}", VersionFilePath);
                if (_fileSystem.File.Exists(VersionFilePath))
                {
                    _logger.LogDebug("Found version file at {VersionFilePath}", VersionFilePath);
                    version = await _fileSystem.File.ReadAllTextAsync(VersionFilePath);
                }
                else
                {
                    var entryAssemblyVersion = _entryAssembly?.GetName()?.Version?.ToString(3);
                    if (entryAssemblyVersion is not null)
                        version = entryAssemblyVersion;
                }
                _logger.LogInformation("GetVersionQuery handler. Found version: {Version}", version);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to find version");
            }
            return Results.Ok(version.Trim());
        }
    }
}
