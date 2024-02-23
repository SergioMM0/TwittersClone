using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Core.Domain.Responses;

/// <summary>
/// Represents a standard format for HTTP error responses.
/// </summary>
public class ErrorResponse {

    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    public int Status { get; }

    /// <summary>
    /// Gets the optional trace identifier for the error, used for logging and diagnostics.
    /// </summary>
    public string? TraceId { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Constructs a new instance of the ErrorResponse class.
    /// </summary>
    /// <param name="type">The type of the error.</param>
    /// <param name="status">The HTTP status code associated with the error.</param>
    /// <param name="traceId">The optional trace identifier for the error.</param>
    /// <param name="error">The error message.</param>
    public ErrorResponse(string type, int status, string? traceId, string error) {
        Type = type;
        Status = status;
        TraceId = traceId;
        Error = error;
    }

    /// <summary>
    /// Constructs a new instance of the ErrorResponse class, with an empty error type and automatically detected trace identifier.
    /// </summary>
    /// <param name="status">The HTTP status code associated with the error.</param>
    /// <param name="error">The error message.</param>
    public ErrorResponse(int status, string error) {
        Type = "";
        Status = status;
        TraceId = Activity.Current?.Id;
        Error = error;
    }

    /// <summary>
    /// Constructs a new instance of the ErrorResponse class, with an automatically detected trace identifier.
    /// </summary>
    /// <param name="type">The type of the error.</param>
    /// <param name="status">The HTTP status code associated with the error.</param>
    /// <param name="error">The error message.</param>
    public ErrorResponse(string type, int status, string error) {
        Type = type;
        Status = status;
        TraceId = Activity.Current?.Id;
        Error = error;
    }

    /// <summary>
    /// Constructs a new instance of the ErrorResponse class based on model state errors. 
    /// The error type is empty and the trace identifier is automatically detected.
    /// </summary>
    /// <param name="modelState">The model state containing validation errors.</param>
    /// <param name="status">The HTTP status code associated with the error.</param>
    public ErrorResponse(ModelStateDictionary modelState, int status) {
        Type = "";
        Status = status;
        TraceId = Activity.Current?.Id;
        // Select first error from first property
        Error = modelState?.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage ?? "";
    }
}
