namespace AspNet.KickStarter.FunctionalResult.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> type.
/// </summary>
public static class SwitchExtensions
{
    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public static void Switch(this Result result, Action onSuccess, Action<Error> onError)
    {
        if (result.Error is null)
            onSuccess();
        else
            onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    public static void Switch<TSuccess>(this Result<TSuccess> result, Action<TSuccess> onSuccess, Action<Error> onError)
    {
        if (result.Error is null)
            onSuccess(result.Value);
        else
            onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task SwitchAsync(this Result result, Func<Task> onSuccess, Func<Error, Task> onError)
    {
        if (result.Error is null)
            await onSuccess();
        else
            await onError(result.Error.Value);
    }

    /// <summary>
    /// Invoke the appropriate action depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <param name="onError">The action to invoke if the result is an error.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task SwitchAsync<TSuccess>(this Result<TSuccess> result, Func<TSuccess, Task> onSuccess, Func<Error, Task> onError)
    {
        if (result.Error is null)
            await onSuccess(result.Value);
        else
            await onError(result.Error.Value);
    }
}
