using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace AspNet.KickStarter.FunctionalResult;

/// <summary>
/// Represents a failed operation.
/// </summary>
public readonly struct Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> struct from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="message">The error message.</param>
    public Error(string message)
    {
        Message = message;
        InnerException = null;
        ValidationResult = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> struct from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception for the error.</param>
    public Error(Exception exception)
    {
        Message = exception.Message;
        InnerException = exception;
        ValidationResult = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> struct from a <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">The validation result for the error.</param>
    public Error(ValidationResult validationResult)
    {
        Message = validationResult.ToString();
        InnerException = null;
        ValidationResult = validationResult;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the <see cref="Exception"/> for this error, if any.
    /// </summary>
    public Exception? InnerException { get; }

    /// <summary>
    /// Gets the <see cref="ValidationResult"/> for this error, if any.
    /// </summary>
    public ValidationResult? ValidationResult { get; }

    /// <summary>
    /// Create an <see cref="Error"/> from a string.
    /// </summary>
    /// <param name="message">The message to create the <see cref="Error"/> from.</param>
    public static implicit operator Error(string message) => new(message);

    /// <summary>
    /// Create an <see cref="Error"/> from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to create the <see cref="Error"/> from.</param>
    public static implicit operator Error(Exception exception) => new(exception);

    /// <summary>
    /// Create an <see cref="Error"/> from an <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">The <see cref="ValidationResult"/> to create the <see cref="Error"/> from.</param>
    public static implicit operator Error(ValidationResult validationResult) => new(validationResult);

    /// <summary>
    /// Create an appropriate <see cref="Microsoft.AspNetCore.Http.IResult"/> for the error.
    /// </summary>
    /// <returns>A ValidationProblem or a Problem result.</returns>
    public Microsoft.AspNetCore.Http.IResult AsHttpResult()
    {
        return ValidationResult is not null
            ? Results.ValidationProblem(ValidationResult.ToDictionary())
            : Results.Problem(Message);
    }

    /// <inheritdoc/>
    public override string ToString() => Message;
}
