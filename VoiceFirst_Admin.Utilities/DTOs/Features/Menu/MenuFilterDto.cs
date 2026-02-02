using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu
{
    public class MenuFilterDto : CommonFilterDto
    {
        public MenuSearchBy? SearchBy { get; set; }
        public int? PlateFormId { get; set; }
    }
    public enum MenuSearchBy
    {
        MenuName,
        MenuRoute,
        MenuIcon,
        CreatedUser,
        UpdatedUser,
        DeletedUser
    }

}
