using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message, int statusCode = StatusCodes.Status200OK, string? error = null)
            => new() {  StatusCode = statusCode, Message = message, Error = error, Data = data };

        public static ApiResponse<T> Fail(string message, int statusCode = StatusCodes.Status400BadRequest, string? error = null)
            => new() { StatusCode = statusCode, Message = message, Error = error, Data = default };
    }
}
