using Dapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class MenuRepo : IMenuRepo
{
    private readonly IDapperContext _context;

    public MenuRepo(IDapperContext context)
    {
        _context = context;
    }
    public async Task<MenuMaster?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM MenuMaster WHERE MenuName = @Name";
        if (excludeId.HasValue)
            sql += " AND MenuMasterId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<MenuMaster>(cmd);
    }
    public async Task<WebMenu?> ExistsMenuMasterByWebAsync(int menuMasterId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM WebMenus WHERE MenuMasterId = @MenuMasterId";
        if (excludeId.HasValue)
            sql += " AND WebMenuId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { MenuMasterId = menuMasterId, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<WebMenu>(cmd);
    }
    public async Task<AppMenus?> ExistsMenuMasterByAppAsync(int menuMasterId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM AppMenus  WHERE MenuMasterId = @MenuMasterId";
        if (excludeId.HasValue)
            sql += " AND AppMenuId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { MenuMasterId = menuMasterId, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AppMenus>(cmd);
    }
    public async Task<IEnumerable<WebMenu>> GetAllWebMenusAsync(CancellationToken cancellationToken = default)
    {
        // Return all web menus without applying filters or paging
        const string sql = @"
            SELECT
                w.WebMenuId,
                w.ParentWebMenuId,
                w.MenuMasterId,
                m.MenuName,
                m.MenuIcon,
                m.MenuRoute,
                w.SortOrder,
                w.IsActive,
                w.IsDeleted,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUserName,
                w.CreatedAt,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS UpdatedUserName,
                w.UpdatedAt,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUserName,
                w.DeletedAt
            FROM dbo.WebMenus w
            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
            INNER JOIN dbo.Users uC ON uC.UserId = w.CreatedBy
            LEFT JOIN dbo.Users uU ON uU.UserId = w.UpdatedBy
            LEFT JOIN dbo.Users uD ON uD.UserId = w.DeletedBy
            WHERE m.IsDeleted = 0
            ORDER BY w.SortOrder;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<WebMenu>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        var list = items.ToList();
        return list;
    }
    public async Task<IEnumerable<AppMenus>> GetAllAppMenusAsync(CancellationToken cancellationToken = default)
    {
        // Return all web menus without applying filters or paging
        const string sql = @"
            SELECT
                w.AppMenuId,
                w.ParentAppMenuId,
                w.MenuMasterId,
                m.MenuName,
                m.MenuIcon,
                m.MenuRoute,
                w.SortOrder,
                w.IsActive,
                w.IsDeleted,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUserName,
                w.CreatedAt,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS UpdatedUserName,
                w.UpdatedAt,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUserName,
                w.DeletedAt
            FROM dbo.AppMenus w
            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
            INNER JOIN dbo.Users uC ON uC.UserId = w.CreatedBy
            LEFT JOIN dbo.Users uU ON uU.UserId = w.UpdatedBy
            LEFT JOIN dbo.Users uD ON uD.UserId = w.DeletedBy
            WHERE m.IsDeleted=0
            ORDER BY w.SortOrder;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<AppMenus>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        var list = items.ToList();
        return list;
    }
    public async Task<MenuMaster> GetMenuMastersByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT
                m.MenuMasterId,
                m.MenuName,
                m.MenuIcon AS MenuIcon,
                m.MenuRoute AS MenuRoute,
                m.ApplicationId AS ApplicationId,
                m.IsActive AS Active,
                m.IsDeleted AS Deleted,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                m.CreatedAt AS CreatedDate,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                m.UpdatedAt AS ModifiedDate,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
                m.DeletedAt AS DeletedDate FROM dbo.MenuMaster m
            INNER JOIN dbo.Users uC ON uC.UserId = m.CreatedBy
            LEFT JOIN dbo.Users uU ON uU.UserId = m.UpdatedBy
            LEFT JOIN dbo.Users uD ON uD.UserId = m.DeletedBy
            WHERE MenuMasterId=@MenuMasterId";

        using var connection = _context.CreateConnection();
        var cmd = new CommandDefinition(sql, new { MenuMasterId = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<MenuMaster>(cmd);
    }
    public async Task<IEnumerable<MenuProgramLink>> GetAllMenuProrgamByMenuMastersIdAsync(int menuMastersId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT
                m.MenuProgramLinkId,
                m.MenuMasterId,
                m.ProgramId,
                m.IsPrimaryProgram,
                sp.ProgramName,
                sp.ProgramRoute,
                m.IsActive AS Active,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                m.CreatedAt AS CreatedDate,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                m.UpdatedAt AS ModifiedDate FROM dbo.MenuProgramLink m
            INNER JOIN dbo.Users uC ON uC.UserId = m.CreatedBy
            INNER JOIN dbo.SysProgram sp ON sp.SysProgramId = m.ProgramId
            LEFT JOIN dbo.Users uU ON uU.UserId = m.UpdatedBy
            WHERE MenuMasterId=@MenuMasterId";

        using var connection = _context.CreateConnection();
        var cmd = new CommandDefinition(sql, new { MenuMasterId = menuMastersId }, cancellationToken: cancellationToken);
        return await connection.QueryAsync<MenuProgramLink>(cmd);
    }
    public async Task<PagedResultDto<MenuMaster>> GetAllMenuMastersAsync(MenuFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new System.Text.StringBuilder(@"FROM dbo.MenuMaster m
            INNER JOIN dbo.Users uC ON uC.UserId = m.CreatedBy
            LEFT JOIN dbo.Users uU ON uU.UserId = m.UpdatedBy
            LEFT JOIN dbo.Users uD ON uD.UserId = m.DeletedBy
            WHERE 1=1 ");

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND m.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }
        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND m.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        // filter by ApplicationId when provided
        if (filter is MenuFilterDto mf && mf.PlateFormId.HasValue)
        {
            baseSql.Append(" AND m.ApplicationId = @ApplicationId");
            parameters.Add("ApplicationId", mf.PlateFormId.Value);
        }
        var searchByMap = new Dictionary<MenuSearchBy, string>
        {
            [MenuSearchBy.MenuName] = "m.MenuName",
            [MenuSearchBy.MenuRoute] = "m.MenuRoute",
            [MenuSearchBy.MenuIcon] = "m.MenuIcon",
            [MenuSearchBy.CreatedUser] = "CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, ''))",
            [MenuSearchBy.UpdatedUser] = "CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, ''))",
            [MenuSearchBy.DeletedUser] = "CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, ''))",
        };
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            // allow searching by specific column when requested


            // If a specific SearchBy is provided and maps to a column, use it
            if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
            {
                baseSql.Append($" AND {col} LIKE @Search");
            }
            else
            {
                // Default: search across everything (name + route + icon + users + ids)
                baseSql.Append(@" AND (
                    m.MenuName LIKE @Search
                 OR m.MenuRoute LIKE @Search
                 OR m.MenuIcon LIKE @Search
                 OR CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) LIKE @Search
                 OR CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) LIKE @Search
                 OR CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')) LIKE @Search");

                parameters.Add("Search", $"%{filter.SearchText}%");


                baseSql.Append(")");
            }
        }

        var sortMap = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ["MenuMasterId"] = "m.MenuMasterId",
            ["MenuName"] = "m.MenuName",
            ["MenuRoute"] = "m.MenuRoute",
            ["Active"] = "po.IsActive",
            ["Deleted"] = "po.IsDeleted",
            ["CreatedDate"] = "po.CreatedAt",
            ["ModifiedDate"] = "po.UpdatedAt",
            ["DeletedDate"] = "po.DeletedAt",
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "MenuMasterId" : filter.SortBy;
        if (!sortMap.TryGetValue(sortKey, out var sortColumn)) sortColumn = sortMap["MenuMasterId"];

        var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        var itemsSql = $@"
            SELECT
                m.MenuMasterId,
                m.MenuName,
                m.MenuIcon AS MenuIcon,
                m.MenuRoute AS MenuRoute,
                m.ApplicationId AS ApplicationId,
                m.IsActive AS Active,
                m.IsDeleted AS Deleted,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                m.CreatedAt AS CreatedDate,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                m.UpdatedAt AS ModifiedDate,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
                m.DeletedAt AS DeletedDate
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var items = await connection.QueryAsync<MenuMaster>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

        return new PagedResultDto<MenuMaster>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }
    public async Task<int> CreateMenuAsync(MenuMaster menu,List<MenuProgramLink>? programIds,bool web,bool app,int loginId,CancellationToken cancellationToken = default)
    {
        const string insertMenuSql = @"
        INSERT INTO dbo.MenuMaster (MenuName, MenuIcon, MenuRoute, ApplicationId, CreatedBy)
        VALUES (@MenuName, @MenuIcon, @MenuRoute, @ApplicationId, @CreatedBy);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            var menuId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                insertMenuSql,
                new { menu.MenuName, menu.MenuIcon, menu.MenuRoute, menu.ApplicationId, CreatedBy = loginId },
                transaction: tx,
                cancellationToken: cancellationToken));

            if (web)
                await InsertWebMenuAsync(connection, tx, menuId, loginId, cancellationToken);

            if (app)
                await InsertAppMenuAsync(connection, tx, menuId, loginId, cancellationToken);

            if (programIds != null && programIds.Count > 0)
            {
                await InsertMenuProgramLinksAsync(connection, tx, menuId, programIds, loginId, cancellationToken);

            }

            tx.Commit();
            return menuId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    public async Task<BulkUpsertError?> UpdateMenuMasterAsync(MenuMaster entity,List<MenuProgramLink>? addProgramIds,List<MenuProgramLink>? updateProgramIds,CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();

        using var tx = connection.BeginTransaction();
        try
        {
            // 1) Update MenuMaster (partial update)
            var affected = await UpdateMenuMasterFieldsAsync(connection, tx, entity, cancellationToken);

            // optional: if not found / not updated
            if (affected == 0)
                return new BulkUpsertError { Message = "Menu not found.", StatuaCode = StatusCodes.Status404NotFound };

            // 2) Read the current state inside same tx
            var current = await GetMenuMasterByIdAsync(connection, tx, entity.MenuMasterId, cancellationToken);

            // 3) Decide if it is "main menu" (no route) OR you want to delete links when ApplicationId changes
            bool isMainMenuNow = string.IsNullOrWhiteSpace(current.MenuRoute);

            // If you actually mean: "when ApplicationId provided (changed) => remove programs"
            bool appIdChangedOrProvided = entity.ApplicationId != 0;

            if (isMainMenuNow || appIdChangedOrProvided)
            {
                await DeleteMenuProgramLinksAsync(connection, tx, entity.MenuMasterId, entity.UpdatedBy ?? 0, cancellationToken);
            }
            else
            {
                if (addProgramIds is { Count: > 0 })
                    await InsertMenuProgramLinksAsync(connection, tx, entity.MenuMasterId, addProgramIds, entity.UpdatedBy ?? 0, cancellationToken);

                if (updateProgramIds is { Count: > 0 })
                    await UpdateMenuProgramLinksAsync(connection, tx, updateProgramIds, entity.UpdatedBy ?? 0, cancellationToken);
            }

            tx.Commit();
            return null;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    private static async Task<int> UpdateMenuMasterFieldsAsync(IDbConnection connection,IDbTransaction tx,MenuMaster entity,CancellationToken cancellationToken)
    {
        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("MenuMasterId", entity.MenuMasterId);
        p.Add("UpdatedBy", entity.UpdatedBy);

        if (!string.IsNullOrWhiteSpace(entity.MenuName)) { sets.Add("MenuName = @MenuName"); p.Add("MenuName", entity.MenuName); }
        if (!string.IsNullOrWhiteSpace(entity.MenuIcon)) { sets.Add("MenuIcon = @MenuIcon"); p.Add("MenuIcon", entity.MenuIcon); }
        if (entity.MenuRoute != null) { sets.Add("MenuRoute = @MenuRoute"); p.Add("MenuRoute", entity.MenuRoute); }
        // ^ IMPORTANT: allow empty string to clear route, so check "!= null" not IsNullOrWhiteSpace

        if (entity.ApplicationId != 0) { sets.Add("ApplicationId = @ApplicationId"); p.Add("ApplicationId", entity.ApplicationId); }
        if (entity.IsActive.HasValue) { sets.Add("IsActive = @IsActive"); p.Add("IsActive", entity.IsActive.Value); }

        if (sets.Count == 0) return 0;

        sets.Add("UpdatedAt = SYSDATETIME()");
        sets.Add("UpdatedBy = @UpdatedBy");

        var sql = $@"
            UPDATE dbo.MenuMaster
            SET {string.Join(", ", sets)}
            WHERE MenuMasterId = @MenuMasterId AND IsDeleted = 0;";

        return await connection.ExecuteAsync(new CommandDefinition(sql, p, transaction: tx, cancellationToken: cancellationToken));
    }
    private static async Task<MenuMaster> GetMenuMasterByIdAsync(IDbConnection connection,IDbTransaction tx,int menuMasterId,CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT TOP 1 *
            FROM dbo.MenuMaster
            WHERE MenuMasterId = @MenuMasterId AND IsDeleted = 0;";

        return await connection.QuerySingleAsync<MenuMaster>(new CommandDefinition(
            sql,
            new { MenuMasterId = menuMasterId },
            transaction: tx,
            cancellationToken: cancellationToken));
    }
    private async Task UpdateMenuProgramLinksAsync(IDbConnection connection,IDbTransaction tx,IEnumerable<MenuProgramLink> updateProgramIds,int updatedBy,CancellationToken cancellationToken)
    {
        foreach (var item in updateProgramIds)
        {

            var sets = new List<string>();
            var p = new DynamicParameters();

            p.Add("MenuProgramLinkId", item.MenuProgramLinkId);
            p.Add("UpdatedBy", updatedBy);

            if (item.IsPrimaryProgram.HasValue)
            {
                sets.Add("IsPrimaryProgram = @IsPrimaryProgram");
                p.Add("IsPrimaryProgram", item.IsPrimaryProgram.Value);
            }

            if (item.IsActive.HasValue)
            {
                sets.Add("IsActive = @IsActive");
                p.Add("IsActive", item.IsActive.Value);
            }

            // Always audit
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            // If you want to SKIP when nothing provided besides audit:
            // if (!item.IsPrimaryProgram.HasValue && !item.IsActive.HasValue) continue;

            var sql = $@"
                UPDATE dbo.MenuProgramLink
                SET {string.Join(", ", sets)}
                WHERE MenuProgramLinkId = @MenuProgramLinkId;";

            var updateStatus=await connection.ExecuteAsync(new CommandDefinition(
                sql,
                p,
                transaction: tx,
                cancellationToken: cancellationToken));
            
            
        }
    }
    public async Task<int> CreateWebMenuAsync(int menuMasterId,int createdBy,CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();

        using var tx = connection.BeginTransaction();
        try
        {
            var newId = await InsertWebMenuAsync(connection, tx, menuMasterId, createdBy, cancellationToken);
            tx.Commit();
            return newId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    public async Task<int> CreateAppMenuAsync(int menuMasterId, int createdBy, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();

        using var tx = connection.BeginTransaction();
        try
        {
            var newId = await InsertAppMenuAsync(connection, tx, menuMasterId, createdBy, cancellationToken);
            tx.Commit();
            return newId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    public async Task<int> AddWebMenuAsync(int menuMasterId, int createdBy, int parentWebMenuId = 0, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        IF EXISTS (
            SELECT 1
            FROM dbo.WebMenus
            WHERE MenuMasterId = @MenuMasterId
              AND ParentWebMenuId = @ParentWebMenuId
              AND IsDeleted = 0
        )
        BEGIN
            SELECT 0; -- already exists
            RETURN;
        END;

        INSERT INTO dbo.WebMenus (ParentWebMenuId, MenuMasterId, SortOrder, CreatedBy)
        SELECT
            @ParentWebMenuId,
            @MenuMasterId,
            ISNULL(MAX(w.SortOrder), 100) + 10,
            @CreatedBy
        FROM dbo.WebMenus w WITH (UPDLOCK, HOLDLOCK)
        WHERE w.ParentWebMenuId = @ParentWebMenuId
          AND w.IsDeleted = 0;

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                sql,
                new { MenuMasterId = menuMasterId, ParentWebMenuId = parentWebMenuId, CreatedBy = createdBy },
                transaction: tx,
                cancellationToken: cancellationToken));

            tx.Commit();
            return newId;   // 0 means already existed, otherwise returns WebMenus PK id
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    public async Task<int> AddAppMenuAsync(int menuMasterId, int createdBy, int parentAppMenuId = 0, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        IF EXISTS (
            SELECT 1
            FROM dbo.AppMenus
            WHERE MenuMasterId = @MenuMasterId
              AND ParentAppMenuId = @ParentAppMenuId
              AND IsDeleted = 0
        )
        BEGIN
            SELECT 0; -- already exists
            RETURN;
        END;

        INSERT INTO dbo.AppMenus (ParentAppMenuId, MenuMasterId, SortOrder, CreatedBy)
        SELECT
            @ParentAppMenuId,
            @MenuMasterId,
            ISNULL(MAX(a.SortOrder), 100) + 10,
            @CreatedBy
        FROM dbo.AppMenus a WITH (UPDLOCK, HOLDLOCK)
        WHERE a.ParentAppMenuId = @ParentAppMenuId
          AND a.IsDeleted = 0;

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                sql,
                new
                {
                    MenuMasterId = menuMasterId,
                    ParentAppMenuId = parentAppMenuId,
                    CreatedBy = createdBy
                },
                transaction: tx,
                cancellationToken: cancellationToken));

            tx.Commit();
            return newId; // 0 => already existed
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    public async Task<bool> DeleteMenuProgramLinksAsync(IDbConnection connection, IDbTransaction tx, int menuMasterId, int loginId, CancellationToken cancellationToken = default)
    {
        const string updateParentSql = @"
        UPDATE dbo.MenuProgramLink
        SET IsActive = 0,
            UpdatedBy = @UpdatedBy,
            UpdatedAt = SYSDATETIME()
        WHERE MenuMasterId = @MenuMasterId";

        var status = await connection.ExecuteAsync(new CommandDefinition(
            updateParentSql,
            new { MenuMasterId = menuMasterId, UpdatedBy = loginId },transaction: tx,
            cancellationToken: cancellationToken));
        if (status == 1)
            return true;
        else
            return false;
    }
    private static async Task<int> InsertWebMenuAsync(IDbConnection connection,IDbTransaction tx,int menuMasterId,int createdBy,CancellationToken cancellationToken)
    {
        const string sql = @"
            DECLARE @NextSort INT;

            SELECT @NextSort = ISNULL(MAX(w.SortOrder), 100) + 10
            FROM dbo.WebMenus w WITH (UPDLOCK, HOLDLOCK)
            WHERE w.ParentWebMenuId = 0
              AND w.IsDeleted = 0;

            INSERT INTO dbo.WebMenus (ParentWebMenuId, MenuMasterId, SortOrder, CreatedBy, CreatedAt, IsDeleted)
            OUTPUT INSERTED.WebMenuId
            VALUES (0, @MenuMasterId, @NextSort, @CreatedBy, SYSDATETIME(), 0);";

        var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { MenuMasterId = menuMasterId, CreatedBy = createdBy },
            transaction: tx,
            cancellationToken: cancellationToken));

        return newId;
    }
    private static async Task<int> InsertAppMenuAsync(IDbConnection connection,IDbTransaction tx,int menuMasterId,int createdBy,CancellationToken cancellationToken)
    {
        const string sql = @"
        DECLARE @NextSort INT;

        SELECT @NextSort = ISNULL(MAX(a.SortOrder), 100) + 10
        FROM dbo.AppMenus a WITH (UPDLOCK, HOLDLOCK)
        WHERE a.ParentAppMenuId = 0
          AND a.IsDeleted = 0;

        INSERT INTO dbo.AppMenus (ParentAppMenuId, MenuMasterId, SortOrder, CreatedBy, CreatedAt, IsDeleted)
        OUTPUT INSERTED.AppMenuId
        VALUES (0, @MenuMasterId, @NextSort, @CreatedBy, SYSDATETIME(), 0);";

        var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { MenuMasterId = menuMasterId, CreatedBy = createdBy },
            transaction: tx,
            cancellationToken: cancellationToken));

        return newId;
    }
    private static async Task InsertMenuProgramLinksAsync(IDbConnection connection,IDbTransaction tx,int menuMasterId,IEnumerable<MenuProgramLink>? programLinks,int createdBy,CancellationToken cancellationToken)
    {
        if (programLinks == null) return;

        var list = programLinks
            .Where(x => x.ProgramId > 0)
            .ToList();

        if (list.Count == 0) return;

        const string insertSql = @"
        INSERT INTO dbo.MenuProgramLink (MenuMasterId, ProgramId, IsPrimaryProgram, CreatedBy)
        VALUES (@MenuMasterId, @ProgramId, @IsPrimaryProgram, @CreatedBy);";

        foreach (var link in list)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                insertSql,
                new
                {
                    MenuMasterId = menuMasterId,
                    ProgramId = link.ProgramId,
                    IsPrimaryProgram = link.IsPrimaryProgram,
                    CreatedBy = createdBy
                },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }



    // web menu bulk update

    public async Task<BulkUpsertError?> BulkUpdateWebMenusAsync(WebMenuBulkUpdateDto dto,int loginId,CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();

        using var tx = connection.BeginTransaction();

        try
        {
            if ( dto.MoveAndReorder!=null && dto.MoveAndReorder.Count() > 0 )
            {
                var err = await HandleMoveAndReorderAsync(connection, tx, dto.MoveAndReorder, loginId, cancellationToken);
                if (err != null) { tx.Rollback(); return err; }
            }

            if ( dto.Reorders != null && dto.Reorders.Count() > 0 )
            {
                var err = await HandleReordersAsync(connection, tx, dto.Reorders, loginId, cancellationToken);
                if (err != null) { tx.Rollback(); return err; }
            }

            if ( dto.StatusUpdate != null && dto.StatusUpdate.Count() > 0 )
            {
                await HandleStatusUpdateAsync(connection, tx, dto.StatusUpdate, loginId, cancellationToken);
            }

            tx.Commit();
            return null;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private async Task<BulkUpsertError?> HandleMoveAndReorderAsync(IDbConnection connection,IDbTransaction tx,List<WebMenuMoveDto> items,int loginId,CancellationToken cancellationToken)
    {
        foreach (var m in items)
        {
            await UpdateParentAsync(connection, tx, m.WebMenuId, m.ParentWebMenuId, loginId, cancellationToken);

            if (m.NewOrderUnderToParent is { Count: > 0 })
            {
                var parentId = m.ParentWebMenuId ?? 0;

                var err = await ValidateParentRouteAsync(connection, tx, parentId, cancellationToken);
                if (err != null) return err;

                var step = await InferStepAsync(connection, tx, parentId, cancellationToken);
                var baseOffset = step / 2;

                await ParkByWebMenuIdsAsync(connection, tx, m.NewOrderUnderToParent, loginId, cancellationToken);
                await ApplySortOrdersAsync(connection, tx, m.NewOrderUnderToParent, baseOffset, step, loginId, cancellationToken);
            }
        }

        return null;
    }

    private async Task<BulkUpsertError?> HandleReordersAsync(IDbConnection connection,IDbTransaction tx,List<WebMenuReorderDto> items,int loginId,CancellationToken cancellationToken)
    {
        foreach (var r in items)
        {
            var parentId = r.ParentWebMenuId;

            var err = await ValidateParentRouteAsync(connection, tx, parentId, cancellationToken);
            if (err != null) return err;

            var step = await InferStepAsync(connection, tx, parentId, cancellationToken);
            var baseOffset = step / 2;

            // park ONLY the ids you are going to update, and park them uniquely
            await ParkByWebMenuIdsAsync(connection, tx, r.OrderedIds, loginId, cancellationToken);
            await ApplySortOrdersAsync(connection, tx, r.OrderedIds, baseOffset, step, loginId, cancellationToken);
        }

        return null;
    }

    private async Task HandleStatusUpdateAsync(IDbConnection connection,IDbTransaction tx,List<WebMenuStatusDto> items,int loginId,CancellationToken cancellationToken)
    {
        const string statusSql = @"
            UPDATE dbo.WebMenus
            SET IsActive = @IsActive,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE WebMenuId = @WebMenuId";

        foreach (var s in items)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                statusSql,
                new { IsActive = s.Active, UpdatedBy = loginId, WebMenuId = s.WebMenuId },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }

    private async Task UpdateParentAsync(IDbConnection connection,IDbTransaction tx,int webMenuId,int? parentWebMenuId,int loginId,CancellationToken cancellationToken)
    {
        const string updateParentSql = @"
            UPDATE dbo.WebMenus
            SET ParentWebMenuId = @ParentWebMenuId,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE WebMenuId = @WebMenuId";

        await connection.ExecuteAsync(new CommandDefinition(
            updateParentSql,
            new { ParentWebMenuId = parentWebMenuId, UpdatedBy = loginId, WebMenuId = webMenuId },
            transaction: tx,
            cancellationToken: cancellationToken));
    }

    private async Task<BulkUpsertError?> ValidateParentRouteAsync(IDbConnection connection,IDbTransaction tx,int parentId,CancellationToken cancellationToken)
    {
        // If parentId == 0 means root; allow it
        if (parentId == 0) return null;

        const string parentSql = @"
            SELECT TOP 1 w.WebMenuId, m.MenuRoute
            FROM dbo.WebMenus w
            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
            WHERE w.WebMenuId = @WebMenuId AND w.IsDeleted = 0;";

        var parent = await connection.QuerySingleOrDefaultAsync<(int WebMenuId, string? MenuRoute)>(
            new CommandDefinition(parentSql, new { WebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken));

        if (parent.WebMenuId == 0)
            return new BulkUpsertError { Message = Messages.ParentMenuNotFound, StatuaCode = StatusCodes.Status404NotFound };

        if (!string.IsNullOrWhiteSpace(parent.MenuRoute))
            return new BulkUpsertError { Message = Messages.CannotAddOrUpdate, StatuaCode = StatusCodes.Status406NotAcceptable };

        return null;
    }

    private async Task<int> InferStepAsync(IDbConnection connection,IDbTransaction tx,int parentId,CancellationToken cancellationToken)
    {
        const string selectSiblingsSql = @"
            SELECT SortOrder
            FROM dbo.WebMenus
            WHERE ParentWebMenuId = @ParentWebMenuId AND IsDeleted = 0
            ORDER BY SortOrder";

        var siblingOrders = (await connection.QueryAsync<int>(new CommandDefinition(
            selectSiblingsSql,
            new { ParentWebMenuId = parentId },
            transaction: tx,
            cancellationToken: cancellationToken))).ToList();

        int step = 100;

        if (siblingOrders.Count >= 2)
        {
            var diffs = new List<int>(siblingOrders.Count - 1);
            for (int i = 1; i < siblingOrders.Count; i++)
                diffs.Add(siblingOrders[i] - siblingOrders[i - 1]);

            var avg = (int)Math.Round(diffs.Average());
            if (avg > 0) step = avg;
        }

        return step;
    }

    private async Task ParkByWebMenuIdsAsync(IDbConnection connection,IDbTransaction tx,IList<int> webMenuIds,int loginId,CancellationToken cancellationToken)
    {
        // IMPORTANT: park per WebMenuId (unique), NOT "WHERE ParentWebMenuId=..."
        const string parkSql = @"
            UPDATE dbo.WebMenus
            SET SortOrder = @SortOrder,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE WebMenuId = @WebMenuId";

        int park = -1;
        foreach (var id in webMenuIds)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                parkSql,
                new { SortOrder = park--, UpdatedBy = loginId, WebMenuId = id },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }

    private async Task ApplySortOrdersAsync(IDbConnection connection,IDbTransaction tx,IList<int> orderedIds,int baseOffset,int step,int loginId,CancellationToken cancellationToken)
    {
        const string updateOrderSql = @"
            UPDATE dbo.WebMenus
            SET SortOrder = @SortOrder,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE WebMenuId = @WebMenuId";

        for (int idx = 0; idx < orderedIds.Count; idx++)
        {
            var id = orderedIds[idx];
            var sort = baseOffset + (idx * step);

            await connection.ExecuteAsync(new CommandDefinition(
                updateOrderSql,
                new { SortOrder = sort, UpdatedBy = loginId, WebMenuId = id },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }

    // app menu bulk update


    public async Task<BulkUpsertError?> BulkUpdateAppMenusAsync(AppMenuBulkUpdateDto dto,int loginId,CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();

        using var tx = connection.BeginTransaction();

        try
        {
            if (dto.MoveAndReorder != null && dto.MoveAndReorder is { Count: > 0 })
            {
                var err = await HandleAppMoveAndReorderAsync(connection, tx, dto.MoveAndReorder, loginId, cancellationToken);
                if (err != null) { tx.Rollback(); return err; }
            }

            if (dto.Reorders != null && dto.Reorders is { Count: > 0 })
            {
                var err = await HandleAppReordersAsync(connection, tx, dto.Reorders, loginId, cancellationToken);
                if (err != null) { tx.Rollback(); return err; }
            }

            if (dto.StatusUpdate != null &&  dto.StatusUpdate is { Count: > 0 })
            {
                await HandleAppStatusUpdateAsync(connection, tx, dto.StatusUpdate, loginId, cancellationToken);
            }

            tx.Commit();
            return null;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private async Task<BulkUpsertError?> HandleAppMoveAndReorderAsync(IDbConnection connection,IDbTransaction tx,List<AppMenuMoveDto> items,int loginId,CancellationToken cancellationToken)
    {
        foreach (var m in items)
        {
            await UpdateAppParentAsync(connection, tx, m.AppMenuId, m.ParentAppMenuId, loginId, cancellationToken);

            if (m.NewOrderUnderToParent is { Count: > 0 })
            {
                var parentId = m.ParentAppMenuId ?? 0;

                var err = await ValidateAppParentRouteAsync(connection, tx, parentId, cancellationToken);
                if (err != null) return err;

                var step = await InferAppStepAsync(connection, tx, parentId, cancellationToken);
                var baseOffset = step / 2;

                await ParkByAppMenuIdsAsync(connection, tx, m.NewOrderUnderToParent, loginId, cancellationToken);
                await ApplyAppSortOrdersAsync(connection, tx, m.NewOrderUnderToParent, baseOffset, step, loginId, cancellationToken);
            }
        }

        return null;
    }

    private async Task<BulkUpsertError?> HandleAppReordersAsync(IDbConnection connection,IDbTransaction tx,List<AppMenuReorderDto> items,int loginId,CancellationToken cancellationToken)
    {
        foreach (var r in items)
        {
            var parentId = r.ParentAppMenuId;

            var err = await ValidateAppParentRouteAsync(connection, tx, parentId, cancellationToken);
            if (err != null) return err;

            var step = await InferAppStepAsync(connection, tx, parentId, cancellationToken);
            var baseOffset = step / 2;

            // park ONLY the ids you are going to update, and park them uniquely
            await ParkByAppMenuIdsAsync(connection, tx, r.OrderedIds, loginId, cancellationToken);
            await ApplyAppSortOrdersAsync(connection, tx, r.OrderedIds, baseOffset, step, loginId, cancellationToken);
        }

        return null;
    }

    private async Task HandleAppStatusUpdateAsync(IDbConnection connection,IDbTransaction tx,List<AppMenuStatusDto> items,int loginId,CancellationToken cancellationToken)
    {
        const string statusSql = @"
            UPDATE dbo.AppMenus
            SET IsActive = @IsActive,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE AppMenuId = @AppMenuId";

        foreach (var s in items)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                statusSql,
                new { IsActive = s.Active, UpdatedBy = loginId, AppMenuId = s.AppMenuId },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }

    private async Task UpdateAppParentAsync(IDbConnection connection,IDbTransaction tx,int appMenuId,int? parentWebMenuId,int loginId,CancellationToken cancellationToken)
    {
        const string updateParentSql = @"
            UPDATE dbo.AppMenus
            SET ParentAppMenuId = @ParentAppMenuId,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE AppMenuId = @AppMenuId";

        await connection.ExecuteAsync(new CommandDefinition(
            updateParentSql,
            new { ParentWebMenuId = parentWebMenuId, UpdatedBy = loginId, AppMenuId = appMenuId },
            transaction: tx,
            cancellationToken: cancellationToken));
    }

    private async Task<BulkUpsertError?> ValidateAppParentRouteAsync(IDbConnection connection,IDbTransaction tx,int parentId,CancellationToken cancellationToken)
    {
        // If parentId == 0 means root; allow it
        if (parentId == 0) return null;

        const string parentSql = @"
            SELECT TOP 1 w.AppMenuId, m.MenuRoute
            FROM dbo.AppMenus w
            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
            WHERE w.AppMenuId = @AppMenuId AND w.IsDeleted = 0;";

        var parent = await connection.QuerySingleOrDefaultAsync<(int AppMenuId, string? MenuRoute)>(
            new CommandDefinition(parentSql, new { WebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken));

        if (parent.AppMenuId == 0)
            return new BulkUpsertError { Message = Messages.ParentMenuNotFound, StatuaCode = StatusCodes.Status404NotFound };

        if (!string.IsNullOrWhiteSpace(parent.MenuRoute))
            return new BulkUpsertError { Message = Messages.CannotAddOrUpdate, StatuaCode = StatusCodes.Status406NotAcceptable };

        return null;
    }

    private async Task<int> InferAppStepAsync(IDbConnection connection,IDbTransaction tx,int parentId,CancellationToken cancellationToken)
    {
        const string selectSiblingsSql = @"
            SELECT SortOrder
            FROM dbo.AppMenus
            WHERE ParentAppMenuId = @ParentAppMenuId AND IsDeleted = 0
            ORDER BY SortOrder";

        var siblingOrders = (await connection.QueryAsync<int>(new CommandDefinition(
            selectSiblingsSql,
            new { ParentAppMenuId = parentId },
            transaction: tx,
            cancellationToken: cancellationToken))).ToList();

        int step = 100;

        if (siblingOrders.Count >= 2)
        {
            var diffs = new List<int>(siblingOrders.Count - 1);
            for (int i = 1; i < siblingOrders.Count; i++)
                diffs.Add(siblingOrders[i] - siblingOrders[i - 1]);

            var avg = (int)Math.Round(diffs.Average());
            if (avg > 0) step = avg;
        }

        return step;
    }

    private async Task ParkByAppMenuIdsAsync(IDbConnection connection,IDbTransaction tx,IList<int> webMenuIds,int loginId,CancellationToken cancellationToken)
    {
        // IMPORTANT: park per WebMenuId (unique), NOT "WHERE ParentWebMenuId=..."
        const string parkSql = @"
            UPDATE dbo.AppMenus
            SET SortOrder = @SortOrder,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE AppMenuId = @AppMenuId";

        int park = -1;
        foreach (var id in webMenuIds)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                parkSql,
                new { SortOrder = park--, UpdatedBy = loginId, AppMenuId = id },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }

    private async Task ApplyAppSortOrdersAsync(IDbConnection connection,IDbTransaction tx,IList<int> orderedIds,int baseOffset,int step,int loginId,CancellationToken cancellationToken)
    {
        const string updateOrderSql = @"
            UPDATE dbo.AppMenus
            SET SortOrder = @SortOrder,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE AppMenuId = @AppMenuId";

        for (int idx = 0; idx < orderedIds.Count; idx++)
        {
            var id = orderedIds[idx];
            var sort = baseOffset + (idx * step);

            await connection.ExecuteAsync(new CommandDefinition(
                updateOrderSql,
                new { SortOrder = sort, UpdatedBy = loginId, AppMenuId = id },
                transaction: tx,
                cancellationToken: cancellationToken));
        }
    }



    
}
