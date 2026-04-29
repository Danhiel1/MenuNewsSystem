using Core.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Core.API.Infrastructure
{
    /// <summary>
    /// TẠI SAO cần GlobalExceptionHandler?
    /// - Bắt TẤT CẢ exceptions không được handle trong code
    /// - Trả về ApiResponse format chuẩn (thay vì HTML error page mặc định)
    /// - Frontend LUÔN nhận cùng format, kể cả khi server lỗi
    /// 
    /// TẠI SAO trả ApiResponse thay vì ProblemDetails (như trước)?
    /// - ProblemDetails là format khác: { type, title, status, detail }
    /// - Nếu API bình thường trả { status, message, data } nhưng lỗi trả ProblemDetails
    ///   → Frontend phải check 2 format → phức tạp, dễ bug
    /// </summary>
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
                // TẠI SAO 400? Vì lỗi do CLIENT gửi dữ liệu sai
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = validationException.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage });

                // Data chứa chi tiết lỗi từng field — frontend hiển thị được
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
                // Lỗi hệ thống (NullReference, DB timeout, v.v.)
                // TẠI SAO 500? Vì lỗi do SERVER, không phải client
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // TẠI SAO không trả chi tiết exception?
                // → Lỗ hổng bảo mật! Hacker có thể thấy stack trace, connection string...
                var response = ApiResponse.Fail("Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau.");

                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            }

            return true;
        }
    }
}
