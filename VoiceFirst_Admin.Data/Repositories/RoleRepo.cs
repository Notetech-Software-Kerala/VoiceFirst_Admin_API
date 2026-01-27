using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class RoleRepo : IRoleRepo
{
    private readonly IDapperContext _context;

    public RoleRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<SysRoles> CreateAsync(SysRoles entity, List<int> actionLinkIds,  CancellationToken cancellationToken = default)
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

            if (id > 0 && actionLinkIds != null && actionLinkIds.Any())
            {
                await BulkInsertActionLinksAsync(connection,tx, id, actionLinkIds, entity.CreatedBy,cancellationToken);
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

    public async Task<PagedResultDto<SysRoles>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
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
            LEFT JOIN Users uD ON uD.UserId = r.DeletedBy WHERE 1=1");

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
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            baseSql.Append(" AND (r.RoleName LIKE @Search OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search)");
            parameters.Add("Search", $"%{filter.SearchText}%");
        }

        var countSql = "SELECT COUNT(1) " + baseSql;
        var itemsSql = $@"SELECT r.SysRoleId, r.RoleName, r.IsMandatory, r.RolePurpose, r.ApplicationId, r.CreatedAt, r.IsActive, r.UpdatedAt, r.IsDeleted, r.DeletedAt,
        uC.UserId AS CreatedById, CONCAT(uC.FirstName,' ',uC.LastName) AS CreatedUserName,
        uU.UserId AS UpdatedById, CONCAT(uU.FirstName,' ',uU.LastName) AS UpdatedUserName,
        uD.UserId AS DeletedById, CONCAT(uD.FirstName,' ',uD.LastName) AS DeletedUserName
        {baseSql}
        ORDER BY r.SysRoleId ASC
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
            await connection.ExecuteAsync(new CommandDefinition("UPDATE SysRolesProgramActionLink SET IsActive = 0, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE SysRoleId = @RoleId;", new { RoleId = entity.SysRoleId, UpdatedBy=entity.UpdatedBy },cancellationToken: cancellationToken));
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

    public async Task<IEnumerable<SysRolesProgramActionLink>> GetActionIdsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
                spa.SysRoleProgramActionLinkId ,
                spa.ProgramActionLinkId ,
                pal.ProgramActionId,
                pa.ProgramActionName,
                spa.IsActive,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                spa.CreatedAt AS CreatedAt,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                spa.UpdatedAt AS UpdatedAt
            FROM SysRolesProgramActionLink spa
            LEFT JOIN SysProgramActionsLink pal ON pal.SysProgramActionLinkId = spa.ProgramActionLinkId
            LEFT JOIN SysProgramActions pa ON pa.SysProgramActionId = pal.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
            WHERE spa.SysRoleId = @RoleId And spa.isActive=1 ";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<SysRolesProgramActionLink>(new CommandDefinition(sql, new { RoleId = roleId }, cancellationToken: cancellationToken));
        return items.ToList();
    }

    public async Task<BulkUpsertError?> BulkUpsertRoleActionLinksAsync(int roleId, int applicationId, IEnumerable<int> actionIds,int loginId, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        string? currentAction = null;
        try
        {
            
            // load existing role->action links
            var existingLinks = (await connection.QueryAsync<SysRolesProgramActionLink>(new CommandDefinition("SELECT * FROM SysRolesProgramActionLink WHERE SysRoleId = @RoleId;", new { RoleId = roleId }, transaction: tx, cancellationToken: cancellationToken))).ToList();

            var existingMap = existingLinks.ToDictionary(x => x.ProgramActionLinkId, x => x);

            var incoming = actionIds?.ToList() ?? new List<int>();

            // process incoming: if exists update IsActive=true, otherwise insert
            foreach (var aid in incoming)
            {
                currentAction = aid.ToString();

                if (existingMap.TryGetValue(aid, out var existingLink))
                {
                    // if current IsActive is not true, update it
                    if (existingLink.IsActive != true)
                    {
                        const string updateSql = "UPDATE SysRolesProgramActionLink SET IsActive = 1, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE SysRoleId = @SysRoleId AND ProgramActionLinkId = @ProgramActionLinkId;";
                        await connection.ExecuteAsync(new CommandDefinition(updateSql, new { UpdatedBy = loginId, SysRoleId = roleId, ProgramActionLinkId = aid }, transaction: tx, cancellationToken: cancellationToken));
                    }
                    else
                    {
                        const string updateSql = "UPDATE SysRolesProgramActionLink SET IsActive = 0, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE SysRoleId = @SysRoleId AND ProgramActionLinkId = @ProgramActionLinkId;";
                        await connection.ExecuteAsync(new CommandDefinition(updateSql, new { UpdatedBy = loginId, SysRoleId = roleId, ProgramActionLinkId = aid }, transaction: tx, cancellationToken: cancellationToken));
                    }
                }
                else
                {
                    const string insertSql = "INSERT INTO SysRolesProgramActionLink (SysRoleId, ProgramActionLinkId, CreatedBy) VALUES (@SysRoleId, @ProgramActionLinkId, @CreatedBy);";
                    await connection.ExecuteAsync(new CommandDefinition(insertSql, new { SysRoleId = roleId, ProgramActionLinkId = aid, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
                }
            }

           
            tx.Commit();
            connection.Close();
            return null;
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            tx.Rollback();
            connection.Close();
            return new BulkUpsertError { Message = $"Action '{currentAction ?? "(unknown)"}' already exists.", StatuaCode = StatusCodes.Status409Conflict };
        }
        catch
        {
            tx.Rollback();
            connection.Close();
            throw;
        }
    }
    private async Task BulkInsertActionLinksAsync(IDbConnection connection, IDbTransaction tx, int roleId, IEnumerable<int> actionIds, int createdBy, CancellationToken cancellationToken)
    {
        const string insertLink = @"INSERT INTO SysRolesProgramActionLink (SysRoleId, ProgramActionLinkId, CreatedBy) VALUES (@SysRoleId, @ProgramActionLinkId, @CreatedBy);";
        foreach (var actionId in actionIds)
        {
            await connection.ExecuteAsync(new CommandDefinition(insertLink, new { SysRoleId = roleId, ProgramActionLinkId = actionId, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
        }
    }
}
