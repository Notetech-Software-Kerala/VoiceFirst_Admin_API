using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Users
{
    public class EmployeeCreateDto
    {      
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Gender { get; set; }
        public string? MobileNo { get; set; }
        public short? BirthYear { get; set; }
        public int DialCodeId { get; set; }

        private List<int> _roleIds = new();

        public List<int> RoleIds
        {
            get => _roleIds;
            set => _roleIds = value?
                .Distinct()
                .ToList()
                ?? new List<int>();
        }

    }
}
