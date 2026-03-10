using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysUserCustomFieldRepo : ISysUserCustomFieldRepo
    {
        private readonly IDapperContext _context;

        public SysUserCustomFieldRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> validations, List<SysUserCustomFieldOptions> options, int createdBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                const string sql = @"INSERT INTO SysUserCustomField (FieldName, FieldKey, FieldDataType, CreatedBy)
                                 VALUES (@FieldName, @FieldKey, @FieldDataType, @CreatedBy);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
                {
                    entity.FieldName,
                    entity.FieldKey,
                    entity.FieldDataType,
                    CreatedBy = createdBy
                }, transaction: tx, cancellationToken: cancellationToken));

                var rows = 0;
                if (validations != null && validations.Any())
                {
                     await InsertSysUserCustomFieldValidationsAsync(connection, tx, id, validations, createdBy, cancellationToken);
                    
                }

                if (options != null && options.Any())
                {
                     await InsertSysUserCustomFieldOptionsAsync(connection, tx, id, options, createdBy, cancellationToken);
                    
                }

                tx.Commit();
                return id;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                return 0;
            }
        }

        public async Task<SysUserCustomField> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
             const string sqlField = @"
        SELECT 
            f.SysUserCustomFieldId,
            f.FieldName,
            f.FieldKey, 
            f.FieldDataType,
            f.IsActive,
            f.IsDeleted,
            f.CreatedBy,
            f.CreatedAt,
            f.UpdatedBy,
            f.UpdatedAt,
            f.DeletedBy,
            f.DeletedAt,
            uC.UserId AS CreatedById,
            CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
            uU.UserId AS UpdatedById,
            CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
            uD.UserId AS DeletedById,
            CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
        FROM SysUserCustomField f
        INNER JOIN Users uC ON uC.UserId = f.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = f.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = f.DeletedBy
        WHERE f.SysUserCustomFieldId = @Id;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { Id = id }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomField> ExistsByFieldKeyAsync(string fieldKey, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT SysUserCustomFieldId, FieldName, FieldKey, FieldDataType, IsActive, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                       FROM SysUserCustomField WHERE FieldKey = @FieldKey;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { FieldKey = fieldKey }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomField> ExistsByFieldNameAndDataTypeAsync(string fieldName, string fieldDataType, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT SysUserCustomFieldId, FieldName, FieldKey, FieldDataType, IsActive, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                       FROM SysUserCustomField WHERE FieldName = @fieldName And FieldDataType = @FieldDataType;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { FieldName = fieldName, FieldDataType= fieldDataType }, cancellationToken: cancellationToken));
            return field;
        }

        public async Task<bool> UpdateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> addValidations, List<SysUserCustomFieldOptions> addOptions, IEnumerable<SysUserCustomFieldValidations> validations, IEnumerable<SysUserCustomFieldOptions> options, int updatedBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                if(!string.IsNullOrWhiteSpace(entity.FieldName) || !string.IsNullOrWhiteSpace(entity.FieldKey)|| !string.IsNullOrWhiteSpace(entity.FieldDataType) || entity.IsActive.HasValue)
                {
                    var sets = new List<string>();
                    var parameters = new DynamicParameters();
                    if (!string.IsNullOrWhiteSpace(entity.FieldName))
                    {
                        sets.Add("FieldName = @FieldName");
                        parameters.Add("FieldName", entity.FieldName);
                    }
                    if (!string.IsNullOrWhiteSpace(entity.FieldKey))
                    {
                        sets.Add("FieldKey = @FieldKey");
                        parameters.Add("FieldKey", entity.FieldKey);
                    }
                    if (!string.IsNullOrWhiteSpace(entity.FieldDataType))
                    {
                        sets.Add("FieldDataType = @FieldDataType");
                        parameters.Add("FieldDataType", entity.FieldDataType);
                    }
                    if (entity.IsActive.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", entity.IsActive);
                    }
                    if (sets.Count != 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");
                        parameters.Add("UpdatedBy", entity.UpdatedBy);
                        parameters.Add("SysUserCustomFieldId", entity.SysUserCustomFieldId);
                        var sql = new StringBuilder();
                        sql.Append("UPDATE SysUserCustomField SET ");
                        sql.Append(string.Join(", ", sets));
                        sql.Append(" WHERE SysUserCustomFieldId = @SysUserCustomFieldId AND IsDeleted = 0;");
                        var cmd = new CommandDefinition(sql.ToString(), parameters, transaction: tx, cancellationToken: cancellationToken);
                         await connection.ExecuteAsync(cmd);
                        
                    }

                }



                if (addValidations != null && addValidations.Any())
                {
                    await InsertSysUserCustomFieldValidationsAsync(connection, tx, entity.SysUserCustomFieldId, addValidations, updatedBy, cancellationToken);
                    
                }

                if (addOptions != null && addOptions.Any())
                {
                    InsertSysUserCustomFieldOptionsAsync(connection, tx, entity.SysUserCustomFieldId, addOptions, updatedBy, cancellationToken);
                   
                }
                if (validations != null && validations.Any())
                {
                    await UpdateSysUserCustomFieldValidationsAsync(connection, tx, validations, updatedBy, cancellationToken);
                    

                }


                if (options != null && options.Any())
                {
                     await UpdateSysUserCustomFieldOptionsAsync(connection, tx, options, updatedBy, cancellationToken);
                   
                }

                tx.Commit();
                return true;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                return false;
            }
        }
        
        public async Task<bool> SoftDeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                const string sql = @"UPDATE SysUserCustomField
                                 SET IsDeleted = 1, DeletedBy = @DeletedBy, DeletedAt = SYSDATETIME()
                                 WHERE SysUserCustomFieldId = @Id;";

                var rows = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, DeletedBy = deletedBy }, transaction: tx, cancellationToken: cancellationToken));

                const string delV = "UPDATE SysUserCustomFieldValidations SET IsActive = 0 WHERE SysUserCustomFieldId = @Id;";
                const string delO = "UPDATE SysUserCustomFieldOptions SET IsActive = 0 WHERE SysUserCustomFieldId = @Id;";
                await connection.ExecuteAsync(new CommandDefinition(delV, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));
                await connection.ExecuteAsync(new CommandDefinition(delO, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));

                tx.Commit();
                return rows > 0;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        private async Task InsertSysUserCustomFieldValidationsAsync(
                    IDbConnection connection,
                    IDbTransaction tx,
                    int sysUserCustomFieldId,
                    IEnumerable<dynamic>? addValidations,
                    int createdBy,
                    CancellationToken cancellationToken)
        {
            if (addValidations == null || !addValidations.Any())
                return ;

            const string sql = @"
                INSERT INTO SysUserCustomFieldValidations
                (
                    SysUserCustomFieldId,
                    RuleName,
                    RuleValue,
                    message,
                    CreatedBy
                )
                VALUES
                (
                    @SysUserCustomFieldId,
                    @RuleName,
                    @RuleValue,
                    @message,
                    @CreatedBy
                );";

            int affectedRows = 0;

            foreach (var v in addValidations)
            {
                

                var parameter = new
                {
                    SysUserCustomFieldId = sysUserCustomFieldId,
                    RuleName = v.RuleName,
                    RuleValue = v.RuleValue,
                    message = v.message,
                    CreatedBy = createdBy
                };

                var cmd = new CommandDefinition(
                    sql,
                    parameter,
                    transaction: tx,
                    cancellationToken: cancellationToken);

                 await connection.ExecuteAsync(cmd);
            }

            
        }
        private async Task InsertSysUserCustomFieldOptionsAsync(
                IDbConnection connection,
                IDbTransaction tx,
                int sysUserCustomFieldId,
                IEnumerable<dynamic>? addOptions,
                int createdBy,
                CancellationToken cancellationToken)
        {
            

            const string sql = @"
            INSERT INTO SysUserCustomFieldOptions
            (
                SysUserCustomFieldId,
                label,
                value,
                CreatedBy
            )
            VALUES
            (
                @SysUserCustomFieldId,
                @label,
                @value,
                @CreatedBy
            );";

        

            foreach (var o in addOptions)
            {
                
                var parameter = new
                {
                    SysUserCustomFieldId = sysUserCustomFieldId,
                    label = o.label,
                    value = o.value,
                    CreatedBy = createdBy
                };

                var cmd = new CommandDefinition(
                    sql,
                    parameter,
                    transaction: tx,
                    cancellationToken: cancellationToken);

                await connection.ExecuteAsync(cmd);
            }

            

            

     
        }
        private async Task UpdateSysUserCustomFieldOptionsAsync(
            IDbConnection connection,
            IDbTransaction tx,
            IEnumerable<dynamic>? options,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (options == null || !options.Any())
                return ;

            foreach (var option in options)
            {
                if (!string.IsNullOrWhiteSpace(option.label) ||
                    option.value != null ||
                    option.IsActive.HasValue)
                {
                    var sets = new List<string>();
                    var parameters = new DynamicParameters();

                    if (!string.IsNullOrWhiteSpace(option.label))
                    {
                        sets.Add("label = @label");
                        parameters.Add("label", option.label);
                    }

                    if (option.value != null)
                    {
                        sets.Add("value = @value");
                        parameters.Add("value", option.value);
                    }

                    if (option.IsActive.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", option.IsActive);
                    }

                    if (sets.Count > 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");

                        parameters.Add("UpdatedBy", updatedBy);
                        parameters.Add("SysUserCustomFieldOptionsId", option.SysUserCustomFieldOptionsId);

                        var sql = new StringBuilder();
                        sql.Append("UPDATE SysUserCustomFieldOptions SET ");
                        sql.Append(string.Join(", ", sets));
                        sql.Append(" WHERE SysUserCustomFieldOptionsId = @SysUserCustomFieldOptionsId;");

                        var cmd = new CommandDefinition(
                            sql.ToString(),
                            parameters,
                            transaction: tx,
                            cancellationToken: cancellationToken);

                        var affected = await connection.ExecuteAsync(cmd);

                        
                    }
                }
            }

            
        }
        private async Task UpdateSysUserCustomFieldValidationsAsync(
            IDbConnection connection,
            IDbTransaction tx,
            IEnumerable<dynamic>? validations,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (validations == null || !validations.Any())
                return ;

            foreach (var validation in validations)
            {
                if (!string.IsNullOrWhiteSpace(validation.RuleName) ||
                    validation.RuleValue != null ||
                    !string.IsNullOrWhiteSpace(validation.message) ||
                    validation.IsActive.HasValue)
                {
                    var sets = new List<string>();
                    var parameters = new DynamicParameters();

                    if (!string.IsNullOrWhiteSpace(validation.RuleName))
                    {
                        sets.Add("RuleName = @RuleName");
                        parameters.Add("RuleName", validation.RuleName);
                    }

                    if (validation.RuleValue != null)
                    {
                        sets.Add("RuleValue = @RuleValue");
                        parameters.Add("RuleValue", validation.RuleValue);
                    }

                    if (!string.IsNullOrWhiteSpace(validation.message))
                    {
                        sets.Add("message = @message");
                        parameters.Add("message", validation.message);
                    }

                    if (validation.IsActive.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", validation.IsActive);
                    }

                    if (sets.Count > 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");

                        parameters.Add("UpdatedBy", updatedBy);
                        parameters.Add("SysUserCustomFieldValidationId", validation.SysUserCustomFieldValidationId);

                        var sql = new StringBuilder();
                        sql.Append("UPDATE SysUserCustomFieldValidations SET ");
                        sql.Append(string.Join(", ", sets));
                        sql.Append(" WHERE SysUserCustomFieldValidationId = @SysUserCustomFieldValidationId;");

                        var cmd = new CommandDefinition(
                            sql.ToString(),
                            parameters,
                            transaction: tx,
                            cancellationToken: cancellationToken);

                         await connection.ExecuteAsync(cmd);

                        
                    }
                }
            }

            
        }

        public async Task<PagedResultDto<SysUserCustomField>> GetAllAsync(CustomFieldFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
                FROM SysUserCustomField f
                INNER JOIN Users uC ON uC.UserId = f.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = f.UpdatedBy
                LEFT JOIN Users uD ON uD.UserId = f.DeletedBy
                WHERE 1=1");

            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND f.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }
            if (filter.Deleted.HasValue)
            {
                baseSql.Append(" AND f.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Deleted.Value);
            }

            var searchByMap = new Dictionary<SysUserCustomFieldSearchBy, string>
            {
                [SysUserCustomFieldSearchBy.FieldName] = "f.FieldName",
                [SysUserCustomFieldSearchBy.FieldKey] = "f.FieldKey",
                [SysUserCustomFieldSearchBy.FieldDataType] = "f.FieldDataType",
                [SysUserCustomFieldSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [SysUserCustomFieldSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [SysUserCustomFieldSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@" AND (f.FieldName LIKE @Search OR f.FieldKey LIKE @Search OR f.FieldDataType LIKE @Search
                        OR CONCAT(uC.FirstName,' ',uC.LastName) LIKE @Search OR CONCAT(uU.FirstName,' ',uU.LastName) LIKE @Search)");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["SysUserCustomFieldId"] = "f.SysUserCustomFieldId",
                ["FieldName"] = "f.FieldName",
                ["FieldKey"] = "f.FieldKey",
                ["FieldDataType"] = "f.FieldDataType",
                ["Active"] = "f.IsActive",
                ["Deleted"] = "f.IsDeleted",
                ["CreatedDate"] = "f.CreatedAt",
                ["ModifiedDate"] = "f.UpdatedAt",
            };

            var sortOrder = filter.SortOrder == VoiceFirst_Admin.Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "SysUserCustomFieldId" : filter.SortBy;
            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["SysUserCustomFieldId"];

            var countSql = "SELECT COUNT(1) " + baseSql;
            var itemsSql = $@"
                SELECT f.SysUserCustomFieldId, f.FieldName, f.FieldKey, f.FieldDataType, f.IsActive, f.IsDeleted, f.CreatedAt, f.UpdatedAt,
                       uC.UserId AS CreatedById, CONCAT(uC.FirstName,' ',uC.LastName) AS CreatedUserName,
                       uU.UserId AS UpdatedById, CONCAT(uU.FirstName,' ',uU.LastName) AS UpdatedUserName,
                       uD.UserId AS DeletedById, CONCAT(uD.FirstName,' ',uD.LastName) AS DeletedUserName
                {baseSql}
                ORDER BY {sortColumn} {sortOrder}
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<SysUserCustomField>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysUserCustomField>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }
        public async Task<PagedResultDto<SysUserCustomField>> GetLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
                FROM SysUserCustomField f
                INNER JOIN Users uC ON uC.UserId = f.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = f.UpdatedBy
                LEFT JOIN Users uD ON uD.UserId = f.DeletedBy
                WHERE Where IsDeleted=0 And IsActive=1");

            

            var searchByMap = new Dictionary<SysUserCustomFieldSearchBy, string>
            {
                [SysUserCustomFieldSearchBy.FieldName] = "f.FieldName",
                [SysUserCustomFieldSearchBy.FieldKey] = "f.FieldKey",
                [SysUserCustomFieldSearchBy.FieldDataType] = "f.FieldDataType",
                [SysUserCustomFieldSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [SysUserCustomFieldSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [SysUserCustomFieldSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                baseSql.Append(@" AND (f.FieldName LIKE @Search OR f.FieldKey LIKE @Search OR f.FieldDataType LIKE @Search
                        OR CONCAT(uC.FirstName,' ',uC.LastName) LIKE @Search OR CONCAT(uU.FirstName,' ',uU.LastName) LIKE @Search)");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            

            
            

            var countSql = "SELECT COUNT(1) " + baseSql;
            var itemsSql = $@"
                SELECT f.SysUserCustomFieldId, f.FieldName, f.FieldDataType
                {baseSql}
                ORDER BY SysUserCustomFieldId
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<SysUserCustomField>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysUserCustomField>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }
        public async Task<IEnumerable<SysUserCustomFieldValidations>> GetValidationsByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    v.SysUserCustomFieldValidationId,
                    v.SysUserCustomFieldId,
                    v.RuleName,
                    v.RuleValue,
                    v.message,
                    v.IsActive,
                    v.CreatedBy,
                    v.CreatedAt,
                    v.UpdatedBy,
                    v.UpdatedAt,
                    uC.UserId AS CreatedById,
                    CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                    uU.UserId AS UpdatedById,
                    CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
                FROM SysUserCustomFieldValidations v
                INNER JOIN Users uC ON uC.UserId = v.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = v.UpdatedBy
                WHERE v.SysUserCustomFieldId = @Id
                ORDER BY v.SysUserCustomFieldValidationId;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SysUserCustomFieldValidations>(new CommandDefinition(sql, new { Id = fieldId }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<SysUserCustomFieldOptions>> GetOptionsByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    o.SysUserCustomFieldOptionsId,
                    o.SysUserCustomFieldId,
                    o.label,
                    o.value,
                    o.IsActive,
                    o.CreatedBy,
                    o.CreatedAt,
                    o.UpdatedBy,
                    o.UpdatedAt,
                    uC.UserId AS CreatedById,
                    CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                    uU.UserId AS UpdatedById,
                    CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
                FROM SysUserCustomFieldOptions o
                INNER JOIN Users uC ON uC.UserId = o.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = o.UpdatedBy
                WHERE o.SysUserCustomFieldId = @Id
                ORDER BY o.SysUserCustomFieldOptionsId;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SysUserCustomFieldOptions>(new CommandDefinition(sql, new { Id = fieldId }, cancellationToken: cancellationToken));
        }
    }
}
