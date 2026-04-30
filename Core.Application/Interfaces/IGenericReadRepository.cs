namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO tách Read Repository riêng?
    /// - Theo CQRS: Read (query) và Write (command) dùng DB khác nhau
    /// - Write → SQL Server (qua IGenericRepository)
    /// - Read  → MongoDB (qua IGenericReadRepository)
    /// - Tách ra để code không bị phụ thuộc lẫn nhau
    /// </summary>
    public interface IGenericReadRepository<T> where T : class
    {
        Task InsertAsync(T document, CancellationToken cancellationToken);
        Task UpdateAsync(T document, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
