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

        public const string PlatFormAlreadyExists = "PLATFORM_EXISTS";
        public const string PlatFormAlreadyExistsRecoverable = "PLATFORM_EXISTS_RECOVERABLE";
        public const string PlatFormNotFound = "PLATFORM_NOT_FOUND";

        public const string ProgramActionAlreadyExists = "PROGRAM_ACTION_EXISTS";
        public const string ProgramActionAlreadyExistsRecoverable = "PROGRAM_ACTION_EXISTS_RECOVERABLE";
        public const string ProgramActionNotFound = "PROGRAM_ACTION_NOT_FOUND";


        public const string ProgramAlreadyExistsRecoverable = "PROGRAM_EXISTS_RECOVERABLE";
        public const string ProgramNameAlreadyExists = "PROGRAM_NAME_EXISTS";
        public const string ProgramLabelAlreadyExists = "PROGRAM_LABEL_EXISTS";
        public const string ProgramRouteAlreadyExists = "PROGRAM_ROUTE_EXISTS";
        public const string ProgramNameAlreadyExistsRecoverable = "PROGRAM_NAME_EXISTS_RECOVERABLE";
        public const string ProgramLabelAlreadyExistsRecoverable = "PROGRAM_LABEL_EXISTS_RECOVERABLE";
        public const string ProgramRouteAlreadyExistsRecoverable = "PROGRAM_ROUTE_EXISTS_RECOVERABLE";

        // ===== SYS BUSINESS ACTIVITY =====
        public const string BusinessActivityAlreadyExists = "BUSINESS_ACTIVITY_EXISTS";
        public const string BusinessActivityAlreadyExistsRecoverable = "BUSINESS_ACTIVITY_EXISTS_RECOVERABLE";
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
