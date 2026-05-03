using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace Core.API.Interceptors;

public class GrpcLoggingInterceptor : Interceptor
{
    private readonly ILogger<GrpcLoggingInterceptor> _logger;

    public GrpcLoggingInterceptor(ILogger<GrpcLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("gRPC started: {Method}", context.Method);
        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation("gRPC OK: {Method} | {Ms}ms", context.Method, sw.ElapsedMilliseconds);
            return response;
        }
        catch (RpcException ex)
        {
            // Lỗi gRPC đã được typed sẵn — log warning rồi re-throw
            _logger.LogWarning("gRPC error: {Method} | {Ms}ms | {StatusCode} | {Message}",
                context.Method, sw.ElapsedMilliseconds, ex.StatusCode, ex.Message);
            throw;
        }
        catch (ValidationException ex)
        {
            // Lỗi validation (FluentValidation) → InvalidArgument (tương đương HTTP 400)
            var errors = string.Join("; ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            _logger.LogWarning("gRPC validation failed: {Method} | {Ms}ms | {Errors}",
                context.Method, sw.ElapsedMilliseconds, errors);
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        catch (Exception ex)
        {
            // Lỗi hệ thống → Internal (tương đương HTTP 500)
            _logger.LogError(ex, "gRPC internal error: {Method} | {Ms}ms | {Message}",
                context.Method, sw.ElapsedMilliseconds, ex.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Đã có lỗi hệ thống. Vui lòng thử lại sau."));
        }
    }
}
