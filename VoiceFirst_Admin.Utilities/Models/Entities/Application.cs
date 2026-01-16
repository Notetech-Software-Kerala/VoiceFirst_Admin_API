using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;


public class Application : BaseModel
{

    public int ApplicationId { get; set; }

    public string ApplicationName { get; set; } = string.Empty;
}
