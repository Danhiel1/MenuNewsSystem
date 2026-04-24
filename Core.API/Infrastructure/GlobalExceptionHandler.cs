using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
            // Write LogException(exception);
            _logger.LogError(exception, "Hệ thống gặp lỗi: {Message}", exception.Message);
            //
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };
            // Type Error
            if (exception is ValidationException validationException)
            {
                // Nếu là lỗi dữ liệu từ FluentValidation (Trạm kiểm soát)
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Lỗi dữ liệu đầu vào (Validation Error)";
                problemDetails.Detail = "Một hoặc nhiều trường dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
                // Trích xuất chi tiết các trường bị lỗi
                problemDetails.Extensions["errors"] = validationException.Errors
                    .Select(e=> new { e.PropertyName, e.ErrorMessage } );
            }
            else
            {
                // Nếu là các lỗi hệ thống khác (rớt mạng, sai code, v.v.)
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Lỗi máy chủ (Server Error)";
                problemDetails.Detail = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau.";
                // Mẹo: Ở môi trường Production, không nên ném exception.Message ra ngoài để bảo mật.
            }
            // Trả về Client 
            httpContext.Response.StatusCode= problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        }
    }


