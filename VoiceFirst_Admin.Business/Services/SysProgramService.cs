using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysProgramService: ISysProgramService
    {
        private readonly ISysProgramRepo _repo;
        private readonly IApplicationRepo _applicationRepo;
        private readonly IProgramActionRepo _programActionRepo;
        private readonly IMapper _mapper;

        public SysProgramService(
            ISysProgramRepo repo, 
            IApplicationRepo applicationRepo, 
            IProgramActionRepo programActionRepo,
            IMapper mapper)
        {
            _repo = repo;
            _applicationRepo = applicationRepo;
            _programActionRepo = programActionRepo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SysProgramDto>>
            CreateAsync(SysProgramCreateDTO dto, 
            int loginId, 
            CancellationToken cancellationToken = default)
        {

            // Platform check (Application)
            var app = await _applicationRepo.
                GetActiveByIdAsync(dto.PlatformId, cancellationToken);

            if (app == null)
                return ApiResponse<SysProgramDto>.Fail(
                    "Platform is not found.",
                    StatusCodes.Status404NotFound
                    );


            // Permission checks (multiple)
            if (dto.PermissionIds != null && dto.PermissionIds.Count > 0)
            {
                foreach (var pid in dto.PermissionIds)
                {
                    var perm = await _programActionRepo.GetActiveByIdAsync(pid, cancellationToken);
                    if (perm == null)
                        return ApiResponse<SysProgramDto>.
                            Fail("Action is  not found.", 
                            StatusCodes.Status404NotFound);
                }
            }



            // Uniqueness checks scoped to Application
            var existingByName =
                await _repo.ExistsByNameAsync
                (dto.PlatformId, dto.ProgramName, null, cancellationToken);

            if (existingByName != null)
            {
                if (existingByName.IsDeleted == true)
                    return ApiResponse<SysProgramDto>.Fail(Messages.ProgramNameAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity);
                return ApiResponse<SysProgramDto>.Fail(Messages.ProgramNameAlreadyExists, StatusCodes.Status409Conflict);
            }

            var existingByLabel = await _repo.ExistsByLabelAsync
                (dto.PlatformId, dto.Label, null, cancellationToken);

            if (existingByLabel != null)
            {
                if (existingByLabel.IsDeleted == true)
                    return ApiResponse<SysProgramDto>.Fail(Messages.ProgramLabelAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity);
                return ApiResponse<SysProgramDto>.Fail(Messages.ProgramLabelAlreadyExists, StatusCodes.Status409Conflict);
            }

            var existingByRoute = await _repo.
                ExistsByRouteAsync(dto.PlatformId, 
                dto.Route, null, cancellationToken);

            if (existingByRoute != null)
            {
                if (existingByRoute.IsDeleted == true)
                    return ApiResponse<SysProgramDto>.Fail(Messages.ProgramRouteAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity);
                return ApiResponse<SysProgramDto>.Fail(Messages.ProgramRouteAlreadyExists, StatusCodes.Status409Conflict);
            }

            var entity = _mapper.Map<SysProgram>(dto);
            entity.CreatedBy = loginId;

            var created = await _repo.CreateAsync
                (entity, dto.PermissionIds ?? new List<int>(), cancellationToken);

            var dtoOut = _mapper.Map<SysProgramDto>(created);
            // Load and map action links
            var links = await _repo.GetLinksByProgramIdAsync(created.SysProgramId, cancellationToken);
            dtoOut.Action = links.Select(l => new VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkDTO
            {
                ActionId = l.ActionId,
                ActionName = l.ActionName,
                Active = l.Active,
                CreatedUser = l.CreatedUser ,
                CreatedDate = l.CreatedDate,
                ModifiedUser = l.ModifiedUser,
                ModifiedDate = l.ModifiedDate
            }).ToList();

            return ApiResponse<SysProgramDto>.Ok(dtoOut, Messages.ProgramCreated, 
                Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
        }
    }
}
