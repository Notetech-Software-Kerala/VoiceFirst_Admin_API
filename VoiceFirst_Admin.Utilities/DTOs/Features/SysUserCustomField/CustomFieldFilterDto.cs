using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldFilterDto : CommonFilterDto
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
