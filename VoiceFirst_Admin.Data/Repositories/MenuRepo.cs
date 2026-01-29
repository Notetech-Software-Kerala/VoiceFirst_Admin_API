using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class MenuRepo : IMenuRepo
{
    private readonly IDapperContext _context;

    public MenuRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateMenuAsync(MenuMaster menu, IEnumerable<int> programIds, MenuWebDto? web, MenuAppDto? app, int loginId, CancellationToken cancellationToken = default)
    {
        const string insertMenuSql = @"INSERT INTO dbo.MenuMaster (MenuName, MenuIcon, MenuRoute, ApplicationId, CreatedBy) VALUES (@MenuName, @MenuIcon, @MenuRoute, @ApplicationId, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            var menuId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(insertMenuSql, new { menu.MenuName, menu.MenuIcon, menu.MenuRoute, menu.ApplicationId, menu.CreatedBy }, transaction: tx, cancellationToken: cancellationToken));

            if (web != null)
            {
                const string insertWeb = @"INSERT INTO dbo.WebMenus (ParentWebMenuId, MenuMasterId, SortOrder, CreatedBy) VALUES (@ParentWebMenuId, @MenuMasterId, @SortOrder, @CreatedBy);";
                await connection.ExecuteAsync(new CommandDefinition(insertWeb, new { ParentWebMenuId = web.ParentId == 0 ? (int?)null : web.ParentId, MenuMasterId = menuId, SortOrder = web.SortOrder, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
            }

            if (app != null)
            {
                const string insertApp = @"INSERT INTO dbo.AppMenus (ParentAppMenuId, MenuMasterId, SortOrder, CreatedBy) VALUES (@ParentAppMenuId, @MenuMasterId, @SortOrder, @CreatedBy);";
                await connection.ExecuteAsync(new CommandDefinition(insertApp, new { ParentAppMenuId = app.ParentId == 0 ? (int?)null : app.ParentId, MenuMasterId = menuId, SortOrder = app.SortOrder, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
            }

            if (programIds != null)
            {
                const string insertLink = @"INSERT INTO dbo.MenuProgramLink (MenuMasterId, ProgramId, IsPrimaryProgram, CreatedBy) VALUES (@MenuMasterId, @ProgramId, 0, @CreatedBy);";
                foreach (var pid in programIds)
                {
                    await connection.ExecuteAsync(new CommandDefinition(insertLink, new { MenuMasterId = menuId, ProgramId = pid, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
                }
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

    public async Task<MenuMaster?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT TOP 1 * FROM dbo.MenuMaster WHERE MenuMasterId = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MenuMaster>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<MenuMaster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM dbo.MenuMaster WHERE IsDeleted = 0";
        using var connection = _context.CreateConnection();
        var rows = await connection.QueryAsync<MenuMaster>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows;
    }

    public async Task<MenuDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // master
        var master = await GetByIdAsync(id, cancellationToken);
        if (master == null) return null;

        var detail = new MenuDetailDto { Master = new MenuMasterDto {
            MenuMasterId = master.MenuMasterId,
            MenuName = master.MenuName,
            MenuIcon = master.MenuIcon,
            MenuRoute = master.MenuRoute,
            ApplicationId = master.ApplicationId,
            Active = master.IsActive ?? false,
            Deleted = master.IsDeleted ?? false,
            CreatedUser = master.CreatedUserName ?? string.Empty,
            CreatedDate = master.CreatedAt ?? System.DateTime.MinValue,
            ModifiedUser = master.UpdatedUserName ?? string.Empty,
            ModifiedDate = master.UpdatedAt,
            DeletedUser = master.DeletedUserName ?? string.Empty,
            DeletedDate = master.DeletedAt
        } };

        using var connection = _context.CreateConnection();
        const string progSql = @"SELECT MenuProgramLinkId, MenuMasterId, ProgramId, IsPrimaryProgram, IsActive AS Active FROM dbo.MenuProgramLink WHERE MenuMasterId = @Id";
        var progs = await connection.QueryAsync<MenuProgramDto>(new CommandDefinition(progSql, new { Id = id }, cancellationToken: cancellationToken));
        detail.Programs = progs.ToList();

        const string webSql = @"SELECT WebMenuId, ParentWebMenuId, SortOrder FROM dbo.WebMenus WHERE MenuMasterId = @Id";
        var web = await connection.QueryFirstOrDefaultAsync<WebMenuDetailDto>(new CommandDefinition(webSql, new { Id = id }, cancellationToken: cancellationToken));
        detail.Web = web;

        const string appSql = @"SELECT AppMenuId, ParentAppMenuId, SortOrder FROM dbo.AppMenus WHERE MenuMasterId = @Id";
        var appm = await connection.QueryFirstOrDefaultAsync<AppMenuDetailDto>(new CommandDefinition(appSql, new { Id = id }, cancellationToken: cancellationToken));
        detail.App = appm;

        return detail;
    }

    public async Task<bool> UpdateMenuAsync(MenuMaster menu, IEnumerable<int> addProgramIds, IEnumerable<ProgramStatusDto> updateProgramIds, MenuWebDto? web, MenuAppDto? app, int loginId, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {
            // update menu master partially
            var sets = new List<string>();
            var p = new DynamicParameters();
            p.Add("MenuMasterId", menu.MenuMasterId);
            p.Add("UpdatedBy", loginId);

            if (!string.IsNullOrWhiteSpace(menu.MenuName)) { sets.Add("MenuName = @MenuName"); p.Add("MenuName", menu.MenuName); }
            if (!string.IsNullOrWhiteSpace(menu.MenuIcon)) { sets.Add("MenuIcon = @MenuIcon"); p.Add("MenuIcon", menu.MenuIcon); }
            if (!string.IsNullOrWhiteSpace(menu.MenuRoute)) { sets.Add("MenuRoute = @MenuRoute"); p.Add("MenuRoute", menu.MenuRoute); }
            if (menu.ApplicationId != 0) { sets.Add("ApplicationId = @ApplicationId"); p.Add("ApplicationId", menu.ApplicationId); }

            if (sets.Any())
            {
                var sql = $"UPDATE dbo.MenuMaster SET {string.Join(", ", sets)}, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE MenuMasterId = @MenuMasterId AND IsDeleted = 0";
                var affected = await connection.ExecuteAsync(new CommandDefinition(sql, p, transaction: tx, cancellationToken: cancellationToken));
                if (affected == 0) { tx.Rollback(); return false; }
            }

            // upsert web/app menu rows: naive approach - delete existing and insert new
            if (web != null)
            {
                const string deleteWeb = @"DELETE FROM dbo.WebMenus WHERE MenuMasterId = @MenuMasterId";
                await connection.ExecuteAsync(new CommandDefinition(deleteWeb, new { MenuMasterId = menu.MenuMasterId }, transaction: tx, cancellationToken: cancellationToken));

                const string insertWeb = @"INSERT INTO dbo.WebMenus (ParentWebMenuId, MenuMasterId, SortOrder, CreatedBy) VALUES (@ParentWebMenuId, @MenuMasterId, @SortOrder, @CreatedBy);";
                await connection.ExecuteAsync(new CommandDefinition(insertWeb, new { ParentWebMenuId = web.ParentId == 0 ? (int?)null : web.ParentId, MenuMasterId = menu.MenuMasterId, SortOrder = web.SortOrder, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
            }

            if (app != null)
            {
                const string deleteApp = @"DELETE FROM dbo.AppMenus WHERE MenuMasterId = @MenuMasterId";
                await connection.ExecuteAsync(new CommandDefinition(deleteApp, new { MenuMasterId = menu.MenuMasterId }, transaction: tx, cancellationToken: cancellationToken));

                const string insertApp = @"INSERT INTO dbo.AppMenus (ParentAppMenuId, MenuMasterId, SortOrder, CreatedBy) VALUES (@ParentAppMenuId, @MenuMasterId, @SortOrder, @CreatedBy);";
                await connection.ExecuteAsync(new CommandDefinition(insertApp, new { ParentAppMenuId = app.ParentId == 0 ? (int?)null : app.ParentId, MenuMasterId = menu.MenuMasterId, SortOrder = app.SortOrder, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
            }

            // add program links
            if (addProgramIds != null)
            {
                const string insertLink = @"INSERT INTO dbo.MenuProgramLink (MenuMasterId, ProgramId, IsPrimaryProgram, CreatedBy) VALUES (@MenuMasterId, @ProgramId, 0, @CreatedBy);";
                foreach (var pid in addProgramIds)
                {
                    await connection.ExecuteAsync(new CommandDefinition(insertLink, new { MenuMasterId = menu.MenuMasterId, ProgramId = pid, CreatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
                }
            }

            // update program link status
            if (updateProgramIds != null)
            {
                const string updateLink = @"UPDATE dbo.MenuProgramLink SET IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE MenuMasterId = @MenuMasterId AND ProgramId = @ProgramId";
                foreach (var u in updateProgramIds)
                {
                    await connection.ExecuteAsync(new CommandDefinition(updateLink, new { MenuMasterId = menu.MenuMasterId, ProgramId = u.ProgramId, IsActive = u.Status, UpdatedBy = loginId }, transaction: tx, cancellationToken: cancellationToken));
                }
            }

            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
