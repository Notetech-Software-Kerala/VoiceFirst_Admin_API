using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class PostOfficeFilterDto :CommonFilterDto
    {
        public int? CountryId { get; set; }
        public int? DivOneId { get; set; }
        public int? DivTwoId { get; set; }
        public int? DivThreeId { get; set; }
        public PostOfficeSearchBy? SearchBy { get; set; }
    }
    public class PostOfficeLookUpFilterDto 
    {
        public int? CountryId { get; set; }
        public int? DivOneId { get; set; }
        public int? DivTwoId { get; set; }
        public int? DivThreeId { get; set; }
        
    }
    public class PostOfficeLookUpWithZipCodeFilterDto : PostOfficeLookUpFilterDto
    {
        public string? ZipCode { get; set; }
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
