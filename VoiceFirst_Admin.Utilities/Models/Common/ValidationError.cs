using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common;
public sealed class ValidationError
{
    public string Field { get; init; }
    public string Message { get; init; }
    public string? Code { get; init; }
}
