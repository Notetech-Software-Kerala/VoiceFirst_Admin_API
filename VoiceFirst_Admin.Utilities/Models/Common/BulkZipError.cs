using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common
{
    public class BulkUpsertError
    {
        public string Message { get; set; } = "";
        public int StatusCode  { get; set; }      
    }
    public class BulkUpsertResult
    {
        public bool Success { get; set; }
        public int? Id { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}
