using System.Net;

namespace Application.Exceptions;

public class UserAuthorizationException : BaseApplicationException
{
    public UserAuthorizationException(string message) : base(message)
    {
    }

    public UserAuthorizationException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string Title => "User does not belong to the company";
}