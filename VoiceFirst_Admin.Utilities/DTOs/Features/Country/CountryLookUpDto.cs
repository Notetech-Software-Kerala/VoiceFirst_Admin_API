using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Country
{
    public class CountryLookUpDto
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string? DivisionOne { get; set; }
        public string? DivisionTwo { get; set; }
        public string? DivisionThree { get; set; }
    }
}
