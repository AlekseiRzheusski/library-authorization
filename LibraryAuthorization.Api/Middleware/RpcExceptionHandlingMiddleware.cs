using Grpc.Core;

namespace LibraryAuthorization.Api.Middleware;

public class RpcExceptionHandlingMiddleware
{
    private readonly RequestDelegate _request;

    public RpcExceptionHandlingMiddleware(RequestDelegate request)
    {
        _request = request;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (RpcException ex)
        {
            context.Response.ContentType = "application/json";

            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                case StatusCode.InvalidArgument:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                
                case StatusCode.OutOfRange:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case StatusCode.Unauthenticated:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Status.Detail,
                grpcCode = ex.StatusCode.ToString()
            });
        }
    }
}
