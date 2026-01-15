using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class ApplicationVersion
{
    public int ApplicationVersionId { get; set; }

    public int ApplicationId { get; set; }

    public string Version { get; set; } = string.Empty;   // VARCHAR(20)

    public string Type { get; set; } = string.Empty;      // 'IOS' | 'Android' | 'Web' (VARCHAR(10))

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }               // DATETIME2(3)
    public DateTime? UpdatedAt { get; set; }              // DATETIME2(3) NULL
}