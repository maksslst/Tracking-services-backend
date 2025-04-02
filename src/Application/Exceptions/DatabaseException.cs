using System.Data.Common;
using System.Net;

namespace Application.Exceptions;

public class DatabaseException : BaseApplicationException
{
    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, DbException inner) : base(message, inner)
    {
    }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    public override string Title => "Database error";
}