namespace AspNet.KickStarter.FunctionalResult;

/// <summary>
/// Represents the result of an operation, either success or an error.
/// </summary>
public readonly struct Result : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with a successful result.
    /// </summary>
    public Result() => Error = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with an error result.
    /// </summary>
    /// <param name="error">The error details.</param>
    public Result(Error error) => Error = error;

    /// <inheritdoc/>
    public readonly bool IsSuccess => Error is null;

    /// <inheritdoc/>
    public readonly bool IsError => Error is not null;

    /// <inheritdoc/>
    public Error? Error { get; }

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.FunctionalResult.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.FunctionalResult.Error"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result(Error error) => new(error);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result(Exception exception) => new((Error)exception);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.FunctionalResult.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.FunctionalResult.Error"/> with details about the failure.</param>
    /// <returns>A new unsuccessful result.</returns>
    public static Result FromError(Error error) => new(error);

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <returns>A new successful result.</returns>
    public static Result Success() => new();
}
