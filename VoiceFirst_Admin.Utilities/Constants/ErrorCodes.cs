namespace VoiceFirst_Admin.Utilities.Constants
{
    public static class ErrorCodes
    {
        // ===== COMMON =====
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string NotFound = "NOT_FOUND";
        public const string RolesNotFound = "ROLES_NOT_FOUND";
        public const string RolesNotAvailable= "ROLES_NOT_AVAILABEL";
        public const string ActionNotFound = "ACTION_NOT_FOUND";
        public const string PostOfficeNotFound = "POST_OFFICE_NOT_FOUND";
        public const string ZipCodesNotFound = "ZIP_CODES_NOT_FOUND";
        public const string ZipCodesNotAvaliable = "ZIP_CODES_NOT_AVALIABLE";
        public const string MobileCountryCodeNotAvaliable = "MOBILE_COUNTRY_CODE_NOT_AVALIABLE";
        public const string MobileCountryCodeNotFound = "MOBILE_COUNTRY_CODE_NOT_FOUND";
        public const string Conflict = "CONFLICT";
        public const string PlanAlreadyExists = "PLAN_NAME_EXISTS";
        public const string EmployeeNotFoundById = "EMPLOYEE_NOT_FOUND";

        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string Payload = "PAYLOAD_REQUIRED";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string NoRowAffected = "NO_ROW_AFFECTED";
        public const string BusinessActivityAlreadyRecovered =
       "BUSINESS_ACTIVITY_ALREADY_RECOVERED";

        public const string PlanAlreadyRecovered =
        "PLAN_ALREADY_RECOVERED";


        public const string ActionsAreAlreadyLinked = "ACTIONS_ARE_ALREADY_LINKED";
        public const string PostOfficeAreAlreadyLinked = "POST_OFFICE_ARE_ALREADY_LINKED";
        public const string ZipCodesAreAlreadyLinked = "ZIP_CODES_ARE_ALREADY_LINKED";
        public const string PlaceAlreadyRecovered =
   "PLACE_ALREADY_RECOVERED";

        public const string BusinessActivityAlreadyDeleted =
    "BUSINESS_ACTIVITY_ALREADY_DELETED";


        public const string PlanAlreadyDeleted =
    "PLAN_ALREADY_DELETED";

        public const string PlaceAlreadyDeleted =
  "PLACE_ALREADY_DELETED";

        public const string ProgramAlreadyRecovered =
   "PROGRAM_ALREADY_RECOVERED";


        public const string EmployeeAlreadyRecovered =
   "EMPLOYEE_ALREADY_RECOVERED";

        public const string ProgramAlreadyDeleted =
"PROGRAM_ALREADY_DELETED";


        public const string EmployeeAlreadyDeleted =
"EMPLOYEE_ALREADY_DELETED";

        public const string PlatFormAlreadyExists = "PLATFORM_EXISTS";
        public const string PlatFormAlreadyExistsRecoverable = "PLATFORM_EXISTS_RECOVERABLE";
        public const string PlatFormNotFound = "PLATFORM_NOT_FOUND";
        public const string PlatFormNotActive = "PLATFORM_NOT_ACTIVE";

        public const string ProgramActionAlreadyExists = "PROGRAM_ACTION_EXISTS";
        public const string ProgramActionAlreadyExistsRecoverable = "PROGRAM_ACTION_EXISTS_RECOVERABLE";
        public const string ProgramActionNotFound = "PROGRAM_ACTION_NOT_FOUND";
       
        public const string ProgramNotFoundById = "PROGRAM_NOT_FOUND";
        public const string EmployeeNotAvaliable = "EMPLOYEE_NOT_AVAILABLE";
        public const string ProgramAlreadyExistsRecoverable = "PROGRAM_EXISTS_RECOVERABLE";
        public const string ProgramNameAlreadyExists = "PROGRAM_NAME_EXISTS";
        public const string EmployeeEmailAlreadyExists = "EMPLOYEE_EMAIL_EXISTS";
        public const string EmployeeMobileNoAlreadyExists = "EMPLOYEE_MOBILE_NO_EXISTS";
        public const string ProgramLabelAlreadyExists = "PROGRAM_LABEL_EXISTS";
        public const string ProgramRouteAlreadyExists = "PROGRAM_ROUTE_EXISTS";
        public const string EmployeeEmailAlreadyExistsRecoverable = "EMPLOYEE_EMAIL_EXISTS_RECOVERABLE";
        public const string EmployeeMobileNoAlreadyExistsRecoverable = "EMPLOYEE_MOBILE_EXISTS_RECOVERABLE";

        public const string ProgramNameAlreadyExistsRecoverable = "PROGRAM_NAME_EXISTS_RECOVERABLE";
        public const string ProgramLabelAlreadyExistsRecoverable = "PROGRAM_LABEL_EXISTS_RECOVERABLE";
        public const string ProgramRouteAlreadyExistsRecoverable = "PROGRAM_ROUTE_EXISTS_RECOVERABLE";

        // ===== SYS BUSINESS ACTIVITY =====
        public const string BusinessActivityAlreadyExists = "BUSINESS_ACTIVITY_EXISTS";
        public const string PlaceAlreadyExists = "PLACE_ACTIVITY_EXISTS";
        public const string PlanAlreadyExistsRecoverable = "PLAN_EXISTS_RECOVERABLE";
        public const string PlaceAlreadyExistsRecoverable = "PLACE_EXISTS_RECOVERABLE";
        public const string BusinessActivityAlreadyExistsRecoverable = "BUSINESS_ACTIVITY_EXISTS_RECOVERABLE";
        public const string BusinessActivityNotFound = "BUSINESS_ACTIVITY_NOT_FOUND_DELETED_TRUE";
        public const string BusinessActivityNotFoundById = "BUSINESS_ACTIVITY_NOT_FOUND";
        public const string PlaceNotFoundById = "PLACE_NOT_FOUND";
        public const string PlaceNotFound = "PLACE_NOT_FOUND_DELETED_TRUE";
        public const string PlanNotFoundById = "PLAN_NOT_FOUND";

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

        // ===== AUTH =====
        public const string InvalidOrExpiredToken = "INVALID_OR_EXPIRED_TOKEN";
        public const string PasswordResetFailed = "PASSWORD_RESET_FAILED";
        public const string ForgotPasswordLimitExceeded = "FORGOT_PASSWORD_LIMIT_EXCEEDED";
        public const string OtpAttemptsExceeded = "OTP_ATTEMPTS_EXCEEDED";
        public const string ForgotPasswordCooldown = "FORGOT_PASSWORD_COOLDOWN";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string AccountInactive = "ACCOUNT_INACTIVE";
        public const string AccountDeleted = "ACCOUNT_DELETED";
        public const string DeviceInfoRequired = "DEVICE_INFO_REQUIRED";
        public const string LoginFailed = "LOGIN_FAILED";
    }
}
