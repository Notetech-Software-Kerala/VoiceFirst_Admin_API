using System;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class BusinessValidationException : BusinessException
    {
        public BusinessValidationException(string message, string errorCode)
            : base(message, errorCode)
        {
        }
    }
}
