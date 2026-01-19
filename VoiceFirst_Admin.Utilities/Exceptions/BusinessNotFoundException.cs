using System;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class BusinessNotFoundException : BusinessException
    {
        public BusinessNotFoundException(string message, string errorCode)
            : base(message, errorCode)
        {
        }
    }
}
