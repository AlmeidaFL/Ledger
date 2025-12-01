namespace UserApi.Services.Exceptions;

public class DomainForbiddenException : Exception
{
    public DomainForbiddenException(string message) : base(message) { }
}