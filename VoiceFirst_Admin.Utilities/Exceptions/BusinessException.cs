

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public abstract class BusinessException : Exception
    {
        public string ErrorCode { get; }

        protected BusinessException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
