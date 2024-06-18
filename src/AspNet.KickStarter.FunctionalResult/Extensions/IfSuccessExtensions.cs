namespace AspNet.KickStarter.FunctionalResult.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> type.
/// </summary>
public static class IfSuccessExtensions
{
    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    public static void IfSuccess(this Result result, Action onSuccess)
    {
        if (result.Error is null)
            onSuccess();
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    public static void IfSuccess<TSuccess>(this Result<TSuccess> result, Action<TSuccess> onSuccess)
    {
        if (result.Error is null)
            onSuccess(result.Value);
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task IfSuccessAsync(this Result result, Func<Task> onSuccess)
    {
        if (result.Error is null)
            await onSuccess();
    }

    /// <summary>
    /// Invoke an action only if the result was successful.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The action to invoke if the result is a success.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async static Task IfSuccessAsync<TSuccess>(this Result<TSuccess> result, Func<TSuccess, Task> onSuccess)
    {
        if (result.Error is null)
            await onSuccess(result.Value);
    }
}
