using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseDescriptionAttribute : Attribute
{
    public int StatusCode { get; }
    public string Description { get; }

    public SwaggerResponseDescriptionAttribute(int statusCode, string description)
    {
        StatusCode = statusCode;
        Description = description;
    }
}