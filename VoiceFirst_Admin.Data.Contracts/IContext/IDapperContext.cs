using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VoiceFirst_Admin.Data.Contracts.IContext
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
        IDbConnection CreateConnection(string connectionName);
    }
}
