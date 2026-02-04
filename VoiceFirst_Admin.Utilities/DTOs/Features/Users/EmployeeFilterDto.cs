using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Users
{
    public class EmployeeFilterDto : CommonFilterDto
    {
        public EmployeeSearchBy? SearchBy { get; set; }
    }

    public enum EmployeeSearchBy
    {
        FirstName,
        LastName,
        Gender,
        Email,
        MobileNo,
        BirthYear,
        MobileCountryCode,
        CreatedUser,
        ModifiedUser,
        DeletedUser
    }
    
}
