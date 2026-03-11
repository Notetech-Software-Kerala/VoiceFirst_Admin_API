using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services;

public class PlatformVersionService : IApplicationVersionService
{
    private readonly IApplicationRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public PlatformVersionService(
        IApplicationRepository repo,
        IMapper mapper, 
        IConfiguration configuration
        )
    {
        _repo = repo;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ApiResponse<PlatformVersionDto>> CreateAsync(
        PlatformVersionCreateDto dto,
        int loginId,
        CancellationToken cancellationToken = default)
    {
        dto.Version = dto.Version.Trim();
        dto.ClientType = dto.ClientType;
        var appId = int.Parse(_configuration["ApplicationSettings:DefaultApplicationId"]);
        var application = await _repo.IsIdExistAsync
            (appId, cancellationToken);

        if (application == null )
            return ApiResponse<PlatformVersionDto>.Fail(
                Messages.ApplicationNotFoundById,
                StatusCodes.Status404NotFound,
                ErrorCodes.PlatformNotFound);
        
        var existing = await _repo.VersionExistsAsync
            (appId, dto.Version, dto.ClientType, cancellationToken);

        if (existing != null)
        {
            return ApiResponse<PlatformVersionDto>.Fail(
                Messages.ApplicationVersionAlreadyExists,
                StatusCodes.Status409Conflict,
                ErrorCodes.ApplicationVersionAlreadyExists);
        }

        var entity = _mapper.Map<ApplicationVersion>(dto);
        entity.CreatedBy = loginId;

        entity.ApplicationVersionId = await _repo.CreateVersionAsync
            (entity, cancellationToken);

        if (entity.ApplicationVersionId <= 0)
            return ApiResponse<PlatformVersionDto>.Fail(
                Messages.SomethingWentWrong,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalServerError);

        var created = await _repo.GetVersionByIdAsync
            (entity.ApplicationVersionId, cancellationToken);

        return ApiResponse<PlatformVersionDto>.Ok(
            created!,
            Messages.ApplicationVersionCreated,
            StatusCodes.Status201Created);
    }
}
