using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlanService:IPlanService
    {
        private readonly IPlanRepo _planRepository;
        private readonly IDapperContext _context;
        private readonly IProgramActionRepo _programActionRepo;
        private readonly VoiceFirst_Admin.Data.Contracts.IRepositories.IRoleRepo _roleRepository;

        public PlanService(IPlanRepo planRepository,IDapperContext _context, IProgramActionRepo programActionRepo, IRoleRepo roleRepository)
        {
            _planRepository = planRepository;
            this._context = _context;
            _programActionRepo = programActionRepo;
            _roleRepository = roleRepository;
        }



        public async Task<ApiResponse<PlanDetailDto>>
         GetByIdAsync(int id,
         CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var dto = await _planRepository.GetByIdAsync
                (id,
                connection,
                transaction,
                cancellationToken);

            if (dto == null)
            {

                return ApiResponse<PlanDetailDto>.Fail(
                   Messages.PlanNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound);
            }        
            return ApiResponse<PlanDetailDto>.Ok(
                dto,
                Messages.PlanRetrieved,
                StatusCodes.Status200OK);
        }




        public async Task<ApiResponse<IEnumerable<PlanActiveDto>>> 
            GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var items = await _planRepository.GetActiveAsync(cancellationToken);
            return ApiResponse<IEnumerable<PlanActiveDto>>.
                Ok(items, Messages.PlanRetrieved);
        }







        
        public async Task<ApiResponse<IEnumerable<ProgramPlanDetailDto>>> GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var list = await _planRepository.GetProgramDetailsByPlanIdAsync(planId, connection,transaction, cancellationToken);
            transaction.Commit();
            return ApiResponse<IEnumerable<ProgramPlanDetailDto>>.Ok(list, Messages.PlanRetrieved);
        }









        public async Task<ApiResponse<PlanDetailDto>> CreatePlanAsync(
        PlanCreateDto dto,
        int loginId,
        CancellationToken cancellationToken = default)
        {
            var existing = await _planRepository.GetByNameAsync
                (dto.PlanName, cancellationToken);

            if (existing != null )
            {
                if(existing.IsDeleted == false)
                {
                    return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanNameAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlanAlreadyExists);
                }
                return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanNameAlreadyExistsRecoverable,
                    StatusCodes.Status422UnprocessableEntity,
                    ErrorCodes.PlanAlreadyExists,
                    new PlanDetailDto
                    {
                        PlanId = existing.PlanId
                    });
            }

            if (dto.ProgramActionLinkIds != null && dto.ProgramActionLinkIds.Any())
            {
                var exist = await _programActionRepo.
                    IsBulkIdsExistAsync(dto.ProgramActionLinkIds,
               cancellationToken);
                if (exist["idNotFound"] == true)
                {
                    return ApiResponse<PlanDetailDto>.Fail(
                      Messages.ActionsNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.ActionNotFound
                      );
                }
                if (exist["deletedOrInactive"] == true)
                {
                    return ApiResponse<PlanDetailDto>.Fail
                       (Messages.ProgramActionNotFound,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.ProgramActionNotFound);
                }

            }
            var entity = new Plan
            {
                PlanName = dto.PlanName,
                CreatedBy = loginId
            };

            using var connection = _context.CreateConnection();
            connection.Open();               
            using var transaction = connection.BeginTransaction();

            try
            {
                

                var createdId = await _planRepository.CreatePlanAsync(
                    entity,
                    connection,
                    transaction,
                    cancellationToken);


                

                
                    await _planRepository.BulkInsertActionLinksAsync(
                        createdId,
                        dto.ProgramActionLinkIds,
                        loginId,
                        connection,
                        transaction,
                        cancellationToken);
                

                
                var outdto = await _planRepository.GetByIdAsync
                    (createdId, connection,transaction, cancellationToken);
                transaction.Commit();
                return ApiResponse<PlanDetailDto>.Ok(
                    outdto!,
                    Messages.PlanCreated,
                    StatusCodes.Status201Created);
            }
            catch
            {
                transaction.Rollback(); // 🔥 FULL ROLLBACK (Plan + Links)
                throw;
            }
        }



        public async Task<ApiResponse<bool>> UpdateAsync(int planId, PlanUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            // Uniqueness check when PlanName provided
            if (!string.IsNullOrWhiteSpace(dto.PlanName))
            {
                var existing = await _planRepository.GetByNameAsync(dto.PlanName, cancellationToken);
                if (existing != null && existing.PlanId != planId)
                {
                    if (existing.IsDeleted == false)
                        return ApiResponse<bool>.Fail(Messages.AlreadyExist, StatusCodes.Status409Conflict, ErrorCodes.Conflict);
                    return ApiResponse<bool>.Fail(Messages.AlreadyExist, StatusCodes.Status422UnprocessableEntity, ErrorCodes.Conflict);
                }
            }

            var updated = await _planRepository.UpdatePlanAsync(planId, dto.PlanName, dto.Active, loginId, cancellationToken);
            if (!updated && (dto.ActionLinks == null || dto.ActionLinks.Count == 0))
            {
                return ApiResponse<bool>.Fail(Messages.UpdateFailed, StatusCodes.Status400BadRequest, ErrorCodes.ValidationFailed);
            }

            if (dto.ActionLinks != null && dto.ActionLinks.Count > 0)
            {
                await _planRepository.UpsertPlanProgramActionLinksAsync(planId, dto.ActionLinks, loginId, cancellationToken);
            }

            return ApiResponse<bool>.Ok(true, Messages.Updated);
        }


      

        public async Task<ApiResponse<PagedResultDto<PlanDetailDto>>> 
            GetAllAsync(PlanFilterDto filter, 
            CancellationToken cancellationToken = default)
        {
            var result = await _planRepository.GetAllAsync(filter, cancellationToken);
                     
            return ApiResponse<PagedResultDto<PlanDetailDto>>.Ok(
               result,
               result.TotalCount == 0
                   ? Messages.PlansNotFound
                   : Messages.PlansRetrieved,
                statusCode: StatusCodes.Status200OK
           );
        }


        public async Task<ApiResponse<PlanDetailDto>>
           DeleteAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {


            var existDto =
            await _planRepository.IsIdExistAsync(id,
            cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlanNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlanAlreadyDeleted);
            }

            var rowAffect = await _planRepository.DeleteAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<PlanDetailDto>.Fail(
                     Messages.PlanAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlanAlreadyDeleted);
            }
            var dto =
               await GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<PlanDetailDto>.
               Ok(dto.Data, Messages.PlanDeleted, statusCode: StatusCodes.Status200OK);

         
        }




        public async Task<ApiResponse<PlanDetailDto>>
            RecoverAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {

            var existDto =
              await _planRepository.IsIdExistAsync(id,
              cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlanNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<PlanDetailDto>.Fail(
                    Messages.PlanAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlanAlreadyRecovered);
            }

            var rowAffect = await _planRepository.RecoverAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<PlanDetailDto>.Fail(
                     Messages.PlanAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlanAlreadyRecovered);
            }

            var dto =
               await GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<PlanDetailDto>.
               Ok(dto.Data, Messages.PlanRecovered, statusCode: StatusCodes.Status200OK);
        }


        
        
        
        public async Task<ApiResponse<int>> LinkPlansRoleAsync(int roleId, List<int> planIds, int loginId, CancellationToken cancellationToken = default)
        {
            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null || role.IsDeleted == true)
                return ApiResponse<int>.Fail("Role not found.", StatusCodes.Status404NotFound);

            if (role.IsActive == false)
                return ApiResponse<int>.Fail("Role is inactive.", StatusCodes.Status400BadRequest);

            if (planIds == null || planIds.Count == 0)
                return ApiResponse<int>.Fail("PlanIds are required.", StatusCodes.Status400BadRequest);

            // Validate plan ids exist
            var existing = await _planRepository.GetExistingPlanIdsAsync(planIds, cancellationToken);
            var existingSet = new HashSet<int>(existing);
            var invalid = planIds.Where(p => !existingSet.Contains(p)).ToList();
            if (invalid.Any())
            {
                return ApiResponse<int>.Fail($"Some PlanIds are invalid: {string.Join(',', invalid)}", StatusCodes.Status404NotFound);
            }

            var res = await _planRepository.LinkPlanRoleAsync(roleId, planIds, loginId, cancellationToken);
            if (res <= 0)
            {
                return ApiResponse<int>.Fail(Messages.Failed, StatusCodes.Status400BadRequest);
            }
            return ApiResponse<int>.Ok(res, Messages.Success);
        }

    }
}
