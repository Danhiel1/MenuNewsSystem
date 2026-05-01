namespace Core.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // Factory method cho response thành công
        public static ApiResponse<T> Success(T data, string message = "Thành công")
            => new() { Status = true, Message = message, Data = data };

        // Factory method cho response thất bại
        public static ApiResponse<T> Fail(string message)
            => new() { Status = false, Message = message, Data = default };
    }
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Success(string message = "Thành công")
            => new() { Status = true, Message = message };

        public new static ApiResponse Fail(string message)
            => new() { Status = false, Message = message };
    }
}
