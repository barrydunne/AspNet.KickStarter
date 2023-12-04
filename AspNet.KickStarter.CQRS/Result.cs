namespace AspNet.KickStarter.CQRS;

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
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.CQRS.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.CQRS.Error"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result(Error error) => new(error);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result(Exception exception) => new((Error)exception);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.CQRS.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.CQRS.Error"/> with details about the failure.</param>
    /// <returns>A new unsuccessful result.</returns>
    public static Result FromError(Error error) => new(error);

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <returns>A new successful result.</returns>
    public static Result Success() => new();

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public readonly void Switch(Action onSuccess, Action<Error> onError)
    {
        if (Error is null)
            onSuccess();
        else
            onError(Error.Value);
    }

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async readonly Task SwitchAsync(Func<Task> onSuccess, Func<Error, Task> onError)
    {
        if (Error is null)
            await onSuccess();
        else
            await onError(Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    public readonly void IfSuccess(Action onSuccess)
    {
        if (Error is null)
            onSuccess();
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async readonly Task IfSuccessAsync(Func<Task> onSuccess)
    {
        if (Error is null)
            await onSuccess();
    }

    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public readonly void IfError(Action<Error> onError)
    {
        if (Error is not null)
            onError(Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async readonly Task IfErrorAsync(Func<Error, Task> onError)
    {
        if (Error is not null)
            await onError(Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public readonly T Match<T>(Func<T> onSuccess, Func<Error, T> onError)
    {
        if (Error is null)
            return onSuccess();
        return onError(Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public async readonly Task<T> MatchAsync<T>(Func<Task<T>> onSuccess, Func<Error, Task<T>> onError)
    {
        if (Error is null)
            return await onSuccess();
        return await onError(Error.Value);
    }
}
