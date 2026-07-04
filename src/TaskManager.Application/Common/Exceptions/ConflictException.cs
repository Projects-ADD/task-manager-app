using System.Net;

namespace TaskManager.Application.Common.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message, string errorCode = "resource_conflict")
        : base(message, errorCode, (int)HttpStatusCode.Conflict)
    {
    }

    /* public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    } */
}