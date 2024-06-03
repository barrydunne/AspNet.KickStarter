using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.IO.Abstractions;
using System.Reflection;

namespace AspNet.KickStarter.HttpHandlers;

/// <summary>
/// Provides standard health check functionality.
/// </summary>
public class HealthHandler
{
    private const string _versionFile = "version.txt";

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly Assembly? _entryAssembly;
    private readonly Counter<long> _countStatus;
    private readonly Counter<long> _countVersion;
    private readonly Histogram<double> _versionTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthHandler"/> class.
    /// </summary>
    /// <param name="fileSystem">The testable file system wrapper.</param>
    /// <param name="meterFactory">The factory to use to create a Meter for recording metrics.</param>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="entryAssembly">The application entry assembly to read the version from.</param>
    public HealthHandler(IFileSystem fileSystem, IMeterFactory meterFactory, ILogger<HealthHandler> logger, Assembly? entryAssembly = null)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        _entryAssembly = entryAssembly ?? Assembly.GetEntryAssembly();
        VersionFilePath = GetVersionFilePath();

        var meter = meterFactory.CreateAssemblyMeter();

        var subjectName = nameof(HealthHandler).ToLower();
        _countStatus = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.getstatus.count", description: "The number of get status requests handled.");
        _countVersion = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.getversion.count", description: "The number of get version requests handled.");
        _versionTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.getversion.duration", description: "Time taken to get the version number.", unit: "ms");
    }

    /// <summary>
    /// Gets the file path that will be checked for a text file containing the version.
    /// </summary>
    public string VersionFilePath { get; }

    /// <summary>
    /// Get the status of the API.
    /// </summary>
    /// <returns>OK "GOOD".</returns>
    public IResult GetStatus()
    {
        _countStatus.Add(1);
        return Results.Ok("GOOD");
    }

    /// <summary>
    /// Get the version of the API.
    /// This can come from the content of a file named version.txt located in the same directory as the entry assembly, or if not found then from the entry assembly version.
    /// </summary>
    /// <returns>The API version number as a string.</returns>
    public async Task<IResult> GetVersionAsync()
    {
        var version = "UNKNOWN";
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _countVersion.Add(1);
            _logger.LogDebug("Checking for version file at {VersionPath}", VersionFilePath);
            if (_fileSystem.File.Exists(VersionFilePath))
            {
                _logger.LogDebug("Found version file at {VersionFilePath}", VersionFilePath);
                version = await _fileSystem.File.ReadAllTextAsync(VersionFilePath);
            }
            else
            {
                var entryAssemblyVersion = GetAssemblyVersion();
                if (entryAssemblyVersion is not null)
                    version = entryAssemblyVersion;
            }
            _logger.LogInformation("GetVersionQuery handler. Found version: {Version}", version);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to find version");
        }
        finally
        {
            _versionTime.Record(stopwatch.ElapsedMilliseconds);
        }
        return Results.Ok(version.Trim());
    }

    [ExcludeFromCodeCoverage]
    private string GetVersionFilePath()
        => Path.Combine(Path.GetDirectoryName(_entryAssembly?.Location ?? string.Empty) ?? ".", _versionFile);

    [ExcludeFromCodeCoverage]
    private string? GetAssemblyVersion()
        => _entryAssembly?.GetName()?.Version?.ToString(3);
}
