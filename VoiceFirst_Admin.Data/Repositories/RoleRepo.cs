using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static Dapper.SqlMapper;

namespace VoiceFirst_Admin.Data.Repositories;

public class RoleRepo : IRoleRepo
{
    private readonly IDapperContext _context;

    public RoleRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<SysRoles> CreateAsync(SysRoles entity, List<PlanActionLinkCreateDto> PlanActionLinkCreateDto,  CancellationToken cancellationToken = default)
    {
        const string sql = @"INSERT INTO SysRoles (RoleName, IsMandatory, RolePurpose, ApplicationId, CreatedBy) VALUES (@RoleName, @IsMandatory, @RolePurpose, @ApplicationId, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
        using var connection = _context.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        string? currentAction = null;
        try
        {
            
            var cmd = new CommandDefinition(sql, new
            {
                entity.RoleName,
                entity.IsMandatory,
                entity.RolePurpose,
                entity.ApplicationId,
                entity.CreatedBy
            }, transaction: tx, cancellationToken: cancellationToken);
            
            var id = await connection.ExecuteScalarAsync<int>(cmd);

            entity.SysRoleId = id;

            if (id > 0 && PlanActionLinkCreateDto != null && PlanActionLinkCreateDto.Any())
            {
                foreach (var plan in PlanActionLinkCreateDto)
                {
                    var planRoleId=await InsertRolePlanLinksAsync(connection, tx, id, plan.PlanId, entity.CreatedBy, cancellationToken);
                    if (planRoleId > 0)
                    {
                        await BulkInsertPlanRoleActionLinksAsync(connection, tx, planRoleId, plan.ActionLinkIds, entity.CreatedBy, cancellationToken);
                    }
                    else
                    {
                        tx.Rollback();
                        connection.Close();
                        return null;
                    }
                }
                
            }




            tx.Commit();
            connection.Close();
            return entity;
        }
        
        catch
        {
            tx.Rollback();
            connection.Close();
            throw;
        }
            
       
    }

