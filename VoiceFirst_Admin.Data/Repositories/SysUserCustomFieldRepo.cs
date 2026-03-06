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

        public async Task<BulkUpsertError?> CreateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> validations, List<SysUserCustomFieldOptions> options, int createdBy, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                const string sql = @"INSERT INTO SysUserCustomField (CountryName, FieldKey, FieldDataType, CreatedBy)
                                 VALUES (@CountryName, @FieldKey, @FieldDataType, @CreatedBy);
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
                    var validationError = await InsertSysUserCustomFieldValidationsAsync(connection, tx, entity.SysUserCustomFieldId, validations, createdBy, cancellationToken);
                    if (validationError != null)
                    {
                        return validationError;
                    }
                }

                if (options != null && options.Any())
                {
                    var optionError = await InsertSysUserCustomFieldOptionsAsync(connection, tx, entity.SysUserCustomFieldId, options, createdBy, cancellationToken);
                    if (optionError != null)
                    {
                        return optionError;
                    }
                }

                tx.Commit();
                return null;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public async Task<SysUserCustomField> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sqlField = @"SELECT SysUserCustomFieldId, CountryName, FieldKey, FieldDataType, IsActive, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                       FROM SysUserCustomField WHERE SysUserCustomFieldId = @Id;";
            using var conn = _context.CreateConnection();
            var field = await conn.QueryFirstOrDefaultAsync<SysUserCustomField>(new CommandDefinition(sqlField, new { Id = id }, cancellationToken: cancellationToken));
            return field;
        }

        public async Task<BulkUpsertError?> UpdateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> addValidations, List<SysUserCustomFieldOptions> addOptions, IEnumerable<SysUserCustomFieldValidations> validations, IEnumerable<SysUserCustomFieldOptions> options, int updatedBy, CancellationToken cancellationToken = default)
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
                        sql.Append(" WHERESysUserCustomFieldId = @SysUserCustomFieldId AND IsDeleted = 0;");
                        var cmd = new CommandDefinition(sql.ToString(), parameters, transaction: tx, cancellationToken: cancellationToken);
                        var affected = await connection.ExecuteAsync(cmd);
                        if (affected <= 0)
                        {
                            return new BulkUpsertError
                            {
                                StatuaCode = StatusCodes.Status500InternalServerError,
                                Message = Messages.SomethingWentWrong
                            };
                        }
                    }

                }



                if (addValidations != null && addValidations.Any())
                {
                    var validationError = await InsertSysUserCustomFieldValidationsAsync(connection, tx, entity.SysUserCustomFieldId, addValidations, updatedBy, cancellationToken);
                    if (validationError != null)
                    {
                        return validationError;
                    }
                }

                if (addOptions != null && addOptions.Any())
                {
                    var optionError = await InsertSysUserCustomFieldOptionsAsync(connection, tx, entity.SysUserCustomFieldId, addOptions, updatedBy, cancellationToken);
                    if (optionError != null)
                    {
                        return optionError;
                    }
                }
                if (validations != null && validations.Any())
                {
                    var validationError = await UpdateSysUserCustomFieldValidationsAsync(connection, tx, validations, updatedBy, cancellationToken);
                    if (validationError != null)
                    {
                        return validationError;
                    }

                }


                if (options != null && options.Any())
                {
                    var optionError = await UpdateSysUserCustomFieldOptionsAsync(connection, tx, options, updatedBy, cancellationToken);
                    if (optionError != null)
                    {
                        return optionError;
                    }
                }

                tx.Commit();
                return null;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
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

                const string delV = "UPDATE SysUserCustomFieldValidations SET IsDeleted = 1 WHERE SysUserCustomFieldId = @Id;";
                const string delO = "UPDATE SysUserCustomFieldOptions SET IsDeleted = 1 WHERE SysUserCustomFieldId = @Id;";
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

        private async Task<BulkUpsertError?> InsertSysUserCustomFieldValidationsAsync(
                    IDbConnection connection,
                    IDbTransaction tx,
                    int sysUserCustomFieldId,
                    IEnumerable<dynamic>? addValidations,
                    int createdBy,
                    CancellationToken cancellationToken)
        {
            if (addValidations == null || !addValidations.Any())
                return null;

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

            var parameters = addValidations
                .Where(v =>
                    !string.IsNullOrWhiteSpace(v.RuleName) ||
                    v.RuleValue != null ||
                    !string.IsNullOrWhiteSpace(v.message))
                .Select(v => new
                {
                    SysUserCustomFieldId = sysUserCustomFieldId,
                    v.RuleName,
                    v.RuleValue,
                    v.message,
                    CreatedBy = createdBy
                })
                .ToList();

            if (!parameters.Any())
                return null;

            var cmd = new CommandDefinition(
                sql,
                parameters,
                transaction: tx,
                cancellationToken: cancellationToken);

            var affected = await connection.ExecuteAsync(cmd);

            if (affected <= 0)
            {
                return new BulkUpsertError
                {
                    StatuaCode = StatusCodes.Status500InternalServerError,
                    Message = Messages.SomethingWentWrong
                };
            }

            return null;
        }
        private async Task<BulkUpsertError?> InsertSysUserCustomFieldOptionsAsync(
                IDbConnection connection,
                IDbTransaction tx,
                int sysUserCustomFieldId,
                IEnumerable<dynamic>? addOptions,
                int createdBy,
                CancellationToken cancellationToken)
        {
            if (addOptions == null || !addOptions.Any())
                return null;

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

            var parameters = addOptions
                .Where(o => !string.IsNullOrWhiteSpace(o.label) || o.value != null)
                .Select(o => new
                {
                    SysUserCustomFieldId = sysUserCustomFieldId,
                    o.label,
                    o.value,
                    CreatedBy = createdBy
                })
                .ToList();

            if (!parameters.Any())
                return null;

            var cmd = new CommandDefinition(
                sql,
                parameters,
                transaction: tx,
                cancellationToken: cancellationToken);

            var affected = await connection.ExecuteAsync(cmd);

            if (affected <= 0)
            {
                return new BulkUpsertError
                {
                    StatuaCode = StatusCodes.Status500InternalServerError,
                    Message = Messages.SomethingWentWrong
                };
            }

            return null;
        }
        private async Task<BulkUpsertError?> UpdateSysUserCustomFieldOptionsAsync(
            IDbConnection connection,
            IDbTransaction tx,
            IEnumerable<dynamic>? options,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (options == null || !options.Any())
                return null;

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

                        if (affected <= 0)
                        {
                            return new BulkUpsertError
                            {
                                StatuaCode = StatusCodes.Status500InternalServerError,
                                Message = Messages.SomethingWentWrong
                            };
                        }
                    }
                }
            }

            return null;
        }
        private async Task<BulkUpsertError?> UpdateSysUserCustomFieldValidationsAsync(
            IDbConnection connection,
            IDbTransaction tx,
            IEnumerable<dynamic>? validations,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            if (validations == null || !validations.Any())
                return null;

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

                        var affected = await connection.ExecuteAsync(cmd);

                        if (affected <= 0)
                        {
                            return new BulkUpsertError
                            {
                                StatuaCode = StatusCodes.Status500InternalServerError,
                                Message = Messages.SomethingWentWrong
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
