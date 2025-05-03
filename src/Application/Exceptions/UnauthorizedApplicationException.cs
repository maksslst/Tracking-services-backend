using System.Net;

namespace Application.Exceptions;

public class UnauthorizedApplicationException : BaseApplicationException
{
    public UnauthorizedApplicationException(string message) : base(message)
    {
    }

    public UnauthorizedApplicationException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
    public override string Title => "Unauthorized access";
}