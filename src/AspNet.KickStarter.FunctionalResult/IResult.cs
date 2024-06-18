namespace AspNet.KickStarter.FunctionalResult;

/// <summary>
/// Represents the result of an operation, either success or an error.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether was the result a success.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether was the result an error.
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// Gets the error if the result was unsuccessful.
    /// </summary>
    Error? Error { get; }
}
