using Dapper;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysIssueMediaFormatRepo : ISysIssueMediaFormatRepo
    {
        private readonly IDapperContext _ctx;
        public SysIssueMediaFormatRepo(IDapperContext ctx) => _ctx = ctx;

        public async Task<SysIssueMediaFormatDTO> ExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
        { var sql = "SELECT IsDeleted As Deleted, SysIssueMediaFormatId As IssueMediaFormatId FROM SysIssueMediaFormat WHERE IssueMediaFormat = @N"; if (excludeId.HasValue) sql += " AND SysIssueMediaFormatId <> @E"; using var c = _ctx.CreateConnection(); return await c.QueryFirstOrDefaultAsync<SysIssueMediaFormatDTO>(new CommandDefinition(sql, new { N = name, E = excludeId }, cancellationToken: ct)); }

        public async Task<int> CreateAsync(SysIssueMediaFormat e, CancellationToken ct = default)
        { const string sql = "INSERT INTO SysIssueMediaFormat (IssueMediaFormat,CreatedBy) VALUES (@IssueMediaFormat,@CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);"; using var c = _ctx.CreateConnection(); return await c.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { e.IssueMediaFormat, e.CreatedBy }, cancellationToken: ct)); }

        public async Task<SysIssueMediaFormatDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        { const string sql = @"SELECT s.SysIssueMediaFormatId As IssueMediaFormatId, s.IssueMediaFormat, s.IsActive As Active, s.IsDeleted As Deleted, s.CreatedAt As CreatedDate, s.UpdatedAt As ModifiedDate, s.DeletedAt As DeletedDate, CONCAT(cu.FirstName,' ',ISNULL(cu.LastName,'')) AS CreatedUser, CONCAT(uu.FirstName,' ',ISNULL(uu.LastName,'')) AS ModifiedUser, CONCAT(du.FirstName,' ',ISNULL(du.LastName,'')) AS DeletedUser FROM dbo.SysIssueMediaFormat s INNER JOIN dbo.Users cu ON cu.UserId=s.CreatedBy LEFT JOIN dbo.Users uu ON uu.UserId=s.UpdatedBy LEFT JOIN dbo.Users du ON du.UserId=s.DeletedBy WHERE s.SysIssueMediaFormatId=@Id;"; using var c = _ctx.CreateConnection(); return await c.QuerySingleOrDefaultAsync<SysIssueMediaFormatDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)); }

        public async Task<SysIssueMediaFormatDTO> IsIdExistAsync(int id, CancellationToken ct = default)
        { const string sql = "SELECT SysIssueMediaFormatId As IssueMediaFormatId, IsDeleted As Deleted FROM dbo.SysIssueMediaFormat WHERE SysIssueMediaFormatId=@Id;"; using var c = _ctx.CreateConnection(); return await c.QuerySingleOrDefaultAsync<SysIssueMediaFormatDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)); }

        public async Task<PagedResultDto<SysIssueMediaFormatDTO>> GetAllAsync(IssueMediaFormatFilterDTO filter, CancellationToken ct = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber; var limit = filter.Limit <= 0 ? 10 : filter.Limit; var offset = (page - 1) * limit;
            var p = new DynamicParameters(); p.Add("Offset", offset); p.Add("Limit", limit);
            var b = new StringBuilder(@"FROM SysIssueMediaFormat sit INNER JOIN Users uC ON uC.UserId=sit.CreatedBy LEFT JOIN Users uU ON uU.UserId=sit.UpdatedBy LEFT JOIN Users uD ON uD.UserId=sit.DeletedBy WHERE 1=1 ");
            if (filter.Deleted.HasValue) { b.Append(" AND sit.IsDeleted=@IsDeleted"); p.Add("IsDeleted", filter.Deleted.Value); }
            if (filter.Active.HasValue) { b.Append(" AND sit.IsActive=@IsActive"); p.Add("IsActive", filter.Active.Value); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) && DateTime.TryParse(filter.CreatedFromDate, out var cf)) { b.Append(" AND sit.CreatedAt>=@CF"); p.Add("CF", cf); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) && DateTime.TryParse(filter.CreatedToDate, out var ct2)) { b.Append(" AND sit.CreatedAt<DATEADD(day,1,@CT)"); p.Add("CT", ct2.Date); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) && DateTime.TryParse(filter.UpdatedFromDate, out var uf)) { b.Append(" AND sit.UpdatedAt>=@UF"); p.Add("UF", uf); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) && DateTime.TryParse(filter.UpdatedToDate, out var ut)) { b.Append(" AND sit.UpdatedAt<DATEADD(day,1,@UT)"); p.Add("UT", ut.Date); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) && DateTime.TryParse(filter.DeletedFromDate, out var df)) { b.Append(" AND sit.DeletedAt>=@DF"); p.Add("DF", df); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) && DateTime.TryParse(filter.DeletedToDate, out var dt2)) { b.Append(" AND sit.DeletedAt<DATEADD(day,1,@DT)"); p.Add("DT", dt2.Date); }
            var sMap = new Dictionary<IssueMediaFormatSearchBy, string> { [IssueMediaFormatSearchBy.IssueMediaFormat] = "sit.IssueMediaFormat", [IssueMediaFormatSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)", [IssueMediaFormatSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)", [IssueMediaFormatSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)" };
            if (!string.IsNullOrWhiteSpace(filter.SearchText)) { if (filter.SearchBy.HasValue && sMap.TryGetValue(filter.SearchBy.Value, out var col)) b.Append($" AND {col} LIKE @S"); else b.Append(" AND (sit.IssueMediaFormat LIKE @S OR uC.FirstName LIKE @S OR uC.LastName LIKE @S OR uU.FirstName LIKE @S OR uU.LastName LIKE @S OR uD.FirstName LIKE @S OR uD.LastName LIKE @S)"); p.Add("S", $"%{filter.SearchText}%"); }
            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["IssueMediaFormatId"] = "sit.SysIssueMediaFormatId", ["IssueMediaFormat"] = "sit.IssueMediaFormat", ["Active"] = "sit.IsActive", ["Deleted"] = "sit.IsDeleted", ["CreatedDate"] = "sit.CreatedAt", ["ModifiedDate"] = "sit.UpdatedAt", ["DeletedDate"] = "sit.DeletedAt" };
            var so = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC"; var sk = string.IsNullOrWhiteSpace(filter.SortBy) ? "IssueMediaFormat" : filter.SortBy; if (!sortMap.TryGetValue(sk, out var sc)) sc = sortMap["IssueMediaFormatId"];
            using var c = _ctx.CreateConnection();
            var total = await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(1) " + b, p, cancellationToken: ct));
            var items = await c.QueryAsync<SysIssueMediaFormatDTO>(new CommandDefinition($"SELECT sit.SysIssueMediaFormatId AS IssueMediaFormatId, sit.IssueMediaFormat, sit.IsActive AS Active, sit.IsDeleted AS Deleted, sit.CreatedAt AS CreatedDate, sit.UpdatedAt AS ModifiedDate, sit.DeletedAt AS DeletedDate, CONCAT(uC.FirstName,' ',ISNULL(uC.LastName,'')) AS CreatedUser, ISNULL(CONCAT(uU.FirstName,' ',ISNULL(uU.LastName,'')),'') AS ModifiedUser, ISNULL(CONCAT(uD.FirstName,' ',ISNULL(uD.LastName,'')),'') AS DeletedUser {b} ORDER BY {sc} {so} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;", p, cancellationToken: ct));
            return new PagedResultDto<SysIssueMediaFormatDTO> { Items = items.ToList(), TotalCount = total, PageNumber = page, PageSize = limit };
        }

        public async Task<List<SysIssueMediaFormatActiveDTO?>> GetActiveAsync(CancellationToken ct = default)
        { const string sql = "SELECT SysIssueMediaFormatId As IssueMediaFormatId, IssueMediaFormat FROM dbo.SysIssueMediaFormat WHERE IsDeleted=0 AND IsActive=1 ORDER BY IssueMediaFormat ASC;"; using var c = _ctx.CreateConnection(); return (await c.QueryAsync<SysIssueMediaFormatActiveDTO?>(new CommandDefinition(sql, cancellationToken: ct))).ToList(); }

        public async Task<bool> UpdateAsync(SysIssueMediaFormat entity, CancellationToken ct = default)
        { var sets = new List<string>(); var p = new DynamicParameters(); p.Add("IssueMediaFormat", entity.IssueMediaFormat); p.Add("Active", entity.IsActive.HasValue ? (entity.IsActive.Value ? 1 : 0) : (int?)null); if (!string.IsNullOrWhiteSpace(entity.IssueMediaFormat)) sets.Add("IssueMediaFormat=@IssueMediaFormat"); if (entity.IsActive.HasValue) sets.Add("IsActive=@Active"); if (sets.Count == 0) return false; sets.Add("UpdatedBy=@UB"); sets.Add("UpdatedAt=SYSDATETIME()"); p.Add("UB", entity.UpdatedBy); p.Add("Id", entity.SysIssueMediaFormatId); var sql = $"UPDATE SysIssueMediaFormat SET {string.Join(",", sets)} WHERE SysIssueMediaFormatId=@Id AND IsDeleted=0 AND ((@IssueMediaFormat IS NOT NULL AND IssueMediaFormat<>@IssueMediaFormat) OR (@Active IS NOT NULL AND IsActive<>@Active));"; using var c = _ctx.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: ct)) > 0; }

        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken ct = default)
        { const string sql = "UPDATE SysIssueMediaFormat SET IsDeleted=1,DeletedAt=SYSDATETIME(),DeletedBy=@DB WHERE SysIssueMediaFormatId=@Id AND IsDeleted=0;"; using var c = _ctx.CreateConnection(); if (c.State != ConnectionState.Open) c.Open(); return await c.ExecuteAsync(new CommandDefinition(sql, new { Id = id, DB = deletedBy }, cancellationToken: ct)) > 0; }

        public async Task<bool> RecoverAsync(int id, int loginId, CancellationToken ct = default)
        { const string sql = "UPDATE SysIssueMediaFormat SET IsDeleted=0,DeletedBy=NULL,DeletedAt=NULL,UpdatedBy=@LI,UpdatedAt=SYSDATETIME() WHERE SysIssueMediaFormatId=@Id AND IsDeleted=1"; using var c = _ctx.CreateConnection(); if (c.State != ConnectionState.Open) c.Open(); return await c.ExecuteAsync(new CommandDefinition(sql, new { Id = id, LI = loginId }, cancellationToken: ct)) > 0; }

        public async Task<BulkValidationResult> IsBulkIdsExistAsync(IEnumerable<int> ids, CancellationToken ct = default)
        { var result = new BulkValidationResult(); if (ids == null || !ids.Any()) return result; const string sql = "SELECT SysIssueMediaFormatId, IsActive, IsDeleted FROM SysIssueMediaFormat WHERE SysIssueMediaFormatId IN @Ids;"; using var c = _ctx.CreateConnection(); var entities = (await c.QueryAsync<dynamic>(new CommandDefinition(sql, new { Ids = ids }, cancellationToken: ct))).ToList(); return new BulkValidationResult { IdNotFound = entities.Count != ids.Distinct().Count(), IsDeleted = entities.Any(e => (bool)e.IsDeleted), IsInactive = entities.Any(e => !(bool)e.IsActive) }; }
    }
}
