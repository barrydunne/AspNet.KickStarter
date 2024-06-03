using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace AspNet.KickStarter;

/// <summary>
/// Extension methods for <see cref="IMeterFactory"/>.
/// </summary>
public static class MeterFactoryExtensions
{
    /// <summary>
    /// Creates a new <see cref="Meter"/> instance with the application name.
    /// </summary>
    /// <param name="meterFactory">The <see cref="IMeterFactory"/> to use to create the <see cref="Meter"/>.</param>
    /// <returns>A new <see cref="Meter"/> instance.</returns>
    public static Meter CreateAssemblyMeter(this IMeterFactory meterFactory)
        => meterFactory.Create(GetAssemblyName());

    [ExcludeFromCodeCoverage(Justification = "Nullable GetEntryAssembly()")]
    private static string GetAssemblyName() => Assembly.GetEntryAssembly()?.GetName().Name ?? "AspNet.KickStarter";
}
