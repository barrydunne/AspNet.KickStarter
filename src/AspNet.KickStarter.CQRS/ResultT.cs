namespace AspNet.KickStarter.CQRS;

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
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.CQRS.Error"/>.
    /// </summary>
    /// <param name="value">The successful value to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(TSuccess value) => new(value);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.CQRS.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.CQRS.Error"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(Error error) => new(error);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to create the <see cref="Result"/> from.</param>
    public static implicit operator Result<TSuccess>(Exception exception) => new((Error)exception);

    /// <summary>
    /// Create a new unsuccessful result with the given <see cref="AspNet.KickStarter.CQRS.Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="AspNet.KickStarter.CQRS.Error"/> with details about the failure.</param>
    /// <returns>A new unsuccessful result.</returns>
    public static Result<TSuccess> FromError(Error error) => new(error);

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <param name="value">The successful value.</param>
    /// <returns>A new successful result.</returns>
    public static Result<TSuccess> Success(TSuccess value) => new(value);

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public readonly void Switch(Action<TSuccess> onSuccess, Action<Error> onError)
    {
        if (Error is null)
            onSuccess(Value);
        else
            onError(Error.Value);
    }

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async readonly Task SwitchAsync(Func<TSuccess, Task> onSuccess, Func<Error, Task> onError)
    {
        if (Error is null)
            await onSuccess(Value);
        else
            await onError(Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    public readonly void IfSuccess(Action<TSuccess> onSuccess)
    {
        if (Error is null)
            onSuccess(Value);
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async readonly Task IfSuccessAsync(Func<TSuccess, Task> onSuccess)
    {
        if (Error is null)
            await onSuccess(Value);
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
    public readonly T Match<T>(Func<TSuccess, T> onSuccess, Func<Error, T> onError)
    {
        if (Error is null)
            return onSuccess(Value);
        return onError(Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public async readonly Task<T> MatchAsync<T>(Func<TSuccess, Task<T>> onSuccess, Func<Error, Task<T>> onError)
    {
        if (Error is null)
            return await onSuccess(Value);
        return await onError(Error.Value);
    }
}
