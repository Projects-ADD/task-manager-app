using System.Net;

namespace TaskManager.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message, string errorCode = "resource_not_found")
        : base(message, errorCode, (int)HttpStatusCode.NotFound)
    {
    }

    /* public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string message, string entityName)
        : base($"Entity '{entityName}' not found. {message}")
    {
    } */
}