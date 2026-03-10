using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class SysBusinessActivityUserCustomFieldLink : PartialBaseModel
    {
        public int SysBusinessActivityUserCustomFieldLinkId { get; set; }
        public int SysBusinessActivityId { get; set; }
        public int SysUserCustomFieldId { get; set; }
        public string? FieldDataType { get; set; }
        public string? FieldName { get; set; }
    }
}
