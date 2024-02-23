using System.Diagnostics;
using API.Core.Domain.Exceptions;
using API.Core.Domain.Responses;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Application.Filters; 

/// <summary>
/// An exception filter that filters exceptions thrown by the application.
/// </summary>
public class ExceptionFilter : IAsyncExceptionFilter {
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(IHostEnvironment hostEnvironment, ILogger<ExceptionFilter> logger) {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    /// <summary>
    /// Filters exceptions thrown by the application to return a proper error response.
    /// </summary>
    /// <param name="context">The context of the exception.</param>
    public async Task OnExceptionAsync(ExceptionContext context) {
        context.ExceptionHandled = true;
        const string baseErrorMessage = "Something went wrong";
        var trace = Activity.Current?.Id ?? context?.HttpContext.TraceIdentifier;
        var exception = context!.Exception;

        string errorCode;
        int statusCode;
        string errorMessage;

        if (exception is AppException appException) {
            switch (appException) {
                // Add here any kind of exception thrown by the API
                
                /*
                Ex:
                case UserNotFoundException userNotFoundException:
                    errorCode = "userNotFound";
                    statusCode = StatusCodes.Status404NotFound;
                    errorMessage = localeNotFoundException.Message;
                    break;
                 
                 */
                default:
                    errorCode = "AppExceptionNotHandled";
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorMessage = "AppException not handled in exception filter";
                    break;
            }
        } else {
            // Canceled request 
            if (exception is OperationCanceledException) {
                context.HttpContext.Response.StatusCode = 499;
                return;
            }
            _logger.LogError(exception, "Unhandled exception");

            errorCode = string.Empty;
            statusCode = StatusCodes.Status500InternalServerError;
            errorMessage = baseErrorMessage;
        }

        context.HttpContext.Response.StatusCode = statusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ErrorResponse(errorCode, statusCode, trace, errorMessage));
    }
}
