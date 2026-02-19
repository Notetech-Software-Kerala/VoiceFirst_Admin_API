using System.Data;
using VoiceFirst_Admin.Data.Contracts.IContext;

namespace VoiceFirst_Admin.Data.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDapperContext _context;
        private IDbConnection? _connection;
        private IDbTransaction? _transaction;

        public UnitOfWork(IDapperContext context)
        {
            _context = context;
        }

        public IDbConnection Connection =>
            _connection ?? throw new InvalidOperationException("Unit of work has not been started. Call BeginAsync first.");

        public IDbTransaction Transaction =>
            _transaction ?? throw new InvalidOperationException("Unit of work has not been started. Call BeginAsync first.");

        public Task BeginAsync()
        {
            if (_connection is not null)
                throw new InvalidOperationException("Unit of work is already active. Commit or rollback before starting a new one.");

            _connection = _context.CreateConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            _transaction?.Commit();
            Cleanup();
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            try
            {
                _transaction?.Rollback();
            }
            finally
            {
                Cleanup();
            }
            return Task.CompletedTask;
        }

        private void Cleanup()
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Dispose();
            _connection = null;
        }

        public ValueTask DisposeAsync()
        {
            Cleanup();
            return ValueTask.CompletedTask;
        }
    }
}
