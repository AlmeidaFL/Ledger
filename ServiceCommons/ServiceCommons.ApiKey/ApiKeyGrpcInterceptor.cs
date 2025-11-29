using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ServiceCommons.ApiKey;

public class ApiKeyGrpcInterceptor(string apiKey) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        
        headers.Add("x-api-key", apiKey);
        var newOptions = context.Options.WithHeaders(headers);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);
        
        return continuation(request, newContext);
    }
}