using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class ApplicationVersion : BaseModel
{
    public int ApplicationVersionId { get; set; }

    public int ApplicationId { get; set; }

    public string Version { get; set; } = string.Empty;   // VARCHAR(20)

    public string Type { get; set; } = string.Empty;      // 'IOS' | 'Android' | 'Web' (VARCHAR(10))
}
