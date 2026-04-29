namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO cần IDocument?
    /// - MongoDB cần biết mỗi document có field Id để query, delete, update
    /// - IDocument đảm bảo mọi MongoDB document đều có Id
    /// - MongoGenericRepository&lt;T&gt; yêu cầu T implement IDocument
    /// </summary>
    public interface IDocument
    {
        int Id { get; set; }
    }
}
