namespace AspNet.KickStarter.FunctionalResult.Extensions;

/// <summary>
/// Provides Railway Oriented Programming (ROP) extension methods for the <see cref="Result"/> type.
/// </summary>
public static class BindExtensions
{
    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
        => (result.Error is not null) ? result.Error.Value : func(result.Value);

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Result Bind<TIn>(this Result<TIn> result, Func<TIn, Result> func)
        => (result.Error is not null) ? result.Error.Value : func(result.Value);

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Result<TOut> Bind<TOut>(this Result result, Func<Result<TOut>> func)
        => (result.Error is not null) ? result.Error.Value : func();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Result Bind(this Result result, Func<Result> func)
        => (result.Error is not null) ? result.Error.Value : func();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> func)
        => result.ContinueWith(task => Bind(task.Result, func));

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind<TIn>(this Task<Result<TIn>> result, Func<TIn, Result> func)
        => result.ContinueWith(task => Bind(task.Result, func));

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TOut>(this Task<Result> result, Func<Result<TOut>> func)
        => result.ContinueWith(task => Bind(task.Result, func));

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind(this Task<Result> result, Func<Result> func)
        => result.ContinueWith(task => Bind(task.Result, func));

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Task<Result<TOut>>> func)
        => result.ContinueWith(task => Bind(task.Result, func)).Unwrap();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind<TIn>(this Task<Result<TIn>> result, Func<TIn, Task<Result>> func)
        => result.ContinueWith(task => Bind(task.Result, func)).Unwrap();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TOut>(this Task<Result> result, Func<Task<Result<TOut>>> func)
        => result.ContinueWith(task => Bind(task.Result, func)).Unwrap();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind(this Task<Result> result, Func<Task<Result>> func)
        => result.ContinueWith(task => Bind(task.Result, func)).Unwrap();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> func)
        => (result.Error is not null) ? Task.FromResult<Result<TOut>>(result.Error.Value) : func(result.Value);

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TIn">The successful value type of the first result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind<TIn>(this Result<TIn> result, Func<TIn, Task<Result>> func)
        => (result.Error is not null) ? Task.FromResult<Result>(result.Error.Value) : func(result.Value);

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <typeparam name="TOut">The successful value type of the second result.</typeparam>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result<TOut>> Bind<TOut>(this Result result, Func<Task<Result<TOut>>> func)
        => (result.Error is not null) ? Task.FromResult<Result<TOut>>(result.Error.Value) : func();

    /// <summary>
    /// Allows chaining of functions that return a result.
    /// </summary>
    /// <param name="result">The first result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The new result.</returns>
    public static Task<Result> Bind(this Result result, Func<Task<Result>> func)
        => (result.Error is not null) ? Task.FromResult<Result>(result.Error.Value) : func();
}
