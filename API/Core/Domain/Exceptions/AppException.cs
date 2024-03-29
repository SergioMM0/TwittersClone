namespace API.Core.Domain.Exceptions;

/// <summary>
/// Represents a general application exception.
/// </summary>
public abstract class AppException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="error">The error message associated with the exception.</param>
    public AppException(string error) : base(error) {

    }
}
