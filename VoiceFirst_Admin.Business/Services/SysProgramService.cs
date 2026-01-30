using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysProgramService : ISysProgramService
    {
        private readonly ISysProgramRepo _repo;
        private readonly IApplicationRepo _applicationRepo;
        private readonly IProgramActionRepo _programActionRepo;
        private readonly IMapper _mapper;
        private readonly IDapperContext _context;


        public SysProgramService(
            ISysProgramRepo repo,
            IApplicationRepo applicationRepo,
            IProgramActionRepo programActionRepo,
            IMapper mapper, IDapperContext _context)
        {
            _repo = repo;
            _applicationRepo = applicationRepo;
            _programActionRepo = programActionRepo;
            _mapper = mapper;
            this._context = _context;
        }



        public async Task<ApiResponse<SysProgramDto>>
            CreateAsync(SysProgramCreateDTO dto,
            int loginId,
            CancellationToken cancellationToken = default)
        {
          
            // Platform check (Application)
            var app = await _applicationRepo.
                IsIdExistAsync(dto.PlatformId,
                cancellationToken);

            if (app == null)
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.NotFound,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound
                    );

            if(dto.PlatformId == 1)
            {
                return ApiResponse<SysProgramDto>.Fail(
                        Messages.PlatformNotFound,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.PlatFormNotActive
                        );
            }

            if(app.IsActive == false)
                return ApiResponse<SysProgramDto>.Fail(
                        Messages.PlatformNotFound,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.PlatFormNotActive
                        );


          


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
                        ErrorCodes.ProgramNameAlreadyExistsRecoverable,
                        _mapper.Map<SysProgramDto>(existingByName));

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
                        ErrorCodes.ProgramLabelAlreadyExistsRecoverable,
                        _mapper.Map<SysProgramDto>(existingByLabel));

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
                        ErrorCodes.ProgramRouteAlreadyExistsRecoverable,
                        _mapper.Map<SysProgramDto>(existingByRoute)
                        );

                return ApiResponse<SysProgramDto>.Fail
                    (Messages.ProgramRouteAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramRouteAlreadyExists);
            }

            if (dto.ActionIds != null && dto.ActionIds.Any())
            {
                var exist = await _programActionRepo.
                    IsBulkIdsExistAsync(dto.ActionIds,
               cancellationToken);
                if (exist["idNotFound"] == true)
                {
                    return ApiResponse<SysProgramDto>.Fail(
                      Messages.ActionsNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.ActionNotFound
                      );
                }
                if (exist["deletedOrInactive"] == true)
                {
                    return ApiResponse<SysProgramDto>.Fail
                       (Messages.ProgramActionNotFound,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.ProgramActionNotFound);
                }

            }

            var entity = _mapper.Map<SysProgram>(dto);
            entity.CreatedBy = loginId;

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var createdId = await _repo.CreateAsync
                   (entity,                   
                   connection,
                   transaction, 
                   cancellationToken);

               

                await _repo.BulkInsertActionLinksAsync
                    (
                    createdId,
                    dto.ActionIds,
                    loginId,
                    connection,
                    transaction,
                    cancellationToken);

                var dtoOut = await _repo.GetByIdAsync
                    (createdId,
                     connection,
                    transaction,
                    cancellationToken);

                transaction.Commit();
                return ApiResponse<SysProgramDto>.Ok(
                    dtoOut!,
                    Messages.ProgramCreated,
                   StatusCodes.Status201Created);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<ApiResponse<SysProgramDto>> 
            GetByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var dto = await _repo.GetByIdAsync
                (id, 
                connection, 
                transaction, 
                cancellationToken);

            if (dto == null)
            {

                return ApiResponse<SysProgramDto>.Fail(
                   Messages.ProgramNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound);
            }
            return ApiResponse<SysProgramDto>.Ok(
                dto,
                Messages.ProgramRetrieved,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<SysProgramDto>> DeleteAsync(int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            
          var existDto =
          await _repo.IsIdExistAsync(id,
          cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.ProgramNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);

            if (rowAffect)
            {


                var dto =
               await GetByIdAsync
               (id,
               cancellationToken);

                return ApiResponse<SysProgramDto>.
                   Ok(dto.Data,
                   Messages.ProgramDeleted,
                   statusCode: StatusCodes.Status200OK);

               

            }
            return ApiResponse<SysProgramDto>.Fail(
                     Messages.ProgramAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.ProgramAlreadyDeleted);

        }


        public async Task<ApiResponse<SysProgramDto>> RecoverProgramAsync(
        int id,
        int loginId,
        CancellationToken cancellationToken = default)
        {
           
            var existDto =
            await _repo.IsIdExistAsync(id,
            cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.ProgramNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.ProgramAlreadyRecovered);
            }

            var rowAffected = await _repo.RecoverProgramAsync
                (id, loginId, cancellationToken);

            if (!rowAffected)
            {

                return ApiResponse<SysProgramDto>.Fail(
                     Messages.ProgramAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.ProgramAlreadyRecovered);
            }


            var dto =
               await GetByIdAsync
               (id,
               cancellationToken);

            return ApiResponse<SysProgramDto>.
               Ok(dto.Data, 
               Messages.ProgramRecovered, 
               statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<PagedResultDto<SysProgramDto>>> GetAllAsync(
            SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            
            var result = await _repo.GetAllAsync(filter, cancellationToken);

            return ApiResponse<PagedResultDto<SysProgramDto>>.Ok(
                result,
                result.TotalCount == 0
                    ? Messages.ProgramsNotFound
                    : Messages.ProgramRetrieved,
                 statusCode: StatusCodes.Status200OK
            );
        }


        public async Task<ApiResponse<List<SysProgramLookUp>>>
            GetAllActiveByApplicationIdAsync(
            int applicationId,
            CancellationToken cancellationToken = default)
        {


            // Platform check (Application)
            var app = await _applicationRepo.
                IsIdExistAsync(applicationId,
                cancellationToken);

            if (app == null)
                return ApiResponse<List<SysProgramLookUp>>.Fail(
                    Messages.ApplicationNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlatFormNotFound
                    );

            if (app.IsActive == false)
                return ApiResponse<List<SysProgramLookUp>>.Fail(
                        Messages.PlatformNotFound,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.PlatFormNotActive
                        );

            var items = await _repo.GetProgramLookupAsync(
                applicationId, cancellationToken);

            return ApiResponse<List<SysProgramLookUp>>.Ok(
                items,
                Messages.ProgramRetrieved,
                StatusCodes.Status200OK);                    
        }


        public async Task<ApiResponse<List<SysProgramLookUp>>>
          GetAllActiveForPlanAsync(
          CancellationToken cancellationToken = default)
        {


            // Platform check (Application)
            var app = await _applicationRepo.
                IsIdExistAsync(2,
                cancellationToken);

            if (app == null || app.IsActive == false)
                return ApiResponse<List<SysProgramLookUp>>.Fail(
                    Messages.ApplicationNotFoundById,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.PlatFormNotFound
                    );

          
            var items = await _repo.GetProgramLookupAsync(
                2, cancellationToken);

            return ApiResponse<List<SysProgramLookUp>>.Ok(
                items,
                Messages.ProgramRetrieved,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<List<SysProgramLookUp>>>
            GetProgramLookupAsync(CancellationToken cancellationToken = default)
        {
            var result = await _repo.GetProgramLookupAsync(null,cancellationToken)
                        ?? new List<SysProgramLookUp>();

            return ApiResponse<List<SysProgramLookUp>>.Ok(
                result,
                result.Count == 0
                    ? Messages.NoActivePrograms
                    : Messages.ProgramRetrieved,
                statusCode: StatusCodes.Status200OK
            );
        }


   


        public async Task<ApiResponse<List<SysProgramActionLinkLookUp>>>
            GetActionLookupByProgramIdAsync(int programId, 
            CancellationToken cancellationToken = default)
        {
           
            // Platform check (program)
            var program = await _repo.
                IsIdExistAsync(programId,
                cancellationToken);

            if (program == null)
                return ApiResponse<List<SysProgramActionLinkLookUp>>.Fail(
                    Messages.ProgramNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.ProgramNotFoundById
                    );

            if (program.Active == false || program.Deleted == true)
                return ApiResponse<List<SysProgramActionLinkLookUp>>.Fail(
                        Messages.ProgramNotFound,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.ProgramNotFoundById
                        );
            var items = await _repo.
                GetActionLookupByProgramIdAsync
                (programId, cancellationToken);

            return ApiResponse<List<SysProgramActionLinkLookUp>>.
                Ok(items, Messages.ProgramRetrieved);
        }



        public async Task<ApiResponse<SysProgramDto>> UpdateAsync(
            int programId,
            SysProgramUpdateDTO dto,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var actionUpdation = false;
            // Platform check (program)
            var program = await _repo.
                IsIdExistAsync(programId,
                cancellationToken);

            if (program == null)
                return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.ProgramNotFoundById
                    );

            if (program.Deleted == true)
                return ApiResponse<SysProgramDto>.Fail(
                        Messages.ProgramNotFound,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.ProgramNotFoundById
                        );

            if (program.PlatformId != dto.PlatformId 
                && (dto.PlatformId != null || dto.PlatformId >=0))
            {

                var app = await _applicationRepo.IsIdExistAsync(
                          dto.PlatformId.Value,
                          cancellationToken);

                if (app == null)
                    return ApiResponse<SysProgramDto>.Fail(
                        Messages.ApplicationNotFoundById,
                        StatusCodes.Status404NotFound,
                        ErrorCodes.PlatFormNotFound
                        );

                if (app.IsActive == false)
                    return ApiResponse<SysProgramDto>.Fail(
                            Messages.PlatformNotFound,
                            StatusCodes.Status409Conflict,
                            ErrorCodes.PlatFormNotActive
                            );
            }

            var applicationId = dto.PlatformId ?? program.PlatformId;


            if (!string.IsNullOrWhiteSpace(dto.ProgramName))
            {
                var existingByName = await _repo.ExistsByNameAsync
                    (applicationId, dto.ProgramName, programId, cancellationToken);

                if (existingByName != null)
                {

                    if (existingByName.IsDeleted == true)

                        return ApiResponse<SysProgramDto>.Fail
                            (Messages.ProgramNameAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.ProgramNameAlreadyExistsRecoverable,
                            _mapper.Map<SysProgramDto>(existingByName));

                    return ApiResponse<SysProgramDto>.Fail
                        (Messages.ProgramNameAlreadyExists,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.ProgramNameAlreadyExists);
                   
                }               
            }

            if (!string.IsNullOrWhiteSpace(dto.Label))
            {
                var existingByLabel = await _repo.ExistsByLabelAsync
                    (applicationId, dto.Label, programId, cancellationToken);
                if (existingByLabel != null)
                {
                    if (existingByLabel.IsDeleted == true)

                        return ApiResponse<SysProgramDto>.
                            Fail(Messages.ProgramLabelAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.ProgramLabelAlreadyExistsRecoverable,
                            _mapper.Map<SysProgramDto>(existingByLabel));

                    return ApiResponse<SysProgramDto>.Fail(
                        Messages.ProgramLabelAlreadyExists,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.ProgramLabelAlreadyExists);
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Route))
            {
                var existingByRoute = await _repo.ExistsByRouteAsync
                    (applicationId, dto.Route, programId, cancellationToken);
                if (existingByRoute != null)
                {
                    if (existingByRoute.IsDeleted == true)
                        return ApiResponse<SysProgramDto>.Fail
                            (Messages.ProgramRouteAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.ProgramRouteAlreadyExistsRecoverable,
                            _mapper.Map<SysProgramDto>(existingByRoute)
                            );

                    return ApiResponse<SysProgramDto>.Fail
                        (Messages.ProgramRouteAlreadyExists,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.ProgramRouteAlreadyExists);
                }
            }


           
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();


            try
            {
                 var programUpdation = await _repo.UpdateAsync
               (_mapper.Map<SysProgram>((dto, programId, loginId)),
                 connection,
                 transaction,
                 cancellationToken);




                if (dto.UpdateActions != null)
                {
                    var updationActionsFound =
                     await _repo.CheckProgramActionLinksExistAsync(
                         programId,
                         dto.UpdateActions
                             .Select(x => x.ActionId)
                             .ToList(),
                         true,
                         connection,
                         transaction,
                         cancellationToken);
                    if (!updationActionsFound)
                    {
                        transaction.Rollback();
                        return ApiResponse<SysProgramDto>.Fail(
                          Messages.NotFound,
                          StatusCodes.Status404NotFound,
                          ErrorCodes.NotFound
                          );
                    }

                    actionUpdation = await _repo.BulkUpdateActionLinksAsync(
                             programId,
                             dto.UpdateActions,
                             loginId,
                             connection,
                             transaction,
                             cancellationToken);
                }



                if (dto.InsertActions != null)
                {
                    var linkedActionsFound =
                    await _repo.CheckProgramActionLinksExistAsync(
                        programId,
                        dto.InsertActions,
                        false,
                        connection,
                        transaction,
                        cancellationToken);
                    if (linkedActionsFound)
                    {
                        transaction.Rollback();
                        return ApiResponse<SysProgramDto>.Fail(
                          Messages.ActionsAreAlreadyLinked,
                          StatusCodes.Status409Conflict,
                          ErrorCodes.ActionsAreAlreadyLinked
                          );
                    }

                    var exist = await _programActionRepo.
                     IsBulkIdsExistAsync(dto.InsertActions,
                      cancellationToken);
                    if (exist["idNotFound"] == true)
                    {
                        return ApiResponse<SysProgramDto>.Fail(
                       Messages.ActionsNotFound,
                       StatusCodes.Status404NotFound,
                       ErrorCodes.ActionNotFound
                       );
                    }
                    if (exist["deletedOrInactive"] == true)
                    {
                        return ApiResponse<SysProgramDto>.Fail
                           (Messages.ProgramActionNotFound,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.ProgramActionNotFound);
                    }

                    actionUpdation = await _repo.BulkInsertActionLinksAsync(
                               programId,
                               dto.InsertActions,
                               loginId,
                               connection,
                               transaction,
                               cancellationToken);
                }

                                                                                                
                if (!actionUpdation && !programUpdation)
                {
                    return ApiResponse<SysProgramDto>.Fail(
                    Messages.ProgramUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);
                }
                
                var dtoOut = await _repo.GetByIdAsync(programId,
                    connection,transaction, cancellationToken);

                transaction.Commit();

                return ApiResponse<SysProgramDto>.Ok(
                    dtoOut!,
                    Messages.ProgramUpdated,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                transaction.Rollback();
                throw;  
            }
        }


      
    }
}
