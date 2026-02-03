using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IUserRepo
    {
        Task<int> CreateUserAsync
            (
            Users user,
            IDbConnection connection,
            IDbTransaction transaction
            );
    }
}
