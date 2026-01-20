
using System;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class BusinessRecoverableException : BusinessException
    {
        public BusinessRecoverableException(string message, string errorCode)
            : base(message, errorCode)
        {
        }
    }
}
