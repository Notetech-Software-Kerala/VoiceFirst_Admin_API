namespace VoiceFirst_Admin.Utilities.Constants
{
    public static class ErrorCodes
    {
        // ===== COMMON =====
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string NotFound = "NOT_FOUND";
        public const string Conflict = "CONFLICT";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";

        // ===== SYS BUSINESS ACTIVITY =====
        public const string BusinessActivityAlreadyExists = "BUSINESS_ACTIVITY_EXISTS";
        public const string BusinessActivityNotFound = "BUSINESS_ACTIVITY_NOT_FOUND";

        // ===== USER =====
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string UserEmailInvalid = "USER_EMAIL_INVALID";
        public const string UserAlreadyExists = "USER_ALREADY_EXISTS";

        // ===== ROLE =====
        public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";
        public const string RoleNotFound = "ROLE_NOT_FOUND";

        // ===== COMPANY =====
        public const string CompanyNotFound = "COMPANY_NOT_FOUND";
        public const string CompanyAlreadyExists = "COMPANY_ALREADY_EXISTS";
    }
}
