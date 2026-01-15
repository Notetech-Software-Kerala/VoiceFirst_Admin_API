using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VoiceFirst_Admin.Data.Context
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
        IDbConnection CreateConnection(string connectionName);
    }
}
