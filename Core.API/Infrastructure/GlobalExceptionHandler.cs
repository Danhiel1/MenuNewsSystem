using Core.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Core.API.Infrastructure
{

    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Hệ thống gặp lỗi: {Message}", exception.Message);

            if (exception is ValidationException validationException)
            {
                // Lỗi validation từ FluentValidation
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = validationException.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage });

                // Data chứa chi tiết lỗi từng field
                var response = new ApiResponse<object>
                {
                    Status = false,
                    Message = "Lỗi dữ liệu đầu vào (Validation Error)",
                    Data = errors
                };
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            }
            else
            {
                // Lỗi hệ thống
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = ApiResponse.Fail("Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau.");
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            }

            return true;
        }
    }
}
