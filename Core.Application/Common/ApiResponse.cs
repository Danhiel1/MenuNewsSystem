namespace Core.Application.Common
{
    /// <summary>
    /// Response format chuẩn cho toàn bộ API.
    /// 
    /// TẠI SAO cần class này?
    /// - Mọi endpoint đều trả VỀ CÙNG 1 format: { status, message, data }
    /// - Frontend chỉ cần check response.status để biết thành công/thất bại
    /// - Kể cả khi lỗi (validation, server error) cũng trả cùng format
    /// 
    /// TẠI SAO dùng Generic T?
    /// - Vì data có thể là int (Id), List (danh sách), object (chi tiết)...
    /// - ApiResponse&lt;int&gt;    → data là int
    /// - ApiResponse&lt;List&lt;MenuDto&gt;&gt; → data là list
    /// 
    /// TẠI SAO dùng static factory method thay vì constructor?
    /// - Code gọn hơn: ApiResponse&lt;int&gt;.Success(1) thay vì new ApiResponse&lt;int&gt; { Status = true, ... }
    /// - Đảm bảo Status luôn đúng (Success → true, Fail → false)
    /// </summary>
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

    /// <summary>
    /// Response không cần data — dùng cho Delete, Update (chỉ cần status + message).
    /// Kế thừa ApiResponse&lt;object&gt; để giữ cùng format.
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Success(string message = "Thành công")
            => new() { Status = true, Message = message };

        public new static ApiResponse Fail(string message)
            => new() { Status = false, Message = message };
    }
}
