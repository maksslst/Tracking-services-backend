using System.Net;

namespace Application.Exceptions;

public class EntityUpdateException : BaseApplicationException
{
    public EntityUpdateException(string message) : base(message)
    {
    }

    public EntityUpdateException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string Title => "Failed to update the entity";
}