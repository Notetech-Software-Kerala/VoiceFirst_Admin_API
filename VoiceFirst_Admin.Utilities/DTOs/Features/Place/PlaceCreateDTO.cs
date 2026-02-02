using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceCreateDTO
    {
        public string PlaceName { get; set; } = string.Empty;

        private List<int> _postOfficeIds = new();

        public List<int> PostOfficeIds
        {
            get => _postOfficeIds;
            set => _postOfficeIds = value?
                .Distinct()
                .ToList()
                ?? new List<int>();
        }
    }
}
