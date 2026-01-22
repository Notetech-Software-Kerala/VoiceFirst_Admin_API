using Dapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
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
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["DivisionThree"] = "DivisionThreeName",
            ["CountryName"] = "CountryName",
            ["DivisionOne"] = "DivisionOneName",
            ["DivisionTwo"] = "DivisionTwoName",
            ["CountryDialCode"] = "CountryDialCode",
            ["CountryIsoAlphaTwo"] = "CountryIsoAlphaTwo",
            ["Active"] = "IsActive",
            ["Deleted"] = "IsDeleted"
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "CountryName" : filter.SortBy;
        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["CountryName"];

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
        const string sql = @"SELECT CountryId, CountryName, DivisionOneName, DivisionTwoName, DivisionThreeName, CountryDialCode, CountryIsoAlphaTwo, IsActive, IsDeleted
                             FROM Country
                             WHERE IsActive = 1 AND IsDeleted = 0
                             ORDER BY CountryName ASC;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<Country>(sql);
        return items;
    }

    //   DivisionOne

    public async Task<PagedResultDto<DivisionOne>> GetAllDivisionOneAsync(DivisionOneFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"FROM DivisionOne d INNER JOIN Country ON Country.CountryId = d.CountryId WHERE 1=1");

        if (filter.CountryId.HasValue)
        {
            baseSql.Append(" AND d.CountryId = @CountryId");
            parameters.Add("CountryId", filter.CountryId.Value);
        }

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND d.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND d.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        var searchByMap = new Dictionary<DivOneSearchBy, string>
        {
            [DivOneSearchBy.CountryName] = "Country.CountryName",
            [DivOneSearchBy.DivOneName] = "d.DivisionOneName",

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
                  d.DivisionOneName LIKE @Search
                OR Country.CountryName LIKE @Search 
                
            )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["DivOneName"] = "d.DivisionOneName",
            ["CountryName"] = "Country.CountryName",
            ["Active"] = "d.IsActive",
            ["Deleted"] = "d.IsDeleted"
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "DivOneName" : filter.SortBy;
        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["DivOneName"];
   

        var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        var itemsSql = $@"SELECT d.DivisionOneId, d.DivisionOneName,Country.CountryName, d.CountryId, d.IsActive, d.IsDeleted
                          {baseSql}
                          ORDER BY {sortColumn} {sortOrder}
                          OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<DivisionOne>(itemsSql, parameters);

        return new PagedResultDto<DivisionOne>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }

    public async Task<IEnumerable<DivisionOne>> GetDivisionOneActiveByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT po.DivisionOneId, po.DivisionOneName,Country.CountryName, po.CountryId, po.IsActive, po.IsDeleted
                             FROM DivisionOne po
                             INNER JOIN Country ON Country.CountryId = po.CountryId
                             WHERE po.CountryId = @CountryId AND po.IsActive = 1 AND po.IsDeleted = 0
                             ORDER BY po.DivisionOneName ASC;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<DivisionOne>(sql, new { CountryId = countryId });
        return items;
    }

    //  DivisionTwo
    public async Task<PagedResultDto<DivisionTwo>> GetAllDivisionTwoAsync(DivisionTwoFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"FROM DivisionTwo d INNER JOIN DivisionOne ON DivisionOne.DivisionOneId = d.DivisionOneId WHERE 1=1");

        if (filter.DivisionOneId.HasValue)
        {
            baseSql.Append(" AND d.DivisionOneId = @DivisionOneId");
            parameters.Add("DivisionOneId", filter.DivisionOneId.Value);
        }

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND d.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND d.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        var searchByMap = new Dictionary<DivTwoSearchBy, string>
        {
            [DivTwoSearchBy.DivTwoName] = "d.DivisionTwoName",
            [DivTwoSearchBy.DivOneName] = "DivisionOne.DivisionOneName",

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
                  d.DivisionTwoName LIKE @Search
                OR DivisionOne.DivisionOneName LIKE @Search 
                
            )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["DivTwoName"] = "d.DivisionTwoName",
            ["DivOneName"] = "DivisionOne.DivisionOneName",
            ["Active"] = "d.IsActive",
            ["Deleted"] = "d.IsDeleted"
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "DivTwoName" : filter.SortBy;
        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["DivTwoName"];

   

        var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        var itemsSql = $@"SELECT d.DivisionTwoId, d.DivisionTwoName,DivisionOne.DivisionOneName, d.DivisionOneId,  d.IsActive, d.IsDeleted
                          {baseSql}
                          ORDER BY {sortColumn} {sortOrder}
                          OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<DivisionTwo>(itemsSql, parameters);

        return new PagedResultDto<DivisionTwo>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }
    public async Task<IEnumerable<DivisionTwo>> GetDivisionTwoActiveByDivisionOneIdAsync(int divisionOneId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT po.DivisionTwoId, po.DivisionTwoName, po.DivisionOneId,DivisionOne.DivisionOneName,po.IsActive, po.IsDeleted
                             FROM DivisionTwo po
                             INNER JOIN DivisionOne ON DivisionOne.DivisionOneId = po.DivisionOneId
                             WHERE po.DivisionOneId = @DivisionOneId AND po.IsActive = 1 AND po.IsDeleted = 0
                             ORDER BY po.DivisionTwoName ASC;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<DivisionTwo>(sql, new { DivisionOneId = divisionOneId });
        return items;
    }

    // DivisionThree 
    public async Task<PagedResultDto<DivisionThree>> GetAllDivisionThreeAsync(DivisionThreeFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"FROM DivisionThree d INNER JOIN DivisionTwo ON DivisionTwo.DivisionTwoId = d.DivisionTwoId WHERE 1=1");

        if (filter.DivisionTwoId.HasValue)
        {
            baseSql.Append(" AND d.DivisionTwoId = @DivisionTwoId");
            parameters.Add("DivisionTwoId", filter.DivisionTwoId.Value);
        }

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND d.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND d.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        var searchByMap = new Dictionary<DivThreeSearchBy, string>
        {
            [DivThreeSearchBy.DivThreeName] = "d.DivisionThreeName",
            [DivThreeSearchBy.DivTwoName] = "DivisionTwo.DivisionTwoName",

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
                  d.DivisionThreeName LIKE @Search
                OR DivisionTwo.DivisionTwoName LIKE @Search 
                
            )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["DivThreeName"] = "d.DivisionThreeName",
            ["DivTwoName"] = "DivisionTwo.DivisionTwoName",
            ["Active"] = "d.IsActive",
            ["Deleted"] = "d.IsDeleted"
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "DivTwoName" : filter.SortBy;
        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["DivThreeName"];

        var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        var itemsSql = $@"SELECT d.DivisionThreeId, d.DivisionThreeName,DivisionTwo.DivisionTwoName, d.DivisionTwoId, d.IsActive, d.IsDeleted
                          {baseSql}
                          ORDER BY {sortColumn} {sortOrder}
                          OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<DivisionThree>(itemsSql, parameters);

        return new PagedResultDto<DivisionThree>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }

    public async Task<IEnumerable<DivisionThree>> GetDivisionThreeActiveByDivisionTwoIdAsync(int divisionTwoId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT po.DivisionThreeId, po.DivisionThreeName,DivisionTwo.DivisionTwoName, po.DivisionTwoId, po.IsActive, po.IsDeleted
                             FROM DivisionThree po
                             INNER JOIN DivisionTwo ON DivisionTwo.DivisionTwoId = po.DivisionTwoId
                             WHERE po.DivisionTwoId = @DivisionTwoId AND po.IsActive = 1 AND po.IsDeleted = 0
                             ORDER BY po.DivisionThreeName ASC;";

        using var connection = _context.CreateConnection();
        var items = await connection.QueryAsync<DivisionThree>(sql, new { DivisionTwoId = divisionTwoId });
        return items;
    }
}

