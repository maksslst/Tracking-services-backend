using System.Net;

namespace Application.Exceptions;

public class NotFoundApplicationException : BaseApplicationException
{
    public NotFoundApplicationException(string message) : base(message)
    {
    }

    public NotFoundApplicationException(string message, Exception inner) : base(message, inner)
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public override string Title => "Entity not found";
}