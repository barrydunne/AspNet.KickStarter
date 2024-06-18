namespace AspNet.KickStarter.FunctionalResult.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> type.
/// </summary>
public static class IfErrorExtensions
{
    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <param name="result">The result to act on.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public static void IfError(this Result result, Action<Error> onError)
    {
        if (result.Error is not null)
            onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public static void IfError<TSuccess>(this Result<TSuccess> result, Action<Error> onError)
    {
        if (result.Error is not null)
            onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <param name="result">The result to act on.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task IfErrorAsync(this Result result, Func<Error, Task> onError)
    {
        if (result.Error is not null)
            await onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was an error.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task IfErrorAsync<TSuccess>(this Result<TSuccess> result, Func<Error, Task> onError)
    {
        if (result.Error is not null)
            await onError(result.Error.Value);
    }
}
