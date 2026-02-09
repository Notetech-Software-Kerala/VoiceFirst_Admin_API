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

    public async Task<int> CreateMenuAsync(
    MenuMaster menu,
    List<MenuProgramLink>? programIds,
    bool web,
    bool app,
    int loginId,
    CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<WebMenu>> GetAllWebMenusAsync( CancellationToken cancellationToken = default)
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
    public async Task<IEnumerable<AppMenus>> GetAllAppMenusAsync( CancellationToken cancellationToken = default)
    {
        // Return all web menus without applying filters or paging
        const string sql = @"
            SELECT
                w.AppMenuId,
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

// Add GetAllMenuMastersAsync implementation to return master-only list
// Implemented on the same class (MenuRepo) - add method
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
        var searchByMap = new Dictionary<MenuSearchBy ,string>
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

    public async Task<bool> UpdateMenuMasterAsync(VoiceFirst_Admin.Utilities.Models.Entities.MenuMaster entity, CancellationToken cancellationToken = default)
    {
        var sets = new List<string>();
        var p = new DynamicParameters();
        p.Add("MenuMasterId", entity.MenuMasterId);
        p.Add("UpdatedBy", entity.UpdatedBy);

        if (!string.IsNullOrWhiteSpace(entity.MenuName)) { sets.Add("MenuName = @MenuName"); p.Add("MenuName", entity.MenuName); }
        if (!string.IsNullOrWhiteSpace(entity.MenuIcon)) { sets.Add("MenuIcon = @MenuIcon"); p.Add("MenuIcon", entity.MenuIcon); }
        if (!string.IsNullOrWhiteSpace(entity.MenuRoute)) { sets.Add("MenuRoute = @MenuRoute"); p.Add("MenuRoute", entity.MenuRoute); }
        if (entity.ApplicationId != 0) { sets.Add("ApplicationId = @ApplicationId"); p.Add("ApplicationId", entity.ApplicationId); }
        if (entity.IsActive.HasValue) { sets.Add("IsActive = @IsActive"); p.Add("IsActive", entity.IsActive.Value); }

        if (sets.Count == 0) return false;

        sets.Add("UpdatedAt = SYSDATETIME()");
        var sql = $"UPDATE dbo.MenuMaster SET {string.Join(", ", sets)} WHERE MenuMasterId = @MenuMasterId AND IsDeleted = 0";
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: cancellationToken));
        return affected > 0;
    }

    private static async Task InsertWebMenuAsync(
    IDbConnection connection,
    IDbTransaction tx,
    int menuMasterId,
    int createdBy,
    CancellationToken cancellationToken)
    {
        const string sql = @"
        INSERT INTO dbo.WebMenus (ParentWebMenuId, MenuMasterId, SortOrder, CreatedBy)
        SELECT
            0,
            @MenuMasterId,
            ISNULL(MAX(w.SortOrder), 100) + 10,
            @CreatedBy
        FROM dbo.WebMenus w WITH (UPDLOCK, HOLDLOCK)
        WHERE w.ParentWebMenuId = 0;"; 

    await connection.ExecuteAsync(new CommandDefinition(
        sql,
        new { MenuMasterId = menuMasterId, CreatedBy = createdBy },
        transaction: tx,
        cancellationToken: cancellationToken));
    }

    private static async Task InsertAppMenuAsync(
    IDbConnection connection,
    IDbTransaction tx,
    int menuMasterId,
    int createdBy,
    CancellationToken cancellationToken)
    {
        const string sql = @"
        INSERT INTO dbo.AppMenus (ParentAppMenuId, MenuMasterId, SortOrder, CreatedBy)
        SELECT
            0,
            @MenuMasterId,
            ISNULL(MAX(a.SortOrder), 100) + 10,
            @CreatedBy
        FROM dbo.AppMenus a WITH (UPDLOCK, HOLDLOCK)
        WHERE a.ParentAppMenuId = 0;"; 

    await connection.ExecuteAsync(new CommandDefinition(
        sql,
        new { MenuMasterId = menuMasterId, CreatedBy = createdBy },
        transaction: tx,
        cancellationToken: cancellationToken));
    }
    private static async Task InsertMenuProgramLinksAsync(
    IDbConnection connection,
    IDbTransaction tx,
    int menuMasterId,
    IEnumerable<MenuProgramLink>? programLinks,
    int createdBy,
    CancellationToken cancellationToken)
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
    public async Task<int> AddWebMenuAsync(
    int menuMasterId,
    int createdBy,
    int parentWebMenuId = 0,
    CancellationToken cancellationToken = default)
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
    public async Task<int> AddAppMenuAsync(
    int menuMasterId,
    int createdBy,
    int parentAppMenuId = 0,
    CancellationToken cancellationToken = default)
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

    public async Task<BulkUpsertError> BulkUpdateWebMenusAsync(WebMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            // Move and reorder: update parent then reorder children ordering (SortOrder)
            if (dto.MoveAndReorder != null)
            {
                foreach (var m in dto.MoveAndReorder)
                {
                    // update parent
                    const string updateParentSql = @"UPDATE dbo.WebMenus SET ParentWebMenuId = @ParentWebMenuId, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE WebMenuId = @WebMenuId";
                    await connection.ExecuteAsync(new CommandDefinition(updateParentSql, new { ParentWebMenuId = m.ParentWebMenuId, UpdatedBy = loginId, WebMenuId = m.WebMenuId }, transaction: tx, cancellationToken: cancellationToken));

                    // reorder children based on provided ordered list
                    if (m.NewOrderUnderToParent != null && m.NewOrderUnderToParent.Count > 0)
                    {
                        var parentId = m.ParentWebMenuId ?? 0;
                        if (parentId != 0)
                        {
                            const string parentSql = @"
                            SELECT TOP 1 w.WebMenuId, m.MenuRoute
                            FROM dbo.WebMenus w
                            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
                            WHERE w.WebMenuId = @WebMenuId AND w.IsDeleted = 0;";

                            var parent = await connection.QuerySingleOrDefaultAsync<(int WebMenuId, string? MenuRoute)>(
                                new CommandDefinition(parentSql, new { WebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken)
                            );

                            if (parent.WebMenuId == 0) // not found
                                return new BulkUpsertError { Message = Messages.ParentMenuNotFound, StatuaCode = StatusCodes.Status404NotFound };
                           

                            if (!string.IsNullOrWhiteSpace(parent.MenuRoute))
                                return new BulkUpsertError { Message = Messages.CannotAddOrUpdate, StatuaCode = StatusCodes.Status406NotAcceptable };
                        }
                        // get sibling sort orders to infer spacing
                        const string selectSiblingsSql = @"SELECT SortOrder FROM dbo.WebMenus WHERE ParentWebMenuId = @ParentWebMenuId AND IsDeleted = 0 ORDER BY SortOrder";
                        var siblingOrders = (await connection.QueryAsync<int>(new CommandDefinition(selectSiblingsSql, new { ParentWebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken))).ToList();

                        int step = 100; // default spacing
                        if (siblingOrders.Count >= 2)
                        {
                            var diffs = new List<int>();
                            for (int i = 1; i < siblingOrders.Count; i++)
                                diffs.Add(siblingOrders[i] - siblingOrders[i - 1]);
                            var avg = (int)System.Math.Round(diffs.Average());
                            if (avg > 0) step = avg;
                        }

                        int baseOffset = step / 2; // yields 50 when step == 100

                        const string updateOrderSql = @"UPDATE dbo.WebMenus SET SortOrder = @SortOrder, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE WebMenuId = @WebMenuId";
                        for (int idx = 0; idx < m.NewOrderUnderToParent.Count; idx++)
                        {
                            var id = m.NewOrderUnderToParent[idx];
                            var sort = baseOffset + (idx * step);
                            await connection.ExecuteAsync(new CommandDefinition(updateOrderSql, new { SortOrder = sort, UpdatedBy = loginId, WebMenuId = id }, transaction: tx, cancellationToken: cancellationToken));
                        }
                    }
                }
            }

            // Reorders: just reorder children under parent
            if (dto.Reorders != null)
            {
                foreach (var r in dto.Reorders)
                {
                    // NOTE: pick the correct parent id for the group you are reordering.
                    // Assuming r.ParentWebMenuId exists (like your move snippet).
                    var parentId = r.ParentWebMenuId;
                    if (parentId != 0)
                    {
                        const string parentSql = @"
                            SELECT TOP 1 w.WebMenuId, m.MenuRoute
                            FROM dbo.WebMenus w
                            INNER JOIN dbo.MenuMaster m ON m.MenuMasterId = w.MenuMasterId
                            WHERE w.WebMenuId = @WebMenuId AND w.IsDeleted = 0;";

                        var parent = await connection.QuerySingleOrDefaultAsync<(int WebMenuId, string? MenuRoute)>(
                            new CommandDefinition(parentSql, new { WebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken)
                        );

                        if (parent.WebMenuId == 0) // not found
                            return new BulkUpsertError { Message = Messages.ParentMenuNotFound, StatuaCode = StatusCodes.Status404NotFound };


                        if (!string.IsNullOrWhiteSpace(parent.MenuRoute))
                            return new BulkUpsertError { Message = Messages.CannotAddOrUpdate, StatuaCode = StatusCodes.Status406NotAcceptable };
                    }
                    // get sibling sort orders to infer spacing
                    const string selectSiblingsSql = @"
                        SELECT SortOrder
                        FROM dbo.WebMenus
                        WHERE ParentWebMenuId = @ParentWebMenuId AND IsDeleted = 0
                        ORDER BY SortOrder";

                    var siblingOrders = (await connection.QueryAsync<int>(
                        new CommandDefinition(selectSiblingsSql, new { ParentWebMenuId = parentId }, transaction: tx, cancellationToken: cancellationToken)
                    )).ToList();

                    int step = 100; // default spacing
                    if (siblingOrders.Count >= 2)
                    {
                        var diffs = new List<int>();
                        for (int i = 1; i < siblingOrders.Count; i++)
                            diffs.Add(siblingOrders[i] - siblingOrders[i - 1]);

                        var avg = (int)System.Math.Round(diffs.Average());
                        if (avg > 0) step = avg;
                    }

                    int baseOffset = step / 2; // 50 when step == 100
                                               // 1) park them to unique negative values (no collisions)
                    const string parkSql = @"
                        UPDATE dbo.WebMenus
                        SET SortOrder = @SortOrder,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = SYSDATETIME()
                        WHERE ParentWebMenuId = @ParentWebMenuId;";

                    int park = -1;
                    
                        await connection.ExecuteAsync(new CommandDefinition(
                            parkSql,
                            new { SortOrder = park--, UpdatedBy = loginId, ParentWebMenuId = parentId },
                            transaction: tx,
                            cancellationToken: cancellationToken
                        ));
                    
                    const string updateOrderSql = @"
                        UPDATE dbo.WebMenus
                        SET SortOrder = @SortOrder,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = SYSDATETIME()
                        WHERE WebMenuId = @WebMenuId";

                    for (int idx = 0; idx < r.OrderedIds.Count; idx++)
                    {
                        var id = r.OrderedIds[idx];
                        var sort = baseOffset + (idx * step);

                        await connection.ExecuteAsync(new CommandDefinition(
                            updateOrderSql,
                            new { SortOrder = sort, UpdatedBy = loginId, WebMenuId = id },
                            transaction: tx,
                            cancellationToken: cancellationToken
                        ));
                    }
                }
            }

            // Status updates
            if (dto.StatusUpdate != null)
            {
                const string statusSql = @"UPDATE dbo.WebMenus SET IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE WebMenuId = @WebMenuId";
                foreach (var s in dto.StatusUpdate)
                {
                    await connection.ExecuteAsync(new CommandDefinition(statusSql, new { IsActive = s.Active, UpdatedBy = loginId, WebMenuId = s.WebMenuId }, transaction: tx, cancellationToken: cancellationToken));
                }
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

   
    
}
