using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseDescriptionAttribute : Attribute
{
    public int StatusCode { get; }
    public string Description { get; }   // Swagger response description column

    public string Message { get; }       // Example JSON message field

    public string DataJson { get; }      // Example JSON data field (valid JSON)
    public string? Error { get; }        // Example JSON error field
    public SwaggerResponseDescriptionAttribute(
        int statusCode,
        string description,
        string message,
        string dataJson = "null",
        string? error = null)
    {
        StatusCode = statusCode;
        Description = description;
        Message = message;
        DataJson = dataJson;

        Error = error;
    }
}
