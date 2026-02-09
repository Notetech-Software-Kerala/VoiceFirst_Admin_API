using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceCreateDTO
    {
        public string PlaceName { get; set; } = string.Empty;

        private List<int> _zipCodeLinkIds = new();
        
        public List<int> ZipCodeLinkIds
        {
            get => _zipCodeLinkIds;
            set => _zipCodeLinkIds = value?
                .Distinct()
                .ToList()
                ?? new List<int>();
        }
    }
}