    public async Task<SysRoles?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT r.SysRoleId, r.RoleName, r.IsMandatory, r.RolePurpose, r.ApplicationId, r.IsActive, r.IsDeleted, r.CreatedAt, r.UpdatedAt, r.DeletedAt,
        uC.UserId AS CreatedById, CONCAT(uC.FirstName,' ',uC.LastName) AS CreatedUserName,
        uU.UserId AS UpdatedById, CONCAT(uU.FirstName,' ',uU.LastName) AS UpdatedUserName,
        uD.UserId AS DeletedById, CONCAT(uD.FirstName,' ',uD.LastName) AS DeletedUserName
        FROM SysRoles r
        INNER JOIN Users uC ON uC.UserId = r.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = r.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = r.DeletedBy
        WHERE r.SysRoleId = @Id;";
                using var connection = _context.CreateConnection();
        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<SysRoles>(cmd);
    }
    public async Task<IEnumerable<SysRoles>> GetLookUpAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT r.SysRoleId, r.RoleName
        FROM SysRoles r
        WHERE r.IsActive = 1 And r.IsDeleted=0 AND r.SysRoleId > 5;;";
        using var connection = _context.CreateConnection();
        var cmd = new CommandDefinition(sql, cancellationToken: cancellationToken);
        return await connection.QueryAsync<SysRoles>(cmd);
    }
    public async Task<PagedResultDto<SysRoles>> GetAllAsync(RoleFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"FROM SysRoles r
            INNER JOIN Users uC ON uC.UserId = r.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = r.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = r.DeletedBy WHERE r.SysRoleId > 5 ");

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND r.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }
        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND r.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }
        // support optional SearchBy for role-specific fields
        var searchByMap = new Dictionary<RoleSearchBy, string>
        {
            [RoleSearchBy.RoleName] = "r.RoleName",
            [RoleSearchBy.RolePurpose] = "r.RolePurpose",
            [RoleSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
            [RoleSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
            [RoleSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
        };

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            if (filter is RoleFilterDto roleFilter && roleFilter.SearchBy.HasValue && searchByMap.TryGetValue(roleFilter.SearchBy.Value, out var col))
            {
                baseSql.Append($" AND {col} LIKE @Search");
            }
            else
            {
                // Default: search across role name, purpose and related users
                baseSql.Append(@" AND (
                       r.RoleName LIKE @Search
                    OR r.RolePurpose LIKE @Search
                    OR CONCAT(uC.FirstName,' ',uC.LastName) LIKE @Search
                    OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                    OR CONCAT(uU.FirstName,' ',uU.LastName) LIKE @Search
                    OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                    OR CONCAT(uD.FirstName,' ',uD.LastName) LIKE @Search
                )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SysRoleId"] = "r.SysRoleId",
            ["RoleName"] = "r.RoleName",
            ["Active"] = "r.IsActive",
            ["Deleted"] = "r.IsDeleted",
            ["CreatedDate"] = "r.CreatedAt",
            ["ModifiedDate"] = "r.UpdatedAt",
            ["DeletedDate"] = "r.DeletedAt",
        };
        var sortOrder = filter.SortOrder == Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "SysRoleId" : filter.SortBy;

        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["SysRoleId"];

        var countSql = "SELECT COUNT(1) " + baseSql;
        var itemsSql = $@"SELECT r.SysRoleId, r.RoleName, r.IsMandatory, r.RolePurpose, r.ApplicationId, r.CreatedAt, r.IsActive, r.UpdatedAt, r.IsDeleted, r.DeletedAt,
        uC.UserId AS CreatedById, CONCAT(uC.FirstName,' ',uC.LastName) AS CreatedUserName,
        uU.UserId AS UpdatedById, CONCAT(uU.FirstName,' ',uU.LastName) AS UpdatedUserName,
        uD.UserId AS DeletedById, CONCAT(uD.FirstName,' ',uD.LastName) AS DeletedUserName
        {baseSql}
         ORDER BY {sortColumn} {sortOrder}
        OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var items = await connection.QueryAsync<SysRoles>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

        return new PagedResultDto<SysRoles>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }

    public async Task<bool> UpdateAsync(SysRoles entity, CancellationToken cancellationToken = default)
    {
        var sets = new List<string>();
        var parameters = new DynamicParameters();
        using var connection = _context.CreateConnection();
        if (!string.IsNullOrWhiteSpace(entity.RoleName))
        {
            sets.Add("RoleName = @RoleName");
            parameters.Add("RoleName", entity.RoleName);
        }
        if (entity.IsMandatory != default)
        {
            sets.Add("IsMandatory = @IsMandatory");
            parameters.Add("IsMandatory", entity.IsMandatory ? 1 : 0);
        }
        if (entity.RolePurpose != null)
        {
            sets.Add("RolePurpose = @RolePurpose");
            parameters.Add("RolePurpose", entity.RolePurpose);
        }
        if (entity.ApplicationId != default)
        {
            await connection.ExecuteAsync(new CommandDefinition("UPDATE PlanRoleProgramActionLink SET IsDeleted = 1, DeletedBy = @UpdatedBy, DeletedAt = SYSDATETIME() WHERE SysRoleId = @RoleId;", new { RoleId = entity.SysRoleId, UpdatedBy=entity.UpdatedBy },cancellationToken: cancellationToken));
            sets.Add("ApplicationId = @ApplicationId");
            parameters.Add("ApplicationId", entity.ApplicationId);
        }
      
        if (sets.Count == 0)
            return false;

        sets.Add("UpdatedBy = @UpdatedBy");
        sets.Add("UpdatedAt = SYSDATETIME()");
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        parameters.Add("SysRoleId", entity.SysRoleId);

        var sql = new StringBuilder();
        sql.Append("UPDATE SysRoles SET ");
        sql.Append(string.Join(", ", sets));
        sql.Append(" WHERE SysRoleId = @SysRoleId AND IsDeleted = 0;");

        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }


    public async Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
   List<int> roleIds,
   CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, bool>
    {
        { "idNotFound", false },
        { "deletedOrInactive", false }
    };



        if (roleIds == null || roleIds.Count == 0)
            return result;

        const string sql = @"
        SELECT 
            SysRoleId,
            IsActive,
            IsDeleted
        FROM SysRoles
        WHERE SysRoleId IN @Ids;
        ";

        using var connection = _context.CreateConnection();

        var entities = (await connection.QueryAsync<SysProgramActions>(
            new CommandDefinition(
                sql,
                new { Ids = roleIds },
                cancellationToken: cancellationToken)))
            .ToList();

        // 1?? Check NOT FOUND
        if (entities.Count != roleIds.Distinct().Count())
        {
            result["idNotFound"] = true;
        }

        // 2?? Check Deleted or Inactive
        if (entities.Any(x => x.IsDeleted == true || x.IsActive == false))
        {
            result["deletedOrInactive"] = true;
        }

        return result;
    }

    public async Task<bool> DeleteAsync(SysRoles entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE SysRoles SET IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy=@DeletedBy WHERE SysRoleId = @SysRoleId AND IsDeleted = 0;";
        var parameters = new DynamicParameters();
        parameters.Add("SysRoleId", entity.SysRoleId);
        parameters.Add("DeletedBy", entity.DeletedBy);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> RestoreAsync(SysRoles entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE SysRoles SET IsDeleted = 0, DeletedAt = NULL, DeletedBy = NULL, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE SysRoleId = @SysRoleId AND IsDeleted = 1;";
        var parameters = new DynamicParameters();
        parameters.Add("SysRoleId", entity.SysRoleId);
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<SysRoles> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM SysRoles WHERE RoleName = @Name";
        if (excludeId.HasValue)
            sql += " AND SysRoleId <> @ExcludeId";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SysRoles>(new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<PlanRoleProgramActionLink>> GetActionIdsByRoleIdAsync(int roleId, int planId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT spa.SysRoleId ,
                spa.PlanRoleLinkId ,
                pra.PlanRoleProgramActionLinkId ,
                pra.ProgramActionLinkId ,
                pa.SysProgramActionId,
                pa.ProgramActionName,
                pra.IsActive,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                pra.CreatedAt AS CreatedAt,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                pra.UpdatedAt AS UpdatedAt
            FROM PlanRoleLink spa 
            Inner join PlanRoleProgramActionLink pra on pra.PlanRoleLinkId=spa.PlanRoleLinkId
            LEFT JOIN SysProgramActionsLink pal ON pal.SysProgramActionLinkId = pra.ProgramActionLinkId
            LEFT JOIN SysProgramActions pa ON pa.SysProgramActionId = pal.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = pra.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = pra.UpdatedBy
            WHERE spa.SysRoleId = @RoleId And spa.PlanId = @PlanId And spa.isActive=1 ";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<PlanRoleProgramActionLink>(new CommandDefinition(sql, new { RoleId = roleId , PlanId= planId }, cancellationToken: cancellationToken));
        return items.ToList();
    }

    public async Task<BulkUpsertError?> AddRoleActionLinksAsync(
        int roleId,
        int applicationId,
        List<PlanActionLinkCreateDto> planActionLink,
        int loginId,
        CancellationToken cancellationToken = default)
        {

        const string sql = @"SELECT 
                pra.ProgramActionLinkId ,
                spa.PlanRoleLinkId 
            FROM PlanRoleLink spa 
            LEFT JOIN PlanRoleProgramActionLink pra on pra.PlanRoleLinkId=spa.PlanRoleLinkId
            LEFT JOIN SysProgramActionsLink pal ON pal.SysProgramActionLinkId = pra.ProgramActionLinkId
            LEFT JOIN SysProgramActions pa ON pa.SysProgramActionId = pal.ProgramActionId
            WHERE spa.SysRoleId = @RoleId And spa.PlanId = @PlanId ";


            using var connection = _context.CreateConnection();
                connection.Open();
                using var tx = connection.BeginTransaction();

            try
            {
                if (planActionLink.Count() == 0)
                    return new BulkUpsertError { Message = Messages.BadRequest, StatuaCode = StatusCodes.Status400BadRequest };
                foreach (var item in planActionLink)
                {
                    
                    var existingLinks = (await connection.QueryAsync<PlanRoleProgramActionLink>(new CommandDefinition(sql, new { RoleId = roleId, PlanId = item.PlanId }, transaction: tx, cancellationToken: cancellationToken))).ToList();
                var planRoleId = 0;
                    if (existingLinks.Count() == 0)
                    {
                        planRoleId =await InsertRolePlanLinksAsync(connection, tx, roleId, item.PlanId, loginId, cancellationToken);
                    }
                    else
                    {
                        planRoleId= existingLinks.First().PlanRoleLinkId;

                    }

                    var existingMap = existingLinks.ToDictionary(x => x.ProgramActionLinkId, x => x);
                    if (item.ActionLinkIds.Count() > 0)
                    {
                        foreach (var aid in item.ActionLinkIds)
                        {
                            if (existingMap.TryGetValue(aid, out var existingLink))
                            {
                                return new BulkUpsertError { Message = Messages.AlreadyExist, StatuaCode = StatusCodes.Status409Conflict };
                            }
                            await BulkInsertPlanRoleActionLinksAsync(connection, tx, planRoleId, item.ActionLinkIds, loginId, cancellationToken);
                        }
                    } 

                }


                tx.Commit();
                return null;
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                tx.Rollback();
                return new BulkUpsertError { Message = ex.Message, StatuaCode = StatusCodes.Status400BadRequest };
            }
        }
        public async Task<BulkUpsertError?> UpdateRoleActionLinksAsync(
        int roleId,
        int applicationId,
         List<PlanRoleActionLinkUpdateDto>? UpdateActionLinks,
        int loginId,
        CancellationToken cancellationToken = default)
        {

        const string sql = @"SELECT 
                pra.ProgramActionLinkId ,
                spa.PlanRoleLinkId 
                FROM PlanRoleLink spa 
                LEFT JOIN PlanRoleProgramActionLink pra on pra.PlanRoleLinkId=spa.PlanRoleLinkId
                LEFT JOIN SysProgramActionsLink pal ON pal.SysProgramActionLinkId = pra.ProgramActionLinkId
                LEFT JOIN SysProgramActions pa ON pa.SysProgramActionId = pal.ProgramActionId
                INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
                WHERE spa.PlanRoleLinkId = @PlanRoleLinkId ";

        using var connection = _context.CreateConnection();
             connection.Open();
             using var tx = connection.BeginTransaction();

            try
            {
                
                if (UpdateActionLinks == null || UpdateActionLinks.Count() == 0)
                    return new BulkUpsertError { Message = Messages.BadRequest, StatuaCode = StatusCodes.Status400BadRequest };
                foreach (var item in UpdateActionLinks)
                {
                    var existingLinks = (await connection.QueryAsync<PlanRoleProgramActionLink>(
                        new CommandDefinition(sql,
                        new { RoleId = roleId, PlanRoleLinkId = item.RolePlanLinkId }, transaction: tx, cancellationToken: cancellationToken))).ToList();

                    var existingMap = existingLinks.ToDictionary(x => x.ProgramActionLinkId, x => x);



                    // process incoming: if exists update IsActive=true, otherwise insert
                    foreach (var aid in item.UpdateActionLinks)
                    {

                        if (existingMap.TryGetValue(aid.ActionLinkId, out var existingLink))
                        {
                            // if current IsActive is not true, update it

                            const string updateSql = "UPDATE PlanRoleProgramActionLink SET IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE PlanRoleLinkId = @PlanRoleLinkId AND ProgramActionLinkId = @ProgramActionLinkId;";
                            await connection.ExecuteAsync(new CommandDefinition(updateSql, new { UpdatedBy = loginId, PlanRoleLinkId = item.RolePlanLinkId, ProgramActionLinkId = aid.ActionLinkId, IsActive = aid.Active }, transaction: tx, cancellationToken: cancellationToken));

                        }
                        else
                        {
                            return new BulkUpsertError { Message = Messages.NotFound, StatuaCode = StatusCodes.Status404NotFound };
                        }
                    }

                }
                // load existing role->action links


                tx.Commit();
                return null;
        }
        catch (SqlException ex) when (ex.Number == 50001 || ex.Number == 50002)
        {
             tx.Rollback();
            return new BulkUpsertError { Message = ex.Message, StatuaCode = StatusCodes.Status400BadRequest };
        }
    }

    private async Task<int> InsertRolePlanLinksAsync(IDbConnection connection, IDbTransaction tx, int roleId,int planId, int createdBy, CancellationToken cancellationToken)
    {
        const string insertLink = @"INSERT INTO PlanRoleLink (SysRoleId, PlanId, CreatedBy) VALUES (@SysRoleId, @PlanId, @CreatedBy);SELECT CAST(SCOPE_IDENTITY() AS int);";
        var id= await connection.ExecuteScalarAsync<int>(new CommandDefinition(insertLink, new { SysRoleId = roleId, PlanId = planId, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));

        return id;
    }
    private async Task BulkInsertPlanRoleActionLinksAsync(IDbConnection connection, IDbTransaction tx, int PlanRoleLinkId, IEnumerable<int> actionIds, int createdBy, CancellationToken cancellationToken)
    {
        const string insertLink = @"INSERT INTO PlanRoleProgramActionLink (PlanRoleLinkId, ProgramActionLinkId, CreatedBy) VALUES (@PlanRoleLinkId, @ProgramActionLinkId, @CreatedBy);";
        foreach (var actionId in actionIds)
        {
            await connection.ExecuteAsync(new CommandDefinition(insertLink, new { PlanRoleLinkId = PlanRoleLinkId, ProgramActionLinkId = actionId, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
        }
    }

}
