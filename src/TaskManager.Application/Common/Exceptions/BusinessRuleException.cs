using System.Net;

namespace TaskManager.Application.Common.Exceptions;

public class BusinessRuleException : AppException
{

    public BusinessRuleException(string message, string errorCode = "business_rule_violation")
        : base(message, errorCode, (int)HttpStatusCode.BadRequest)
    {
    }

    /* public BusinessRuleException()
    {
    }

    public BusinessRuleException(string message)
        : base(message)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public BusinessRuleException(string message, string entityName)
        : base($"Business rule violation for entity '{entityName}'. {message}")
    {
    } */
}