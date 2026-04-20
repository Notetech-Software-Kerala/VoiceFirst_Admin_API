using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.BusinessActivityUserCustomFieldLink
{
    public class BusinessActivityUserCustomFieldDto : PartialCommonDto
    {
        public int ActivityCustomFieldLinkId { get; set; }
        public int CustomFieldLinkId { get; set; }
        public int ActivityId { get; set; }
        public string? FieldDataType { get; set; }
        public string? FieldName { get; set; }
    }
}
