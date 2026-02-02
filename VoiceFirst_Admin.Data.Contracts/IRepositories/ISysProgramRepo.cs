using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysProgramRepo
    {
        Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
                     List<int> sysProgramActionsLinkIds,
                     CancellationToken cancellationToken = default);


        Task<SysProgram?> ExistsByNameAsync(
            int applicationId, 
            string name, 
            int? excludeId = null, 
            CancellationToken cancellationToken = default);
        
        Task<SysProgram?> ExistsByLabelAsync(
            int applicationId, 
            string label,
            int? excludeId = null, 
            CancellationToken cancellationToken = default);
        
        Task<SysProgram?> ExistsByRouteAsync(
            int applicationId, 
            string route, 
            int? excludeId = null, 
            CancellationToken cancellationToken = default);

        Task<bool>
            CheckProgramActionLinksExistAsync(
                         int programId,
                IEnumerable<int> programActionLinkIds,
                bool update,
                IDbConnection connection,
                IDbTransaction transaction,
                CancellationToken cancellationToken = default);


        Task<SysProgram?> GetActiveByIdAsync(
            int id, 
            CancellationToken cancellationToken = default);
       
        Task<SysProgramDto?> GetByIdAsync(
            int id,
            IDbConnection connection,
           IDbTransaction transaction,
            CancellationToken cancellationToken = default);
      
        Task<bool> DeleteAsync(
            int id, 
            int deletedBy, 
            CancellationToken cancellationToken = default);

        Task<bool> BulkInsertActionLinksAsync(        
           int programId,
           IEnumerable<int> actionIds,
           int createdBy,
           IDbConnection connection,
           IDbTransaction transaction,
           CancellationToken cancellationToken);

        Task<bool> BulkUpdateActionLinksAsync(
    int programId,
    IEnumerable<SysProgramActionLinkUpdateDTO> dtos,
    int updatedBy,
    IDbConnection connection,
    IDbTransaction tx,
    CancellationToken cancellationToken);

        Task<int> CreateAsync(
            SysProgram entity, 
            IDbConnection connection,
            IDbTransaction transaction, 
            CancellationToken cancellationToken = default);

        Task<SysProgramDto> IsIdExistAsync(
           int programId,
           CancellationToken cancellationToken = default);

        Task<IEnumerable<SysProgramActionLinkDTO>>
            GetLinksByProgramIdAsync(int programId,
            IDbConnection connection,
           IDbTransaction transaction,
            CancellationToken cancellationToken = default);

        Task<bool> RecoverProgramAsync(
            int id, int loginId, 
            CancellationToken cancellationToken = default);
        

        
       
        Task<PagedResultDto<SysProgramDto>> GetAllAsync(
            SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default);

        Task<List<SysProgramActionLinkLookUp>> 
            GetActionLookupByProgramIdAsync(
            int programId, 
            CancellationToken cancellationToken = default);


        Task<List<SysProgramLookUp>> 
            GetProgramLookupAsync(int? applicationId = null,
            CancellationToken cancellationToken = default);


        Task<bool> UpdateAsync(
            SysProgram entity,
            IDbConnection connection,
           IDbTransaction transaction,
            CancellationToken cancellationToken = default);


        Task<Dictionary<int, bool?>>
          GetExistingProgramActionsAsync(
          int programId,
          IDbConnection connection,
          IDbTransaction transaction,
          CancellationToken cancellationToken = default);

        Task<List<int>> GetInvalidProgramActionLinkIdsForApplicationAsync(
            int applicationId,
            IEnumerable<int> programActionLinkIds,
            CancellationToken cancellationToken = default);
        Task<List<int>> GetInvalidProgramIdsForApplicationAsync(
            int applicationId,
            IEnumerable<int> programIds,
            CancellationToken cancellationToken = default);

        //Task<bool> UpsertProgramActionLinksAsync(
        //    int programId, 
        //    IEnumerable<SysProgramActionLinkUpdateDTO> actions,
        //    int userId,
        //    IDbConnection connection,
        //   IDbTransaction transaction,
        //    CancellationToken cancellationToken = default);
    }
}
