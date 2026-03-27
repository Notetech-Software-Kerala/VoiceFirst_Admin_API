using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<int> CreateAsync(SysUserCustomField entity, List<CustomFieldDataTypeDto>? customFieldDataTypes, int createdBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                const string sql = @"INSERT INTO SysUserCustomField (FieldName, FieldKey, CreatedBy)
                                 VALUES (@FieldName, @FieldKey, @CreatedBy);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
                {
                    entity.FieldName,
                    entity.FieldKey,
                    CreatedBy = createdBy
                }, transaction: tx, cancellationToken: cancellationToken));

                var rows = 0;
                if(customFieldDataTypes.Count()>0 && customFieldDataTypes != null)
                {
                    rows=await InsertSysUserCustomFieldDataTypesAsync(
                        connection,
                        tx,
                        id,
                        customFieldDataTypes,
                        createdBy,
                        cancellationToken);
                    
                    
                }
                if(rows== customFieldDataTypes.Count())
                {
                    tx.Commit();
                    return id;
                }
                else
                {
                    try { tx.Rollback(); } catch { }
                    return 0;
                }
                
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
            f.IsActive,
            f.IsDeleted,
            f.CreatedBy,
            f.CreatedAt,
            f.UpdatedBy,
            f.UpdatedAt,
            f.DeletedBy,
            f.DeletedAt,
            CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
            CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
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
        public async Task<SysUserCustomFieldDataTypeLink> GetByLinkIdAsync(int id, CancellationToken cancellationToken = default)
        {
             const string sqlField = @"
        SELECT 
            l.SysUserCustomFieldDataTypeLinkId,
            l.SysUserCustomFieldId,
            l.SysUserCustomFieldDataTypeId,
            l.ValueDataType,
            dT.FieldDataType,
            f.FieldName,
            f.IsDeleted,
            f.CreatedBy,
            f.CreatedAt,
            f.UpdatedBy,
            f.UpdatedAt,
            f.DeletedBy,
            f.DeletedAt,
            CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
            CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
            CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
        FROM SysUserCustomFieldDataTypeLink l
        INNER JOIN SysUserCustomField f ON f.SysUserCustomFieldId = l.SysUserCustomFieldId
        INNER JOIN SysUserCustomFieldDataType dT ON dT.SysUserCustomFieldDataTypeId = l.SysUserCustomFieldDataTypeId
        INNER JOIN Users uC ON uC.UserId = f.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = f.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = f.DeletedBy
        WHERE f.SysUserCustomFieldId = @Id;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomFieldDataTypeLink>(new CommandDefinition(sqlField, new { Id = id }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomField> ExistsByFieldKeyAsync(string fieldKey, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT SysUserCustomFieldId, FieldName, FieldKey, IsActive, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                       FROM SysUserCustomField WHERE FieldKey = @FieldKey;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { FieldKey = fieldKey }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomField> ExistsByFieldNameAsync(string fieldName,  CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT SysUserCustomFieldId, FieldName, FieldKey, IsActive, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                       FROM SysUserCustomField WHERE FieldName = @fieldName ";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { FieldName = fieldName }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomFieldDataTypeLink> ExistsByFieldIdAndDataTypeIdIdAsync(int customerFieldId, int fieldDataTypeId, CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT *
                                       FROM SysUserCustomFieldDataTypeLink WHERE SysUserCustomFieldId = @SysUserCustomFieldId,SysUserCustomFieldDataTypeId=@FieldDataTypeId ";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomFieldDataTypeLink>(new CommandDefinition(sqlField, new { SysUserCustomFieldId = customerFieldId, FieldDataTypeId = fieldDataTypeId }, cancellationToken: cancellationToken));
            return field;
        }
        public async Task<SysUserCustomFieldDataType> ExistsByFieldDataTypeByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysUserCustomFieldDataType WHERE SysUserCustomFieldDataTypeId = @Id;";
            using var conn = _context.CreateConnection();
            var dataType = await conn.QueryFirstOrDefaultAsync<SysUserCustomFieldDataType>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
            return dataType;
        }
        public async Task<IEnumerable< SysUserCustomFieldDataType>> FieldDataTypeLookupAsync( CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysUserCustomFieldDataType ";
            using var conn = _context.CreateConnection();
            var dataType = await conn.QueryAsync<SysUserCustomFieldDataType>(new CommandDefinition(sql, cancellationToken: cancellationToken));
            return dataType;
        }
        public async Task<SysUserCustomFieldValidationsRule> ExistsByValidationRuleByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysUserCustomFieldValidationsRule WHERE SysUserCustomFieldValidationRuleId = @Id;";
            using var conn = _context.CreateConnection();
            var rule = await conn.QueryFirstOrDefaultAsync<SysUserCustomFieldValidationsRule>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
            return rule;
        }
        public async Task<PagedResultDto<SysUserCustomFieldValidationsRule>> ValidationRuleLookupAsync(BasicFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
                FROM SysUserCustomFieldValidationsRule 
                WHERE 1=1");



           

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                baseSql.Append(@" AND RuleName LIKE @Search ");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }






            var countSql = "SELECT COUNT(1) " + baseSql;
            var itemsSql = $@"
                SELECT * 
                {baseSql}
                ORDER BY SysUserCustomFieldValidationRuleId
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<SysUserCustomFieldValidationsRule>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysUserCustomFieldValidationsRule>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
         
        }
        public async Task<bool> UpdateAsync(SysUserCustomField entity, List<UpdateCustomFieldDataTypeDto>? UpdateCustomFieldDataTypes, List<CustomFieldDataTypeDto>? addCustomFieldDataTypes, int updatedBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                if(!string.IsNullOrWhiteSpace(entity.FieldName) || !string.IsNullOrWhiteSpace(entity.FieldKey)||  entity.IsActive.HasValue)
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
                    
                    if (entity.IsActive.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", entity.IsActive);
                    }
                    if (sets.Count != 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");
                        parameters.Add("UpdatedBy", updatedBy);
                        parameters.Add("SysUserCustomFieldId", entity.SysUserCustomFieldId);
                        var sql = new StringBuilder();
                        sql.Append("UPDATE SysUserCustomField SET ");
                        sql.Append(string.Join(", ", sets));
                        sql.Append(" WHERE SysUserCustomFieldId = @SysUserCustomFieldId AND IsDeleted = 0;");
                        var cmd = new CommandDefinition(sql.ToString(), parameters, transaction: tx, cancellationToken: cancellationToken);
                         await connection.ExecuteAsync(cmd);
                        
                    }

                }

                if (addCustomFieldDataTypes != null && addCustomFieldDataTypes.Count() > 0 )
                {
                    await InsertSysUserCustomFieldDataTypesAsync(
                        connection,
                        tx,
                        entity.SysUserCustomFieldId,
                        addCustomFieldDataTypes,
                        updatedBy,
                        cancellationToken);


                }
                
                if (UpdateCustomFieldDataTypes != null && UpdateCustomFieldDataTypes.Count() > 0)
                {

                    await UpdateSysUserCustomFieldDataTypesAsync(
                        connection,
                        tx,
                        entity.SysUserCustomFieldId,
                        UpdateCustomFieldDataTypes,
                        updatedBy,
                        cancellationToken
                    );
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

                const string delV = "UPDATE SysUserCustomFieldDataTypeLink SET IsActive = 0 WHERE SysUserCustomFieldId = @Id;";
                
                await connection.ExecuteAsync(new CommandDefinition(delV, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));
                //await connection.ExecuteAsync(new CommandDefinition(delO, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));

                tx.Commit();
                return rows > 0;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }
        public async Task<bool> RestoreAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                const string sql = @"UPDATE SysUserCustomField
                                 SET IsDeleted = 0, DeletedBy = null, DeletedAt = null
                                 WHERE SysUserCustomFieldId = @Id;";

                var rows = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, DeletedBy = deletedBy }, transaction: tx, cancellationToken: cancellationToken));

                const string delV = "UPDATE SysUserCustomFieldDataTypeLink SET IsActive = 0 WHERE SysUserCustomFieldId = @Id;";
                
                await connection.ExecuteAsync(new CommandDefinition(delV, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));
                //await connection.ExecuteAsync(new CommandDefinition(delO, new { Id = id }, transaction: tx, cancellationToken: cancellationToken));

                tx.Commit();
                return rows > 0;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }
        private async Task<int> InsertSysUserCustomFieldDataTypesAsync(
    IDbConnection connection,
    IDbTransaction tx,
    int sysUserCustomFieldId,
    List<CustomFieldDataTypeDto>? customFieldDataTypes,
    int createdBy,
    CancellationToken cancellationToken)
        {
            if (customFieldDataTypes == null || !customFieldDataTypes.Any())
                return 0;

            const string sqlDataType = @"
        INSERT INTO SysUserCustomFieldDataTypeLink
        (
            SysUserCustomFieldId,
            SysUserCustomFieldDataTypeId,
            ValueDataType,
            CreatedBy
        )
        VALUES
        (
            @SysUserCustomFieldId,
            @SysUserCustomFieldDataTypeId,
            @ValueDataType,
            @CreatedBy
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int rows = 0;

            foreach (var cfd in customFieldDataTypes)
            {
                var linkId = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        sqlDataType,
                        new
                        {
                            SysUserCustomFieldId = sysUserCustomFieldId,
                            SysUserCustomFieldDataTypeId = cfd.FieldDataTypeId,
                            ValueDataType = cfd.ValueDataType,
                            CreatedBy = createdBy
                        },
                        transaction: tx,
                        cancellationToken: cancellationToken
                    )
                );

                if (linkId > 0)
                    rows++;

                if (cfd.AddValidations != null && cfd.AddValidations.Any())
                {
                    await InsertSysUserCustomFieldValidationsAsync(
                        connection,
                        tx,
                        linkId,
                        cfd.AddValidations,
                        createdBy,
                        cancellationToken);
                }

                if (cfd.AddOptions != null && cfd.AddOptions.Any())
                {
                    await InsertSysUserCustomFieldOptionsAsync(
                        connection,
                        tx,
                        linkId,
                        cfd.AddOptions,
                        createdBy,
                        cancellationToken);
                }
            }

            return rows;
        }

        private async Task UpdateSysUserCustomFieldDataTypesAsync(
    IDbConnection connection,
    IDbTransaction tx,
    int sysUserCustomFieldId,
    IEnumerable<UpdateCustomFieldDataTypeDto>? updateCustomFieldDataTypes,
    int updatedBy,
    CancellationToken cancellationToken)
        {
            if (updateCustomFieldDataTypes?.Any() != true)
                return;

            foreach (var updateDataType in updateCustomFieldDataTypes)
            {
                var sets = new List<string>();
                var parameters = new DynamicParameters();

                //if (updateDataType.FieldDataTypeId > 0)
                //{
                //    sets.Add("SysUserCustomFieldDataTypeId = @FieldDataTypeId");
                //    parameters.Add("FieldDataTypeId", updateDataType.FieldDataTypeId);
                //}

                if (updateDataType.ValueDataType != null)
                {
                    sets.Add("ValueDataType = @ValueDataType");
                    parameters.Add("ValueDataType", updateDataType.ValueDataType);
                }

                if (updateDataType.Active.HasValue)
                {
                    sets.Add("IsActive = @IsActive");
                    parameters.Add("IsActive", updateDataType.Active.Value);
                }

                if (sets.Count > 0)
                {
                    sets.Add("UpdatedBy = @UpdatedBy");
                    sets.Add("UpdatedAt = SYSDATETIME()");

                    parameters.Add("UpdatedBy", updatedBy);
                    parameters.Add("CustomFieldLinkId", updateDataType.CustomFieldLinkId);

                    var sql = new StringBuilder();
                    sql.Append("UPDATE SysUserCustomFieldDataTypeLink SET ");
                    sql.Append(string.Join(", ", sets));
                    sql.Append(" WHERE SysUserCustomFieldDataTypeLinkId = @CustomFieldLinkId;");

                    var cmd = new CommandDefinition(
                        sql.ToString(),
                        parameters,
                        transaction: tx,
                        cancellationToken: cancellationToken
                    );

                    await connection.ExecuteAsync(cmd);
                }

                if (updateDataType.AddValidations?.Any() == true)
                {
                    await InsertSysUserCustomFieldValidationsAsync(
                        connection,
                        tx,
                        sysUserCustomFieldId,
                        updateDataType.AddValidations,
                        updatedBy,
                        cancellationToken
                    );
                }

                if (updateDataType.AddOptions?.Any() == true)
                {
                    await InsertSysUserCustomFieldOptionsAsync(
                        connection,
                        tx,
                        sysUserCustomFieldId,
                        updateDataType.AddOptions,
                        updatedBy,
                        cancellationToken
                    );
                }

                if (updateDataType.UpdateValidations?.Any() == true)
                {
                    await UpdateSysUserCustomFieldValidationsAsync(
                        connection,
                        tx,
                        updateDataType.UpdateValidations,
                        updatedBy,
                        cancellationToken
                    );
                }

                if (updateDataType.UpdateOptions?.Any() == true)
                {
                    await UpdateSysUserCustomFieldOptionsAsync(
                        connection,
                        tx,
                        updateDataType.UpdateOptions,
                        updatedBy,
                        cancellationToken
                    );
                }
            }
        }
        private async Task InsertSysUserCustomFieldValidationsAsync(
                    IDbConnection connection,
                    IDbTransaction tx,
                    int SysUserCustomFieldDataTypeLinkId,
                    IEnumerable<CreateCustomFieldValidationsDto>? addValidations,
                    int createdBy,
                    CancellationToken cancellationToken)
        {
            if (addValidations == null || !addValidations.Any())
                return ;

            const string sql = @"
                INSERT INTO SysUserCustomFieldValidations
                (
                    SysUserCustomFieldDataTypeLinkId,
                    RuleId,
                    RuleValue,
                    Message,
                    CreatedBy
                )
                VALUES
                (
                    @SysUserCustomFieldDataTypeLinkId,
                    @RuleId,
                    @RuleValue,
                    @Message,
                    @CreatedBy
                );";

            int affectedRows = 0;

            foreach (var v in addValidations)
            {


                var parameter = new DynamicParameters();
                parameter.Add("SysUserCustomFieldDataTypeLinkId", SysUserCustomFieldDataTypeLinkId);
                parameter.Add("RuleId", v.RuleId);
                parameter.Add("RuleValue", v.RuleValue);
                parameter.Add("Message", v.Message);
                parameter.Add("CreatedBy", createdBy);

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
                int SysUserCustomFieldDataTypeLinkId,
                IEnumerable<CreateCustomFieldOptionsDto>? addOptions,
                int createdBy,
                CancellationToken cancellationToken)
        {
            

            const string sql = @"
            INSERT INTO SysUserCustomFieldOptions
            (
                SysUserCustomFieldDataTypeLinkId,
                label,
                value,
                CreatedBy
            )
            VALUES
            (
                @SysUserCustomFieldDataTypeLinkId,
                @label,
                @value,
                @CreatedBy
            );";

        

            foreach (var o in addOptions)
            {
                
                var parameter = new
                {
                    SysUserCustomFieldDataTypeLinkId = SysUserCustomFieldDataTypeLinkId,
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
            IEnumerable<UpdateCustomFieldOptionsDto>? options,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (options == null || !options.Any())
                return ;

            foreach (var option in options)
            {
                if (!string.IsNullOrWhiteSpace(option.label) ||
                    option.value != null ||
                    option.Active.HasValue)
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

                    if (option.Active.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", option.Active);
                    }

                    if (sets.Count > 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");

                        parameters.Add("UpdatedBy", updatedBy);
                        parameters.Add("SysUserCustomFieldOptionsId", option.CustomFieldOptionsId);

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
            IEnumerable<UpdateCustomFieldValidationsDto>? validations,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (validations == null || !validations.Any())
                return ;

            foreach (var validation in validations)
            {
                if (
                    validation.RuleValue != null ||
                    !string.IsNullOrWhiteSpace(validation.message) ||
                    validation.Active.HasValue)
                {
                    var sets = new List<string>();
                    var parameters = new DynamicParameters();

                    

                    if (validation.RuleValue != null)
                    {
                        sets.Add("RuleValue = @RuleValue");
                        parameters.Add("RuleValue", validation.RuleValue);
                    }

                    if (!string.IsNullOrWhiteSpace(validation.message))
                    {
                        sets.Add("Message = @message");
                        parameters.Add("message", validation.message);
                    }

                    if (validation.Active.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", validation.Active);
                    }

                    if (sets.Count > 0)
                    {
                        sets.Add("UpdatedBy = @UpdatedBy");
                        sets.Add("UpdatedAt = SYSDATETIME()");

                        parameters.Add("UpdatedBy", updatedBy);
                        parameters.Add("SysUserCustomFieldValidationId", validation.CustomFieldValidationId);

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
LEFT JOIN SysUserCustomFieldDataTypeLink dLink ON dLink.SysUserCustomFieldId = f.SysUserCustomFieldId
                LEFT JOIN SysUserCustomFieldDataType dT ON dT.SysUserCustomFieldDataTypeId = dLink.SysUserCustomFieldDataTypeId
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
                [SysUserCustomFieldSearchBy.FieldDataType] = "dT.FieldDataType",
                [SysUserCustomFieldSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [SysUserCustomFieldSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [SysUserCustomFieldSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@" AND (f.FieldName LIKE @Search OR f.FieldKey LIKE @Search OR dT.FieldDataType LIKE @Search
                        OR CONCAT(uC.FirstName,' ',uC.LastName) LIKE @Search OR CONCAT(uU.FirstName,' ',uU.LastName) LIKE @Search)");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["SysUserCustomFieldId"] = "f.SysUserCustomFieldId",
                ["FieldName"] = "f.FieldName",
                ["FieldKey"] = "f.FieldKey",
                ["FieldDataType"] = "dT.FieldDataType",
                ["Active"] = "f.IsActive",
                ["Deleted"] = "f.IsDeleted",
                ["CreatedDate"] = "f.CreatedAt",
                ["ModifiedDate"] = "f.UpdatedAt",
            };

            var sortOrder = filter.SortOrder == VoiceFirst_Admin.Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "SysUserCustomFieldId" : filter.SortBy;
            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["SysUserCustomFieldId"];

            var countSql = "SELECT COUNT(DISTINCT f.SysUserCustomFieldId) " + baseSql;
            var itemsSql = $@" 
                SELECT DISTINCT f.SysUserCustomFieldId, f.FieldName, f.FieldKey, f.IsActive, f.IsDeleted, f.CreatedAt, f.UpdatedAt,
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
                LEFT JOIN SysUserCustomFieldDataTypeLink dLink ON dLink.SysUserCustomFieldId = f.SysUserCustomFieldId
                LEFT JOIN SysUserCustomFieldDataType dT ON dT.SysUserCustomFieldDataTypeId = dLink.SysUserCustomFieldDataTypeId
                WHERE f.IsDeleted=0 And f.IsActive=1");

            

            var searchByMap = new Dictionary<SysUserCustomFieldSearchBy, string>
            {
                [SysUserCustomFieldSearchBy.FieldName] = "f.FieldName",
                [SysUserCustomFieldSearchBy.FieldKey] = "f.FieldKey",
                [SysUserCustomFieldSearchBy.FieldDataType] = "dT.FieldDataType",
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                baseSql.Append(@" AND (f.FieldName LIKE @Search OR f.FieldKey LIKE @Search OR dT.FieldDataType LIKE @Search");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            

            
            

            var countSql = "SELECT COUNT(1) " + baseSql;
            var itemsSql = $@"
                SELECT  f.FieldName, dT.FieldDataType,dLink.SysUserCustomFieldDataTypeLinkId
                {baseSql}
                ORDER BY f.SysUserCustomFieldId
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
        public async Task<IEnumerable<SysUserCustomFieldValidations>> GetValidationsByFieldLinkIdAsync(int fieldLinkId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    v.SysUserCustomFieldValidationId,
                    v.SysUserCustomFieldDataTypeLinkId,
                    v.RuleId,
                    vR.RuleName,
                    v.RuleValue,
                    v.message,
                    v.IsActive,
                    v.CreatedBy,
                    v.CreatedAt,
                    v.UpdatedBy,
                    v.UpdatedAt,
                    CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                    CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
                FROM SysUserCustomFieldValidations v
                INNER JOIN Users uC ON uC.UserId = v.CreatedBy
                INNER JOIN SysUserCustomFieldValidationsRule vR ON vR.SysUserCustomFieldValidationRuleId = v.RuleId
                LEFT JOIN Users uU ON uU.UserId = v.UpdatedBy
                WHERE v.SysUserCustomFieldDataTypeLinkId = @Id
                ORDER BY v.SysUserCustomFieldValidationId;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SysUserCustomFieldValidations>(new CommandDefinition(sql, new { Id = fieldLinkId }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<SysUserCustomFieldOptions>> GetOptionsByFieldLinkIdAsync(int fieldLinkId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    o.SysUserCustomFieldOptionsId,
                    o.SysUserCustomFieldDataTypeLinkId,
                    o.label,
                    o.value,
                    o.IsActive,
                    o.CreatedBy,
                    o.CreatedAt,
                    o.UpdatedBy,
                    o.UpdatedAt,
                    CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                    CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
                FROM SysUserCustomFieldOptions o
                INNER JOIN Users uC ON uC.UserId = o.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = o.UpdatedBy
                WHERE o.SysUserCustomFieldDataTypeLinkId = @Id
                ORDER BY o.SysUserCustomFieldOptionsId;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SysUserCustomFieldOptions>(new CommandDefinition(sql, new { Id = fieldLinkId }, cancellationToken: cancellationToken));
        }
        public async Task<IEnumerable<SysUserCustomFieldDataTypeLink>> GetFieldDataTypeByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    l.SysUserCustomFieldId,
                    l.SysUserCustomFieldDataTypeLinkId,
                    l.SysUserCustomFieldDataTypeId,
                    dT.FieldDataType,
                    l.ValueDataType,
                    l.IsActive,
                    l.CreatedBy,
                    l.CreatedAt,
                    l.UpdatedBy,
                    l.UpdatedAt,
                    CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                    CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
                FROM SysUserCustomFieldDataTypeLink l
                INNER JOIN Users uC ON uC.UserId = l.CreatedBy
                INNER JOIN SysUserCustomFieldDataType dT ON dT.SysUserCustomFieldDataTypeId = l.SysUserCustomFieldDataTypeId
                LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
                WHERE l.SysUserCustomFieldId = @Id
                ORDER BY l.SysUserCustomFieldDataTypeLinkId;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SysUserCustomFieldDataTypeLink>(new CommandDefinition(sql, new { Id = fieldId }, cancellationToken: cancellationToken));
        }
    }
}
