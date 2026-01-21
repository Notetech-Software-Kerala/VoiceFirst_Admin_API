using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysProgramService : ISysProgramService
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
                    Messages.PlatformNotFound,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlatFormNotFound
                    );


            // Permission checks (multiple)
            if (dto.ActionIds != null && dto.ActionIds.Count > 0)
            {
                foreach (var pid in dto.ActionIds)
                {
                    var perm = await _programActionRepo.GetActiveByIdAsync(pid, cancellationToken);
                    if (perm == null)

                        return ApiResponse<SysProgramDto>.Fail(
                       Messages.ProgramActionNotFound,
                       StatusCodes.Status404NotFound,
                       ErrorCodes.ProgramActionNotFound
                       );
                }
            }



            // Uniqueness checks scoped to Application
            var existingByName =
                await _repo.ExistsByNameAsync
                (dto.PlatformId, dto.ProgramName, null, cancellationToken);

            if (existingByName != null)
            {
                if (existingByName.IsDeleted == true)

                    return ApiResponse<SysProgramDto>.Fail
                        (Messages.ProgramNameAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.ProgramNameAlreadyExistsRecoverable
                        );

                return ApiResponse<SysProgramDto>.Fail
                    (Messages.ProgramNameAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramNameAlreadyExists);
            }

            var existingByLabel = await _repo.ExistsByLabelAsync
                (dto.PlatformId, dto.Label, null, cancellationToken);

            if (existingByLabel != null)
            {
                if (existingByLabel.IsDeleted == true)

                    return ApiResponse<SysProgramDto>.
                        Fail(Messages.ProgramLabelAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.ProgramLabelAlreadyExistsRecoverable);

                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramLabelAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramLabelAlreadyExists);
            }

            var existingByRoute = await _repo.
                ExistsByRouteAsync(dto.PlatformId,
                dto.Route, null, cancellationToken);

            if (existingByRoute != null)
            {
                if (existingByRoute.IsDeleted == true)
                    return ApiResponse<SysProgramDto>.Fail
                        (Messages.ProgramRouteAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.ProgramRouteAlreadyExistsRecoverable);

                return ApiResponse<SysProgramDto>.Fail
                    (Messages.ProgramRouteAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramRouteAlreadyExists);
            }

            var entity = _mapper.Map<SysProgram>(dto);
            entity.CreatedBy = loginId;

            var created = await _repo.CreateAsync
                (entity, dto.ActionIds ?? new List<int>(), cancellationToken);

            var dtoOut = await _repo.SysProgramGetByIdAsync(created.SysProgramId, cancellationToken);
            return ApiResponse<SysProgramDto>.Ok(dtoOut!, Messages.ProgramCreated,
                Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<SysProgramDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dto = await _repo.SysProgramGetByIdAsync(id, cancellationToken);
            if (dto == null)
            {
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramNotFound,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound);
            }

            return ApiResponse<SysProgramDto>.Ok(dto, Messages.ProgramRetrieved);
        }


        public async Task<ApiResponse<bool>> DeleteAsync(int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var deleted = await _repo.DeleteAsync(id,loginId, cancellationToken);

            if (!deleted)
            {
                return ApiResponse<bool>.Fail(
                   Messages.ProgramNotFound,
                   StatusCodes.Status404NotFound,
                   ErrorCodes.NotFound);
            }
            return ApiResponse<bool>.Ok(true, Messages.ProgramDeleted);
        }

        public async Task<ApiResponse<SysProgramDto>> RecoverProgramAsync(
        int id,
        int loginId,
        CancellationToken cancellationToken = default)
        {
            var rowAffected = await _repo.RecoverProgramAsync(id, loginId, cancellationToken);
            if (rowAffected <= 0)
            {
                return ApiResponse<SysProgramDto>.Fail(
                   Messages.ProgramNotFound,
                   StatusCodes.Status404NotFound,
                   ErrorCodes.NotFound);
            }
            var dto = await GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysProgramDto>.Ok(dto.Data
                , Messages.ProgramRecovered);

        }


    //    public async Task<List<SysBusinessActivityActiveDTO>> GetActiveAsync(
    //CancellationToken cancellationToken)
    //    {
    //        var entities = await _repo.GetActiveAsync(cancellationToken);

    //        return _mapper.Map<IEnumerable<SysBusinessActivityActiveDTO>>(entities).ToList();
    //    }

        public async Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<SysProgramDto>> GetAllAsync(
            VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetAllAsync(filter, cancellationToken);
            return new VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<SysProgramDto>
            {
                Items = entities.Items,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }

    }
}
