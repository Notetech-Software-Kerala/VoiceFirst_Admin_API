using Dapper;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysIssueMediaTypeRepo : ISysIssueMediaTypeRepo
    {
        private readonly IDapperContext _ctx;
        public SysIssueMediaTypeRepo(IDapperContext ctx) => _ctx = ctx;

        public async Task<SysIssueMediaTypeDTO> ExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
        { var sql = "SELECT IsDeleted As Deleted, SysIssueMediaTypeId As IssueMediaTypeId FROM SysIssueMediaType WHERE IssueMediaType = @N"; if (excludeId.HasValue) sql += " AND SysIssueMediaTypeId <> @E"; using var c = _ctx.CreateConnection(); return await c.QueryFirstOrDefaultAsync<SysIssueMediaTypeDTO>(new CommandDefinition(sql, new { N = name, E = excludeId }, cancellationToken: ct)); }

        public async Task<int> CreateAsync(SysIssueMediaType e, CancellationToken ct = default)
        { const string sql = "INSERT INTO SysIssueMediaType (IssueMediaType,CreatedBy) VALUES (@IssueMediaType,@CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);"; using var c = _ctx.CreateConnection(); return await c.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { e.IssueMediaType, e.CreatedBy }, cancellationToken: ct)); }

        public async Task<SysIssueMediaTypeDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        { const string sql = @"SELECT s.SysIssueMediaTypeId As IssueMediaTypeId, s.IssueMediaType, s.IsActive As Active, s.IsDeleted As Deleted, s.CreatedAt As CreatedDate, s.UpdatedAt As ModifiedDate, s.DeletedAt As DeletedDate, CONCAT(cu.FirstName,' ',ISNULL(cu.LastName,'')) AS CreatedUser, CONCAT(uu.FirstName,' ',ISNULL(uu.LastName,'')) AS ModifiedUser, CONCAT(du.FirstName,' ',ISNULL(du.LastName,'')) AS DeletedUser FROM dbo.SysIssueMediaType s INNER JOIN dbo.Users cu ON cu.UserId=s.CreatedBy LEFT JOIN dbo.Users uu ON uu.UserId=s.UpdatedBy LEFT JOIN dbo.Users du ON du.UserId=s.DeletedBy WHERE s.SysIssueMediaTypeId=@Id;"; using var c = _ctx.CreateConnection(); return await c.QuerySingleOrDefaultAsync<SysIssueMediaTypeDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)); }

        public async Task<SysIssueMediaTypeDTO> IsIdExistAsync(int id, CancellationToken ct = default)
        { const string sql = "SELECT SysIssueMediaTypeId As IssueMediaTypeId, IsDeleted As Deleted FROM dbo.SysIssueMediaType WHERE SysIssueMediaTypeId=@Id;"; using var c = _ctx.CreateConnection(); return await c.QuerySingleOrDefaultAsync<SysIssueMediaTypeDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)); }

        public async Task<PagedResultDto<SysIssueMediaTypeDTO>> GetAllAsync(IssueMediaTypeFilterDTO filter, CancellationToken ct = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber; var limit = filter.Limit <= 0 ? 10 : filter.Limit; var offset = (page - 1) * limit;
            var p = new DynamicParameters(); p.Add("Offset", offset); p.Add("Limit", limit);
            var b = new StringBuilder(@"FROM SysIssueMediaType sit INNER JOIN Users uC ON uC.UserId=sit.CreatedBy LEFT JOIN Users uU ON uU.UserId=sit.UpdatedBy LEFT JOIN Users uD ON uD.UserId=sit.DeletedBy WHERE 1=1 ");
            if (filter.Deleted.HasValue) { b.Append(" AND sit.IsDeleted=@IsDeleted"); p.Add("IsDeleted", filter.Deleted.Value); }
            if (filter.Active.HasValue) { b.Append(" AND sit.IsActive=@IsActive"); p.Add("IsActive", filter.Active.Value); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) && DateTime.TryParse(filter.CreatedFromDate, out var cf)) { b.Append(" AND sit.CreatedAt>=@CF"); p.Add("CF", cf); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) && DateTime.TryParse(filter.CreatedToDate, out var ct2)) { b.Append(" AND sit.CreatedAt<DATEADD(day,1,@CT)"); p.Add("CT", ct2.Date); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) && DateTime.TryParse(filter.UpdatedFromDate, out var uf)) { b.Append(" AND sit.UpdatedAt>=@UF"); p.Add("UF", uf); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) && DateTime.TryParse(filter.UpdatedToDate, out var ut)) { b.Append(" AND sit.UpdatedAt<DATEADD(day,1,@UT)"); p.Add("UT", ut.Date); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) && DateTime.TryParse(filter.DeletedFromDate, out var df)) { b.Append(" AND sit.DeletedAt>=@DF"); p.Add("DF", df); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) && DateTime.TryParse(filter.DeletedToDate, out var dt2)) { b.Append(" AND sit.DeletedAt<DATEADD(day,1,@DT)"); p.Add("DT", dt2.Date); }
            var sMap = new Dictionary<IssueMediaTypeSearchBy, string> { [IssueMediaTypeSearchBy.IssueMediaType] = "sit.IssueMediaType", [IssueMediaTypeSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)", [IssueMediaTypeSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)", [IssueMediaTypeSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)" };
            if (!string.IsNullOrWhiteSpace(filter.SearchText)) { if (filter.SearchBy.HasValue && sMap.TryGetValue(filter.SearchBy.Value, out var col)) b.Append($" AND {col} LIKE @S"); else b.Append(" AND (sit.IssueMediaType LIKE @S OR uC.FirstName LIKE @S OR uC.LastName LIKE @S OR uU.FirstName LIKE @S OR uU.LastName LIKE @S OR uD.FirstName LIKE @S OR uD.LastName LIKE @S)"); p.Add("S", $"%{filter.SearchText}%"); }
            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["IssueMediaTypeId"] = "sit.SysIssueMediaTypeId", ["IssueMediaType"] = "sit.IssueMediaType", ["Active"] = "sit.IsActive", ["Deleted"] = "sit.IsDeleted", ["CreatedDate"] = "sit.CreatedAt", ["ModifiedDate"] = "sit.UpdatedAt", ["DeletedDate"] = "sit.DeletedAt" };
            var so = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC"; var sk = string.IsNullOrWhiteSpace(filter.SortBy) ? "IssueMediaType" : filter.SortBy; if (!sortMap.TryGetValue(sk, out var sc)) sc = sortMap["IssueMediaTypeId"];
            using var c = _ctx.CreateConnection();
            var total = await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(1) " + b, p, cancellationToken: ct));
            var items = await c.QueryAsync<SysIssueMediaTypeDTO>(new CommandDefinition($"SELECT sit.SysIssueMediaTypeId AS IssueMediaTypeId, sit.IssueMediaType, sit.IsActive AS Active, sit.IsDeleted AS Deleted, sit.CreatedAt AS CreatedDate, sit.UpdatedAt AS ModifiedDate, sit.DeletedAt AS DeletedDate, CONCAT(uC.FirstName,' ',ISNULL(uC.LastName,'')) AS CreatedUser, ISNULL(CONCAT(uU.FirstName,' ',ISNULL(uU.LastName,'')),'') AS ModifiedUser, ISNULL(CONCAT(uD.FirstName,' ',ISNULL(uD.LastName,'')),'') AS DeletedUser {b} ORDER BY {sc} {so} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;", p, cancellationToken: ct));
            return new PagedResultDto<SysIssueMediaTypeDTO> { Items = items.ToList(), TotalCount = total, PageNumber = page, PageSize = limit };
        }

        public async Task<List<SysIssueMediaTypeActiveDTO?>> GetActiveAsync(CancellationToken ct = default)
        { const string sql = "SELECT SysIssueMediaTypeId As IssueMediaTypeId, IssueMediaType FROM dbo.SysIssueMediaType WHERE IsDeleted=0 AND IsActive=1 ORDER BY IssueMediaType ASC;"; using var c = _ctx.CreateConnection(); return (await c.QueryAsync<SysIssueMediaTypeActiveDTO?>(new CommandDefinition(sql, cancellationToken: ct))).ToList(); }

        public async Task<bool> UpdateAsync(SysIssueMediaType entity, CancellationToken ct = default)
        { var sets = new List<string>(); var p = new DynamicParameters(); p.Add("IssueMediaType", entity.IssueMediaType); p.Add("Active", entity.IsActive.HasValue ? (entity.IsActive.Value ? 1 : 0) : (int?)null); if (!string.IsNullOrWhiteSpace(entity.IssueMediaType)) sets.Add("IssueMediaType=@IssueMediaType"); if (entity.IsActive.HasValue) sets.Add("IsActive=@Active"); if (sets.Count == 0) return false; sets.Add("UpdatedBy=@UB"); sets.Add("UpdatedAt=SYSDATETIME()"); p.Add("UB", entity.UpdatedBy); p.Add("Id", entity.SysIssueMediaTypeId); var sql = $"UPDATE SysIssueMediaType SET {string.Join(",", sets)} WHERE SysIssueMediaTypeId=@Id AND IsDeleted=0 AND ((@IssueMediaType IS NOT NULL AND IssueMediaType<>@IssueMediaType) OR (@Active IS NOT NULL AND IsActive<>@Active));"; using var c = _ctx.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: ct)) > 0; }

        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken ct = default)
        { const string sql = "UPDATE SysIssueMediaType SET IsDeleted=1,DeletedAt=SYSDATETIME(),DeletedBy=@DB WHERE SysIssueMediaTypeId=@Id AND IsDeleted=0;"; using var c = _ctx.CreateConnection(); if (c.State != ConnectionState.Open) c.Open(); return await c.ExecuteAsync(new CommandDefinition(sql, new { Id = id, DB = deletedBy }, cancellationToken: ct)) > 0; }

        public async Task<bool> RecoverAsync(int id, int loginId, CancellationToken ct = default)
        { const string sql = "UPDATE SysIssueMediaType SET IsDeleted=0,DeletedBy=NULL,DeletedAt=NULL,UpdatedBy=@LI,UpdatedAt=SYSDATETIME() WHERE SysIssueMediaTypeId=@Id AND IsDeleted=1"; using var c = _ctx.CreateConnection(); if (c.State != ConnectionState.Open) c.Open(); return await c.ExecuteAsync(new CommandDefinition(sql, new { Id = id, LI = loginId }, cancellationToken: ct)) > 0; }
    }
}
