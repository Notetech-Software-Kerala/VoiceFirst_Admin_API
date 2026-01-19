using System;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class BusinessConflictException : BusinessException
    {
        public BusinessConflictException(string message, string errorCode)
            : base(message, errorCode)
        {
        }
    }
}
