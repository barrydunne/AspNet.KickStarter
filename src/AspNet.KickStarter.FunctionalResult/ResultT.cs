namespace AspNet.KickStarter.FunctionalResult;

/// <summary>
/// Represents the result of an operation, either success with a value or an error.
/// </summary>
/// <typeparam name="TSuccess">The successful value type.</typeparam>
public readonly struct Result<TSuccess> : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TSuccess}"/> struct with a successful result.
    /// </summary>
    /// <param name="value">The successful value.</param>
    public Result(TSuccess value)
    {
        Value = value;
        Error = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TSuccess}"/> struct with an error result.
    /// </summary>
    /// <param name="error">The error details.</param>
    public Result(Error error)
    {
        Value = default!;
        Error = error;
    }

    /// <inheritdoc/>
    public readonly bool IsSuccess => Error is null;

    /// <inheritdoc/>
    public readonly bool IsError => Error is not null;

    /// <summary>
    /// Gets the va;ie if the result was successful.
    /// </summary>
    public TSuccess Value { get; }

    /// <inheritdoc/>
    public Error? Error { get; }

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.FunctionalResult.Error"/>.
    /// </summary>
    /// <param name="value">The successful value to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(TSuccess value) => new(value);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.FunctionalResult.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.FunctionalResult.Error"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(Error error) => new(error);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(Exception exception) => new((Error)exception);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.FunctionalResult.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.FunctionalResult.Error"/> with details about the failure.</param>
    /// <returns>A new unsuccessful result.</returns>
    public static Result<TSuccess> FromError(Error error) => new(error);

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <param name="value">The successful value.</param>
    /// <returns>A new successful result.</returns>
    public static Result<TSuccess> Success(TSuccess value) => new(value);
}
