namespace AspNet.KickStarter.FunctionalResult.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> type.
/// </summary>
public static class MatchExtensions
{
    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public static T Match<T>(this Result result, Func<T> onSuccess, Func<Error, T> onError)
    {
        if (result.Error is null)
            return onSuccess();
        return onError(result.Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public static T Match<TSuccess, T>(this Result<TSuccess> result, Func<TSuccess, T> onSuccess, Func<Error, T> onError)
    {
        if (result.Error is null)
            return onSuccess(result.Value);
        return onError(result.Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public async static Task<T> MatchAsync<T>(this Result result, Func<Task<T>> onSuccess, Func<Error, Task<T>> onError)
    {
        if (result.Error is null)
            return await onSuccess();
        return await onError(result.Error.Value);
    }

    /// <summary>
    /// Return the result from the appropriate function depending on the state of the <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TSuccess">The successful value type of the result.</typeparam>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="result">The result to act on.</param>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onError">The function to invoke if the result is an error.</param>
    /// <returns>The result from the appropriate function.</returns>
    public async static Task<T> MatchAsync<TSuccess, T>(this Result<TSuccess> result, Func<TSuccess, Task<T>> onSuccess, Func<Error, Task<T>> onError)
    {
        if (result.Error is null)
            return await onSuccess(result.Value);
        return await onError(result.Error.Value);
    }
}
