using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using VoiceFirst_Admin.Data.Contracts.IContext;



namespace VoiceFirst_Admin.Data.Context
{
    public class DapperContext: IDapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IDbConnection CreateConnection()
        {
            return CreateConnection("DefaultConnection");
        }

        public IDbConnection CreateConnection(string connectionName)
        {
            var connectionString = _configuration.GetConnectionString(connectionName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{connectionName}' not found.");
            }

            return new SqlConnection(connectionString);
        }
    }
}
