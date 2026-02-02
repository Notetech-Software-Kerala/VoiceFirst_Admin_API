using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<SysRoles, RoleDto>()
            .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.SysRoleId))
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.RoleName))
            .ForMember(d => d.IsMandatory, opt => opt.MapFrom(s => s.IsMandatory))
            .ForMember(d => d.RolePurpose, opt => opt.MapFrom(s => s.RolePurpose))
            .ForMember(d => d.PlatformId, opt => opt.MapFrom(s => s.ApplicationId))
            .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
            .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
            .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
            .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
            .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt));
        CreateMap<SysRoles, RoleDetailDto>()
            .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.SysRoleId))
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.RoleName))
            .ForMember(d => d.IsMandatory, opt => opt.MapFrom(s => s.IsMandatory))
            .ForMember(d => d.RolePurpose, opt => opt.MapFrom(s => s.RolePurpose))
            .ForMember(d => d.PlatformId, opt => opt.MapFrom(s => s.ApplicationId))
            .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
            .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
            .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
            .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
            .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt));

        CreateMap<SysRoles, RoleLookUpDto>()
            .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.SysRoleId))
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.RoleName));

        CreateMap<PlanRoleProgramActionLink, PlanRoleActionLinkDto>()
                .ForMember(d => d.ActionLinkId, opt => opt.MapFrom(s => s.ProgramActionLinkId))
                .ForMember(d => d.ActionName, opt => opt.MapFrom(s => s.ProgramActionName))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive ?? true));
    }
}
