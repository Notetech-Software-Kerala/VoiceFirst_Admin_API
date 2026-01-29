using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class PostOfficeFilterDto :CommonFilterDto
    {
        public PostOfficeSearchBy? SearchBy { get; set; }
    }
}

public enum PostOfficeSearchBy
{
    PostOfficeName,
    CountryName,
    DivOneName,
    DivTwoName,
    DivThreeName,
    ZipCode,
    CreatedUser,
    UpdatedUser,
    DeletedUser
}
