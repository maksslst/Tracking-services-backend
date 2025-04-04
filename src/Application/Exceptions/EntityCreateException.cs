using System.Net;

namespace Application.Exceptions;

public class EntityCreateException : BaseApplicationException
{
    public EntityCreateException(string message) : base(message)
    {
    }

    public EntityCreateException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string Title => "Couldn't create an entity";
}