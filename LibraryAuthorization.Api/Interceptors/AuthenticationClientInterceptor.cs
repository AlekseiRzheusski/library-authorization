using Grpc.Core.Interceptors;
using Grpc.Core;

namespace LibraryAuthorization.Api.Interceptors;

public class AuthenticationClientInterceptor: Interceptor
{
    private const string headerName = "x-api-key";
    private readonly IConfiguration _configuration;

    public AuthenticationClientInterceptor(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var apiKey = _configuration["ApiKey"];
        var headers = context.Options.Headers ?? new Metadata();
        headers.Add("x-api-key", apiKey!);

        var options = context.Options.WithHeaders(headers);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method, context.Host, options);

        return continuation(request, newContext);
    }
}
