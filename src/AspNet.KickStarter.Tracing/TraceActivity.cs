using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AspNet.KickStarter;

/// <summary>
/// Used to create a new trace Activity.
/// </summary>
public class TraceActivity : ITraceActivity, IDisposable
{
    private readonly ActivitySource _activitySource;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="TraceActivity"/> class.
    /// </summary>
    /// <param name="activityName">The name of the activity source object.</param>
    public TraceActivity(string activityName)
    {
        Name = activityName;
        _activitySource = new(activityName);
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Activity? StartActivity([CallerMemberName] string name = "", ActivityKind kind = ActivityKind.Internal)
        => _activitySource.StartActivity(name, kind);

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose of the <see cref="ActivitySource"/>.
    /// </summary>
    /// <param name="disposing">Whether to dispose of resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
            if (disposing)
                _activitySource.Dispose();
        }
    }
}
