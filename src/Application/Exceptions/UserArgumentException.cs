using System.Net;

namespace Application.Exceptions;

public class UserArgumentException : BaseApplicationException
{
    public UserArgumentException(string message) : base(message)
    {
    }

    public UserArgumentException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string Title => "User not belong to the company";
}