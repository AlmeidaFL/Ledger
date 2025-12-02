namespace LedgerGateway.Exceptions;

public sealed class UnifiedApiException : Exception
{
    public int StatusCode { get; }
    public string Response { get; }
    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

    private UnifiedApiException(
        string message,
        int statusCode,
        string response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers)
        : base(message)
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public static UnifiedApiException From(object apiException)
    {
        var type = apiException.GetType();

        var statusCode = (int)(type.GetProperty("StatusCode")?.GetValue(apiException) 
            ?? throw new InvalidOperationException("StatusCode missing"));

        var message = (string)(type.GetProperty("Message")?.GetValue(apiException) 
            ?? "Unknown error");

        var response = (string)(type.GetProperty("Response")?.GetValue(apiException) 
            ?? "");

        var headersObj = type.GetProperty("Headers")?.GetValue(apiException)
            ?? new Dictionary<string, IEnumerable<string>>();

        var headers = headersObj as IReadOnlyDictionary<string, IEnumerable<string>>
            ?? new Dictionary<string, IEnumerable<string>>();

        return new UnifiedApiException(message, statusCode, response, headers);
    }
}


