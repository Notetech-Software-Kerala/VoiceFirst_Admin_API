namespace VoiceFirst_Admin.Utilities.Constants
{
    public static class ErrorCodes
    {
        // ===== COMMON =====
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string NotFound = "NOT_FOUND";
        public const string RolesNotFound = "ROLES_NOT_FOUND";
        public const string RolesNotAvailable= "ROLES_NOT_AVAILABLE";
        public const string ActionNotFound = "ACTION_NOT_FOUND";
        public const string PostOfficeNotFound = "POST_OFFICE_NOT_FOUND";
        public const string ZipCodesNotFound = "ZIP_CODES_NOT_FOUND";
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string ZipCodesNotAvailable = "ZIP_CODES_NOT_AVAILABLE";
        public const string DialCodeNotAvailable = "DIAL_CODE_NOT_AVAILABLE";
        public const string DialCodeNotFound = "DIAL_CODE_NOT_FOUND";
        public const string Conflict = "CONFLICT";
        public const string PlanAlreadyExists = "PLAN_NAME_EXISTS";
        public const string EmployeeNotFoundById = "EMPLOYEE_NOT_FOUND";
        public const string SomeRolesAreNotAllowed = "ROLES_ARE_NOT_ALLOWED";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string Payload = "PAYLOAD_REQUIRED";
        public const string InvalidMode = "INVALID_MODE";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string NoRowAffected = "NO_ROW_AFFECTED";
        public const string BusinessActivityAlreadyRecovered =
       "BUSINESS_ACTIVITY_ALREADY_RECOVERED";

        public const string MediaFormatNotLinked = "MEDIA_FORMAT_NOT_LINKED_WITH_THIS_ISSUE_TYPE";
        public const string MediaTypeNotLinked = "MEDIA_TYPE_NOT_LINKED_WITH_THIS_MEDIA_FORMAT";

        public const string PlanAlreadyRecovered =
        "PLAN_ALREADY_RECOVERED";


        public const string ActionsAreAlreadyLinked = "ACTIONS_ARE_ALREADY_LINKED";
        public const string PostOfficeAreAlreadyLinked = "POST_OFFICE_ARE_ALREADY_LINKED";
        public const string ZipCodesAreAlreadyLinked = "ZIP_CODES_ARE_ALREADY_LINKED";
        public const string ZipCodesAlreadyLinkedWithPlace = "ZIP_CODES_ALREADY_LINKED_WITH_PLACE";
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

        public const string PlatformAlreadyExists = "PLATFORM_EXISTS";
        public const string PlatformAlreadyExistsRecoverable = "PLATFORM_EXISTS_RECOVERABLE";
        public const string PlatformNotFound = "PLATFORM_NOT_FOUND";
        public const string PlatformNotActive = "PLATFORM_NOT_ACTIVE";

        public const string ProgramActionAlreadyExists = "PROGRAM_ACTION_EXISTS";
        public const string ProgramActionAlreadyExistsRecoverable = "PROGRAM_ACTION_EXISTS_RECOVERABLE";
        public const string ProgramActionNotFound = "PROGRAM_ACTION_NOT_FOUND";
       
        public const string ProgramNotFoundById = "PROGRAM_NOT_FOUND";
        public const string EmployeeNotAvailable = "EMPLOYEE_NOT_AVAILABLE";
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
        public const string ProgramsNotFound = "PROGRAMS_NOT_FOUND";
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
        public const string InvalidOrExpiredResetLink = "INVALID_OR_EXPIRED_RESET_LINK";
        public const string InvalidResetGrant = "INVALID_RESET_GRANT";
        public const string PasswordResetFailed = "PASSWORD_RESET_FAILED";
        public const string ForgotPasswordLimitExceeded = "FORGOT_PASSWORD_LIMIT_EXCEEDED";
        public const string ForgotPasswordCooldown = "FORGOT_PASSWORD_COOLDOWN";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string AccountLockedOut = "ACCOUNT_LOCKED_OUT";
        public const string AccountInactive = "ACCOUNT_INACTIVE";
        public const string AccountDeleted = "ACCOUNT_DELETED";
        public const string DeviceInfoRequired = "DEVICE_INFO_REQUIRED";
        public const string LoginFailed = "LOGIN_FAILED";
        public const string OldPasswordIncorrect = "OLD_PASSWORD_INCORRECT";
        public const string NewPasswordSameAsOld = "NEW_PASSWORD_SAME_AS_OLD";
        public const string ChangePasswordFailed = "CHANGE_PASSWORD_FAILED";

        // ===== SYS ISSUE TYPE =====
        public const string IssueTypeAlreadyExists = "ISSUE_TYPE_EXISTS";
        public const string IssueTypeAlreadyExistsRecoverable = "ISSUE_TYPE_EXISTS_RECOVERABLE";
        public const string IssueTypeNotFoundById = "ISSUE_TYPE_NOT_FOUND";
        public const string IssueTypeNotAvailable = "ISSUE_TYPE_NOT_FOUND_DELETED_TRUE";
        public const string IssueTypeAlreadyRecovered = "ISSUE_TYPE_ALREADY_RECOVERED";
        public const string IssueTypeAlreadyDeleted = "ISSUE_TYPE_ALREADY_DELETED";

        // ===== SYS ISSUE STATUS =====
        public const string IssueStatusAlreadyExists = "ISSUE_STATUS_EXISTS";
        public const string IssueStatusAlreadyExistsRecoverable = "ISSUE_STATUS_EXISTS_RECOVERABLE";
        public const string IssueStatusNotFoundById = "ISSUE_STATUS_NOT_FOUND";
        public const string IssueStatusNotFound = "ISSUE_STATUS_NOT_FOUND_DELETED_TRUE";
        public const string IssueStatusAlreadyRecovered = "ISSUE_STATUS_ALREADY_RECOVERED";
        public const string IssueStatusAlreadyDeleted = "ISSUE_STATUS_ALREADY_DELETED";

        // ===== SYS ISSUE CHARACTER TYPE =====
        public const string IssueCharacterTypeAlreadyExists = "ISSUE_CHARACTER_TYPE_EXISTS";
        public const string IssueCharacterTypeAlreadyExistsRecoverable = "ISSUE_CHARACTER_TYPE_EXISTS_RECOVERABLE";
        public const string IssueCharacterTypeNotFoundById = "ISSUE_CHARACTER_TYPE_NOT_FOUND";
        public const string IssueCharacterTypeNotFound = "ISSUE_CHARACTER_TYPE_NOT_FOUND_DELETED_TRUE";
        public const string IssueCharacterTypeAlreadyRecovered = "ISSUE_CHARACTER_TYPE_ALREADY_RECOVERED";
        public const string IssueCharacterTypeAlreadyDeleted = "ISSUE_CHARACTER_TYPE_ALREADY_DELETED";

        // ===== SYS ISSUE MEDIA TYPE =====
        public const string IssueMediaTypeAlreadyExists = "ISSUE_MEDIA_TYPE_EXISTS";
        public const string IssueMediaTypeAlreadyExistsRecoverable = "ISSUE_MEDIA_TYPE_EXISTS_RECOVERABLE";
        public const string IssueMediaTypeNotFoundById = "ISSUE_MEDIA_TYPE_NOT_FOUND";
        public const string IssueMediaTypeNotFound = "ISSUE_MEDIA_TYPE_NOT_FOUND_DELETED_TRUE";
        public const string IssueMediaTypeAlreadyRecovered = "ISSUE_MEDIA_TYPE_ALREADY_RECOVERED";
        public const string IssueMediaTypeAlreadyDeleted = "ISSUE_MEDIA_TYPE_ALREADY_DELETED";
        public const string IssueMediaTypeNotActive = "ISSUE_MEDIA_TYPE_NOT_ACTIVE";

        // ===== SYS ISSUE MEDIA FORMAT =====
        public const string IssueMediaFormatAlreadyLinked = "ISSUE_MEDIA_FORMAT_LINKED";
        public const string IssueMediaFormatAlreadyExists = "ISSUE_MEDIA_FORMAT_EXISTS";
        public const string IssueMediaFormatAlreadyExistsRecoverable = "ISSUE_MEDIA_FORMAT_EXISTS_RECOVERABLE";
        public const string IssueMediaFormatNotFoundById = "ISSUE_MEDIA_FORMAT_NOT_FOUND";
        public const string IssueMediaFormatNotFound = "ISSUE_MEDIA_FORMAT_NOT_FOUND_DELETED_TRUE";
        public const string IssueMediaFormatAlreadyRecovered = "ISSUE_MEDIA_FORMAT_ALREADY_RECOVERED";
        public const string IssueMediaFormatAlreadyDeleted = "ISSUE_MEDIA_FORMAT_ALREADY_DELETED";
        public const string IssueMediaFormatNotActive = "ISSUE_MEDIA_FORMAT_NOT_ACTIVE";

        // ===== APPLICATION VERSION =====
        public const string ApplicationVersionAlreadyExists = "APPLICATION_VERSION_EXISTS";
    }
}
