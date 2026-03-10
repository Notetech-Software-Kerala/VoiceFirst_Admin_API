using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class SysUserCustomFieldFilterDto : VoiceFirst_Admin.Utilities.DTOs.Shared.CommonFilterDto
    {
        public SysUserCustomFieldSearchBy? SearchBy { get; set; }
    }

    public enum SysUserCustomFieldSearchBy
    {
        FieldName,
        FieldKey,
        FieldDataType,
        CreatedUser,
        UpdatedUser,
        DeletedUser
    }
}
