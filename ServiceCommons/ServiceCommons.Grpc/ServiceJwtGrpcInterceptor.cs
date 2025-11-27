using Grpc.Core;
using Grpc.Core.Interceptors;
using ServiceCommons.Jwt;

namespace ServiceCommons.Grpc;

public class ServiceJwtGrpcInterceptor(IJwtTokenGenerator tokenGenerator, string serviceName) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var token = tokenGenerator.GenerateServiceToken(serviceName);

        var headers = context.Options.Headers ?? [];
        headers.Add("authorization", $"Bearer {token}");

        var newOptions = context.Options.WithHeaders(headers);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            newOptions);

        return continuation(request, newContext);
    }
}