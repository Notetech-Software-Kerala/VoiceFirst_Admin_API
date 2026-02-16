using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.Security;

namespace VoiceFirst_Admin.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly IUserRoleLinkRepo _userRoleLinkRepo;
        private readonly IMapper _mapper;
        private readonly IDapperContext _dapperContext;
        private readonly ICountryRepo _countryRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IConfiguration _configuration;
        private readonly ISessionService _sessionService;
        public UserService(
            IUserRepo repo,
            IUserRoleLinkRepo userRoleLinkRepo,
            IMapper mapper,
            IDapperContext dapperContext,
            ICountryRepo countryRepo,
            IRoleRepo roleRepo,
            IConfiguration configuration,
            ISessionService sessionService)
        {
            _repo = repo;
            _mapper = mapper;
            _dapperContext = dapperContext;
            _countryRepo = countryRepo;
            _userRoleLinkRepo = userRoleLinkRepo;
            _roleRepo = roleRepo;
            _configuration = configuration;
            _sessionService = sessionService;
        }





        public async Task<ApiResponse<EmployeeDetailDto>> CreateAsync(
        EmployeeCreateDto employee,
        int loginId,
        CancellationToken cancellationToken)
        {


            var country = await _countryRepo.
                IsIdExistAsync(employee.MobileCountryCodeId,
                cancellationToken);


            if (country == null)
            {
                return ApiResponse<EmployeeDetailDto>.Fail(
                  Messages.MobileCountryCodeNotFound,
                  StatusCodes.Status404NotFound,
                  ErrorCodes.MobileCountryCodeNotFound
                  );
            }
            if (country.Deleted == true || country.Active == false)
            {
                return ApiResponse<EmployeeDetailDto>.Fail
                   (Messages.MobileCountryCodeNotAvailable,
                   StatusCodes.Status409Conflict,
                   ErrorCodes.MobileCountryCodeNotAvaliable);
            }


            var existingByEmail =
             await _repo.CheckEmailExistsAsync
             (employee.Email, null, cancellationToken);

            if (existingByEmail != null)
            {
                if (existingByEmail.IsDeleted == true)

                    return ApiResponse<EmployeeDetailDto>.Fail
                        (Messages.EmployeeEmailAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.EmployeeEmailAlreadyExistsRecoverable,
                        new EmployeeDetailDto
                        {
                            EmployeeId = existingByEmail.UserId,

                        });

                return ApiResponse<EmployeeDetailDto>.Fail
                    (Messages.EmployeeEmailAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.EmployeeEmailAlreadyExists);
            }

            var existingByMobileNo = await _repo.CheckMobileNoExistsAsync
                (employee.MobileNo, null, cancellationToken);


            if (existingByMobileNo != null)
            {
                if (existingByMobileNo.IsDeleted == true)

                    return ApiResponse<EmployeeDetailDto>.
                        Fail(Messages.EmployeeMobileNoAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.EmployeeMobileNoAlreadyExistsRecoverable,
                        new EmployeeDetailDto
                        {
                            EmployeeId = existingByMobileNo.UserId,

                        });

                return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.EmployeeMobileNoAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.EmployeeMobileNoAlreadyExists);
            }


            if (employee.RoleIds != null && employee.RoleIds.Any())
            {
                var exist = await _roleRepo.
                    IsBulkIdsExistAsync(employee.RoleIds,
               cancellationToken);
                if (exist["idNotFound"] == true)
                {
                    return ApiResponse<EmployeeDetailDto>.Fail(
                      Messages.RolesNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.RolesNotFound
                      );
                }
                if (exist["deletedOrInactive"] == true)
                {
                    return ApiResponse<EmployeeDetailDto>.Fail
                       (Messages.RolesNotAvailable,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.RolesNotAvailable);
                }

            }

            // Generate a strong temporary password
            var password = await PasswordGenerator.GenerateRandomString(4);

            // Hash and salt the password
            var hashResult = await PasswordHasher.HashPasswordAsync(password);
            var HashKey = Convert.FromBase64String(hashResult.Hash);
            var SaltKey = Convert.FromBase64String(hashResult.Salt);

            var entity = _mapper.Map<Users>(employee, opts =>
            {
                opts.Items["CreatedBy"] = loginId;
                opts.Items["HashKey"] = HashKey;
                opts.Items["SaltKey"] = SaltKey;
            });

         

            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {


             
                var createdId = await _repo.CreateAsync
                   (entity,
                   connection,
                   transaction,
                   cancellationToken);

                await _userRoleLinkRepo.BulkInsertUserRoleLinksAsync
                    (
                    createdId,
                    employee.RoleIds,
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
                // Send welcome email with temporary password
                EmailHelper.SendMail(new EmailDTO
                {
                    from_email = _configuration["Email:FromEmail"]!,
                    from_email_password = _configuration["Email:FromPassword"],
                    to_email = employee.Email,
                    email_subject = "Welcome to VoiceFirst Admin",
                    email_html_body = $"<p>Dear {employee.FirstName},</p><p>Your account has been created.</p><p>Temporary password: <strong>{password}</strong></p><p>Please sign in and change your password immediately.</p>",
                    signature_content = "<p>Regards,<br/>VoiceFirst Admin Team</p>"
                });
                return ApiResponse<EmployeeDetailDto>.Ok(
                    dtoOut!,
                    Messages.EmployeeCreated,
                   StatusCodes.Status201Created);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

       



        public async Task<ApiResponse<EmployeeDetailDto>> DeleteAsync(int id,
           int loginId,
           CancellationToken cancellationToken = default)
        {

            var existDto =
            await _repo.IsIdExistAsync(id,
            cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.EmployeeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.EmployeeNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.EmployeeAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.EmployeeAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);

            if (rowAffect)
            {
                await _sessionService.InvalidateAllUserSessionsAsync(id);

                var dto =
               await GetByIdAsync
               (id,
               cancellationToken);

                return ApiResponse<EmployeeDetailDto>.
                   Ok(dto.Data,
                   Messages.EmployeeDeleted,
                   statusCode: StatusCodes.Status200OK);



            }
            return ApiResponse<EmployeeDetailDto>.Fail(
                     Messages.EmployeeAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.EmployeeAlreadyDeleted);

        }



    
        public async Task<ApiResponse<EmployeeDetailDto>> RecoverAsync(
        int id,
        int loginId,
        CancellationToken cancellationToken = default)
        {

            var existDto =
            await _repo.IsIdExistAsync(id,
            cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<EmployeeDetailDto>.Fail(
                   Messages.EmployeeNotFoundById,
                   StatusCodes.Status404NotFound,
                   ErrorCodes.EmployeeNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.EmployeeAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.EmployeeAlreadyRecovered);
            }

            var rowAffected = await _repo.RecoverAsync
                (id, loginId, cancellationToken);

            if (!rowAffected)
            {

                return ApiResponse<EmployeeDetailDto>.Fail(
                     Messages.EmployeeAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.EmployeeAlreadyRecovered);
            }


            var dto =
               await GetByIdAsync
               (id,
               cancellationToken);

            return ApiResponse<EmployeeDetailDto>.
               Ok(dto.Data,
               Messages.EmployeeRecovered ,
               statusCode: StatusCodes.Status200OK);
        }





        public async Task<ApiResponse<PagedResultDto<EmployeeDto>>> GetAllAsync(
            EmployeeFilterDto filter,int loginUserId,
            CancellationToken cancellationToken = default)
        {

            var result = await _repo.GetAllAsync(filter, loginUserId, cancellationToken);

            return ApiResponse<PagedResultDto<EmployeeDto>>.Ok(
                result,
                result.TotalCount == 0
                    ? Messages.EmployeesNotFound
                    : Messages.EmployeeRetrieved,
                 statusCode: StatusCodes.Status200OK
            );
        }



        public async Task<ApiResponse<EmployeeDetailDto>> UpdateAsync(
            int updateUserId,
            EmployeeUpdateDto dto,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var roleUpdation = false;

            var employee = await _repo.
                IsIdExistAsync(updateUserId,
                cancellationToken);

            if (employee == null)
                return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.EmployeeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.EmployeeNotFoundById
                    );

            if (employee.Deleted == true)
                return ApiResponse<EmployeeDetailDto>.Fail(
                        Messages.EmployeeNotAvailable,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.EmployeeNotAvaliable
                        );

            if (employee.MobileCountryCodeId != dto.MobileCountryCodeId
                && (dto.MobileCountryCodeId != null || dto.MobileCountryCodeId >= 0))
            {

                var country = await _countryRepo.
              IsIdExistAsync(employee.MobileCountryCodeId,
              cancellationToken);


                if (country == null)
                {
                    return ApiResponse<EmployeeDetailDto>.Fail(
                      Messages.MobileCountryCodeNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.MobileCountryCodeNotFound
                      );
                }
                if (country.Deleted == true || country.Active == false)
                {
                    return ApiResponse<EmployeeDetailDto>.Fail
                       (Messages.MobileCountryCodeNotAvailable,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.MobileCountryCodeNotAvaliable);
                }
            }




            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingByEmail = await _repo.CheckEmailExistsAsync
                    (dto.Email, updateUserId, cancellationToken);

                if (existingByEmail != null)
                {

                    if (existingByEmail.IsDeleted == true)

                        return ApiResponse<EmployeeDetailDto>.Fail
                            (Messages.EmployeeEmailAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.EmployeeEmailAlreadyExistsRecoverable,
                            _mapper.Map<EmployeeDetailDto>(existingByEmail));

                    return ApiResponse<EmployeeDetailDto>.Fail
                        (Messages.EmployeeEmailAlreadyExists,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.EmployeeEmailAlreadyExists);

                }
            }

            if (!string.IsNullOrWhiteSpace(dto.MobileNo))
            {
                var existingByMobileNo = await _repo.CheckMobileNoExistsAsync
                    ( dto.MobileNo, updateUserId, cancellationToken);
                if (existingByMobileNo != null)
                {
                    if (existingByMobileNo.IsDeleted == true)

                        return ApiResponse<EmployeeDetailDto>.
                            Fail(Messages.EmployeeMobileNoAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.EmployeeMobileNoAlreadyExistsRecoverable,
                            _mapper.Map<EmployeeDetailDto>(existingByMobileNo));

                    return ApiResponse<EmployeeDetailDto>.Fail(
                        Messages.EmployeeMobileNoAlreadyExists,
                        StatusCodes.Status409Conflict,
                        ErrorCodes.EmployeeMobileNoAlreadyExists);
                }
            }

       

            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();


            try
            {
                var employeeUpdation = await _repo.UpdateAsync
              (_mapper.Map<Users>((dto, updateUserId, loginId)),
                connection,
                transaction,
                cancellationToken);




                if (dto.UpdateRoles != null)
                {
                    var updationRolesFound =
                     await _userRoleLinkRepo.CheckUserRoleLinksExistAsync(
                         updateUserId,
                         dto.UpdateRoles
                             .Select(x => x.RoleId)
                             .ToList(),
                         true,
                         connection,
                         transaction,
                         cancellationToken);
                    if (!updationRolesFound)
                    {
                        transaction.Rollback();
                        return ApiResponse<EmployeeDetailDto>.Fail(
                          Messages.RolesNotFound,
                          StatusCodes.Status404NotFound,
                          ErrorCodes.RolesNotFound
                          );
                    }

                    roleUpdation = await _userRoleLinkRepo.BulkUpdateUserRoleLinksAsync(
                             updateUserId,
                             dto.UpdateRoles,
                             loginId,
                             connection,
                             transaction,
                             cancellationToken);
                }



                if (dto.InsertRoles != null)
                {
                    var linkedRolesFound =
                    await _userRoleLinkRepo.CheckUserRoleLinksExistAsync(
                        updateUserId,
                        dto.InsertRoles,
                        false,
                        connection,
                        transaction,
                        cancellationToken);
                    if (linkedRolesFound)
                    {
                        transaction.Rollback();
                        return ApiResponse<EmployeeDetailDto>.Fail(
                          Messages.RolesNotFound,
                          StatusCodes.Status404NotFound,
                          ErrorCodes.RolesNotFound
                          );
                    }

                    var exist = await _roleRepo.
                     IsBulkIdsExistAsync(dto.InsertRoles,
                      cancellationToken);
                    if (exist["idNotFound"] == true)
                    {
                        return ApiResponse<EmployeeDetailDto>.Fail(
                       Messages.RolesNotAvailable,
                       StatusCodes.Status404NotFound,
                       ErrorCodes.RolesNotAvailable
                       );
                    }
                    if (exist["deletedOrInactive"] == true)
                    {
                        return ApiResponse<EmployeeDetailDto>.Fail
                           (Messages.RolesNotAvailable,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.RolesNotAvailable);
                    }

                    roleUpdation = await _userRoleLinkRepo.BulkInsertUserRoleLinksAsync(
                               updateUserId,
                               dto.InsertRoles,
                               loginId,
                               connection,
                               transaction,
                               cancellationToken);
                }


                if (!roleUpdation && !employeeUpdation)
                {
                    return ApiResponse<EmployeeDetailDto>.Fail(
                    Messages.UserUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);
                }

                var dtoOut = await _repo.GetByIdAsync(updateUserId,
                    connection, transaction, cancellationToken);

                transaction.Commit();

                if (dto.Active == false)
                {
                    await _sessionService.InvalidateAllUserSessionsAsync(updateUserId);
                }

                return ApiResponse<EmployeeDetailDto>.Ok(
                    dtoOut!,
                    Messages.EmployeeUpdated,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }



        public async Task<ApiResponse<EmployeeDetailDto>>
           GetByIdAsync(int id,
           CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var dto = await _repo.GetByIdAsync
                (id,
                connection,
                transaction,
                cancellationToken);

            if (dto == null)
            {

                return ApiResponse<EmployeeDetailDto>.Fail(
                   Messages.EmployeeNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.EmployeeNotFoundById);
            }
            return ApiResponse<EmployeeDetailDto>.Ok(
                dto,
                Messages.EmployeeRetrieved,
                StatusCodes.Status200OK);
        }


    }
}
