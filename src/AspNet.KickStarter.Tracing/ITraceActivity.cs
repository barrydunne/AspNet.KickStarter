using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AspNet.KickStarter;

/// <summary>
/// Used to create a new trace Activity.
/// </summary>
public interface ITraceActivity
{
    /// <summary>
    /// Gets the name of the activity source object.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name and activity kind.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="kind">The activity kind.</param>
    /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
    Activity? StartActivity([CallerMemberName] string name = "", ActivityKind kind = ActivityKind.Internal);
}
