using System.Data;

namespace VoiceFirst_Admin.Data.Contracts.IContext
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
