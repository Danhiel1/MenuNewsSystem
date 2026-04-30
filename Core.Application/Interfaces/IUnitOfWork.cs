namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO cần Unit of Work?
    /// 
    /// VẤN ĐỀ 1 — SaveChanges phân tán:
    ///   Mỗi repository tự SaveChanges() → nếu save Menu xong nhưng save MenuNews fail
    ///   → dữ liệu không nhất quán (Menu đã lưu, MenuNews chưa)
    /// 
    /// VẤN ĐỀ 2 — Không có Transaction:
    ///   Handler: AddAsync → SaveChanges → Publish Event
    ///   Nếu Publish Event fail → dữ liệu đã lưu vào DB nhưng event mất
    ///   → MongoDB không được sync → mất đồng bộ
    /// 
    /// GIẢI PHÁP — Unit of Work:
    ///   - SaveChanges TẬP TRUNG ở 1 chỗ (UoW), không phải ở mỗi repo
    ///   - BeginTransaction → tất cả thay đổi → Commit (hoặc Rollback nếu lỗi)
    ///   - Repository&lt;T&gt;() → lấy repo cho BẤT KỲ entity nào mà không cần tạo class
    /// 
    /// TẠI SAO kế thừa IDisposable?
    /// - UoW giữ DbContext và Transaction → cần giải phóng khi request kết thúc
    /// - DI container (Scoped) tự gọi Dispose() cuối mỗi HTTP request
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Lấy generic repository cho bất kỳ entity.
        /// Ví dụ: _unitOfWork.Repository&lt;Menu&gt;() → IGenericRepository&lt;Menu&gt;
        /// KHÔNG CẦN tạo MenuRepository riêng cho CRUD cơ bản!
        /// </summary>
        IGenericRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Lưu TẤT CẢ thay đổi vào DB — gọi 1 lần duy nhất sau khi xong logic.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Bắt đầu transaction — từ đây mọi thay đổi có thể rollback.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Xác nhận commit — CHỈ gọi khi TẤT CẢ steps thành công.
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Hoàn tác TẤT CẢ thay đổi — gọi trong catch block khi có lỗi.
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
