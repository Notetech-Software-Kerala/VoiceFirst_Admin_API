using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class ZipLinkValidationDto
    {
        public int PostOfficeZipCodeLinkId { get; set; }
        public bool ZipCodeLinkIsActive { get; set; }
        public bool PostOfficeIsActive { get; set; }
        public bool PostOfficeIsDeleted { get; set; }
        public bool CountryIsActive { get; set; }
        public bool CountryIsDeleted { get; set; }
        public bool Division1IsActive { get; set; }
        public bool Division1IsDeleted { get; set; }
        public bool Division2IsActive { get; set; }
        public bool Division2IsDeleted { get; set; }
        public bool Division3IsActive { get; set; }
        public bool Division3IsDeleted { get; set; }
    }
}
