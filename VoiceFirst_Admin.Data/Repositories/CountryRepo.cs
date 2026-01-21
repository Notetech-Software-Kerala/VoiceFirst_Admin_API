using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class CountryRepo : ICountryRepo
{
    private readonly IDapperContext _context;

    public CountryRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<Country>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"FROM Country WHERE 1=1");

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }
        var searchByMap = new Dictionary<CountrySearchBy, string>
        {
            [CountrySearchBy.CountryName] = "CountryName",
            [CountrySearchBy.DivisionOne] = "DivisionOneName",
            [CountrySearchBy.DivisionTwo] = "DivisionTwoName",
            [CountrySearchBy.DivisionThree] = "DivisionThreeName",
            [CountrySearchBy.DialCode] = "CountryDialCode",
            [CountrySearchBy.IsoAlphaTwo] = "CountryIsoAlphaTwo",

            // ZipCode cannot be a single column on po; handle separately (see below)
        };
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            // If SearchBy = ZipCode, use EXISTS on PostOfficeZipCode
            if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
            {
                baseSql.Append($" AND {col} LIKE @Search");
            }
            else
            {
                // Default: search across everything (name + users + zipcode)
                baseSql.Append(@"
            AND (
                  CountryName LIKE @Search
                OR DivisionOneName LIKE @Search 
                OR DivisionTwoName LIKE @Search
                OR DivisionThreeName LIKE @Search
                OR CountryDialCode  LIKE @Search 
                OR CountryIsoAlphaTwo   LIKE @Search 
                
            )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }
        

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortColumn = string.IsNullOrWhiteSpace(filter.SortBy) ? "CountryName" : filter.SortBy;

        var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        var itemsSql = $@"SELECT CountryId, CountryName, DivisionOneName, DivisionTwoName, DivisionThreeName, CountryDialCode, CountryIsoAlphaTwo, IsActive, IsDeleted
                          {baseSql}
                          ORDER BY {sortColumn} {sortOrder}
                          OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<Country>(itemsSql, parameters);

        return new PagedResultDto<Country>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }

    public async Task<IEnumerable<Country>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT CountryId, CountryName
                             FROM Country
                             WHERE IsActive = 1 AND IsDeleted = 0
                             ORDER BY CountryName ASC;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<Country>(sql);
        return items;
    }
}

