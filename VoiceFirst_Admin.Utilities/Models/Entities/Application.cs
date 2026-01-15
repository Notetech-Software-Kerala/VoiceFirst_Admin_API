using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace VoiceFirst_Admin.Utilities.Models.Entities;


public class Application 
{

    public int ApplicationId { get; set; }

    public string ApplicationName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
