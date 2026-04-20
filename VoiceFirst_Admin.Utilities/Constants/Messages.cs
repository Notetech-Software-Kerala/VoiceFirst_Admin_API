using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Constants
{
    public class Messages
    {
        public const string SomeRolesAreNotAllowed =
            "One or more selected roles cannot be assigned.";
        // General
        public const string Success = "Success.";
        public const string Failed = "Failed.";
        public const string MediaFormatIsNotFound = "Media format is not found.";
        public const string MediaTypeIsNotFound = "Media type is not found.";
        public const string ValidationFailed = "Validation failed.";
        public const string SomethingWentWrong = "Something went wrong. Please try again.";
        public const string ContactAdmin = "Contact admin.";
        public const string AlreadyExist = "Already exist.";
        public const string AlreadyUsing = "Already using.";
        public const string Created = "Created.";
        public const string UpdateFailed = "Update failed.";
        public const string BadRequest = "Bad Request.";
        public const string Nil = "NIL.";
        public const string Unauthorized = "Unauthorized.";
        public const string PayloadInvalid = "Invalid request payload.";
        public const string InternalServerError= "Unexpected server error.";
        public const string EmployeeUpdated = "Employee updated successfully.";
        // New generic messages
        public const string PayloadRequired = "Payload is required.";
        public const string InvalidMode = "The 'mode' query parameter is required and must be 'cookie' or 'token'.";
        public const string NotFound = "Not found.";
        public const string PostOfficesAreAlreadyLinked = "Some post offices are already linked.Please refresh again.";
        public const string ZipCodesAreAlreadyLinked = "Some Zip codes  are already linked.Please refresh again.";
        public const string ZipCodesAlreadyLinkedWithPlace = "These zip codes are already linked with this place.";

        public const string ActionsAreAlreadyLinked = "Some actions are already linked.Please refresh again.";
        public const string Updated = "Updated.";
        public const string ActionsNotFound = "Actions not found.";
        public const string PostOfficesNotFound = "Post offices not found.";
        public const string ZipCodesNotFound = "Zip codes not found.";
        public const string DialCodeNotFound = "Dial code not found.";
        // Auth
        public const string InvalidCredentials = "Invalid credentials.";
        public const string UserNotFound = "User not found.";
        public const string UnauthorizedAccess = "Unauthorized access.";
        public const string InvalidEmail = "Invalid email.";
        public const string InvalidPassword = "Invalid password.";
        public const string EmailNotExist = "Email does not exist.";
        public const string PhoneNumberNotExist = "Phone number does not exist.";
        public const string PhoneNumberAlreadyExists = "Phone number already exists.";
        public const string UsernameAlreadyTaken = "Username already taken.";
        public const string AlreadyRegistered = "Already registered.";
        public const string NotRegistered = "Not registered.";
        public const string RegistrationSuccess = "Registration successful.";
        public const string LoginSuccess = "Login successful.";
        public const string NameAlreadyExists = "Name already exists.";
        public const string NameExistsInTrash = "This name exists in trash. Restore it to use again.";
        // OTP
        public const string InvalidOtp = "Invalid OTP.";
        public const string OtpExpired = "OTP expired.";
        public const string ForgotPasswordLimitExceeded = "You have exceeded the maximum number of password reset requests. Please try again after 2 minutes.";
        public const string OtpAttemptsExceeded = "Too many invalid OTP attempts. Please request a new OTP.";
        public const string ForgotPasswordCooldown = "Please wait before requesting another OTP.";

        // Login / Logout
        public const string LogoutSuccess = "Logged out successfully.";
        public const string AccountLockedOut = "Account is locked due to too many failed login attempts. Please try again after {0} minutes.";
        public const string InvalidCredentialsWithAttempts = "Invalid credentials. {0} attempt(s) remaining before lockout.";
        public const string AccountInactive = "Your account is inactive. Please contact support.";
        public const string AccountDeleted = "Your account has been deleted. Please contact support.";
        public const string DeviceInfoRequired = "Device information is required.";
        public const string FullDeviceInfoRequired = "Full device details are required for a new device or application version.";

        // Change Password
        public const string ChangePasswordSuccess = "Password changed successfully. Please login again on other devices.";
        public const string OldPasswordIncorrect = "Current password is incorrect.";
        public const string NewPasswordSameAsOld = "New password cannot be the same as the current password.";
        public const string ChangePasswordFailed = "Password change failed. Please try again.";


        //User Profile
        public const string UserProfile = "User profile retrieved successfully.";
        public const string UserProfileNoUpdation = "No changes detected. The submitted values are the same as the current profile.";
        public const string UserProfileUpdated = "User profile updated successfully.";
        public const string UserProfileUpdateFailed = "Failed to update user profile. Please try again.";


        // Existing from your newer list (keep if you use them)
        public const string UserInactive = "User is inactive.";
        public const string UserDeleted = "User is deleted.";
        public const string EmailAlreadyExists = "Email already exists.";
        public const string MobileAlreadyExists = "Mobile number already exists.";
        public const string UserCreated = "User created successfully.";
        public const string UserUpdated = "User updated successfully.";
        public const string UserDeletedSuccess = "User deleted successfully.";

        

        // ===== SYS BUSINESS ACTIVITY =====
        public const string BusinessActivityAlreadyExists =
            "Activity name already exists.Please add another name";



        public const string PlaceAlreadyExists =
           "Place  already exists.Please add another name";
        public const string PlanAlreadyExists =
          "Place  already exists.Please add another name";

        public const string BusinessActivityAlreadyExistsRecoverable =
            "Activity already exists but was deleted. You can recover it.";

        public const string PlaceAlreadyExistsRecoverable =
          "Place already exists but was deleted. You can recover it.";



        public const string PlanAlreadyExistsRecoverable =
          "Plan already exists but was deleted. You can recover it.";

        public const string BusinessActivityCreated =
            "Activity  created successfully.";

        public const string PlaceCreated =
           "Place  created successfully.";

        public const string PlaceWasAlreadyExistZipCodeLinked =
          "Place was already exist .so it was linked with zipcodes.";

        public const string PlanCreated =
           "Plan  created successfully.";

        public const string BusinessActivityRecovered =
            "Activity recovered successfully.";

        public const string PlanRecovered =
          "Plan recovered successfully.";

        public const string BusinessActivityNotFoundById =
            "Activity  not found.";


        public const string PlanNotFoundById =
         "Plan  not found.";

        public const string PlaceNotFoundById =
            "Place  not found.";

        public const string ApplicationNotFoundById =
            "Platform  not found.";

        public const string NoPlatformsFound =
          "No platforms  found.";

        public const string BusinessActivitiesNotFound =
             "Activities  not found.";

        public const string PlansNotFound =
            "Plans  not found.";
        public const string PlacesNotFound =
           "Places  not found.";

        public const string ProgramsNotFound =
             "Programs  not found.";
        public const string EmployeesNotFound =
     "Employees  not found.";


        public const string BusinessActivityDeleted =
            "Activity  deleted sucessfully.";

        public const string PlanDeleted =
            "Plan  deleted sucessfully.";

        public const string PlaceDeleted =
            "Place  deleted sucessfully.";

        public const string BusinessActivityRetrieved =
            "Activity retrieve sucessfully.";

        public const string PlaceRetrieved =
            "Place retrieve sucessfully.";

        public const string BusinessActivityUpdated =
            "Activity updated sucessfully.";

        public const string PlaceUpdated =
           "Place updated sucessfully.";


        public const string BusinessActivityCreationFailed =
            "Activity Createion failed";

        public const string NoActiveBusinessActivities =
         "No active  activities available.";

        public const string NoActivePlaces =
        "No active  places available.";

        public const string NoActiveDialCodes =
   "No active  Dial codes available.";

        public const string NoActivePrograms =
       "No active  programs available.";

        public const string BusinessActivitiesRetrieved =
            "Activity activities retrieved successfully.";

        public const string PlansRetrieved =
               "Plans retrieved successfully.";


        public const string PlacesRetrieved =
       "Places retrieved successfully.";



        public const string DialCodesRetrieved =
       "Dial Codes retrieved successfully.";


        public const string PlatformsRetrieved =
            "Platforms retrieved successfully";

        public const string PlanRetrieved =
           "Plan retrieved successfully.";

        public const string PlanUpdated =
           "Plan updated successfully.";

        // ===== SYS Program  =====
        public const string InvalidActionLinksForApplication =
            "Invalid action links for selected application.";
        public const string ProgramNameAlreadyExists =
            "Program name already exists in this application.";

        public const string EmployeeEmailAlreadyExists =
      "Employee email already exists.";


        public const string EmployeeMobileNoAlreadyExists =
      "Employee mobile no already exists.";

        public const string PlanNameAlreadyExists =
           "Plan name already exists ";
        public const string ProgramLabelAlreadyExists =
     "Program label already exists in this application.";

        public const string ProgramRouteAlreadyExists =
     "Program route already exists in this application.";

        public const string ProgramAlreadyExistsRecoverable =
            "Program already exists but was deleted. You can recover it.";

        public const string ProgramNameAlreadyExistsRecoverable =
         "Program  already exists with this name but was deleted. You can recover it.";

 

        public const string EmployeeEmailAlreadyExistsRecoverable =
        "Employee  already exists with this email but was deleted. You can recover it.";

        public const string EmployeeMobileNoAlreadyExistsRecoverable =
       "Employee  already exists with this mobile no but was deleted. You can recover it.";

        public const string PlanNameAlreadyExistsRecoverable =
 "Plan  already exists with this name but was deleted. You can recover it.";


        public const string ProgramLabelAlreadyExistsRecoverable =
         "Program  already exists with this label but was deleted. You can recover it.";

        public const string ProgramRouteAlreadyExistsRecoverable =
         "Program  already exists with this route but was deleted. You can recover it.";

        public const string ProgramCreated =
            "Program created successfully.";
        public const string EmployeeCreated =
            "Employee created successfully.";

        public const string ProgramRecovered =
            "Program recovered successfully.";


        public const string EmployeeRecovered =
            "Employee recovered successfully.";

        public const string PlaceRecovered =
           "Place recovered successfully.";


        public const string ProgramNotFoundById =
            "Program not found.";

        public const string EmployeeNotFoundById =
    "Employee not found.";

        public const string ProgramDeleted =
            "Program deleted sucessfully.";

        public const string EmployeeDeleted =
         "Employee deleted sucessfully.";


        public const string ProgramRetrieved =
            "Program retrieve sucessfully.";

        public const string EmployeeRetrieved =
           "Employee retrieve sucessfully.";

        public const string ProgramUpdated =
            "Program updated sucessfully.";

       

        public const string BaseRefreshMessage =
            "Please refresh and try again.";

        // ===== SYS Program ACTIVITY =====
        public const string PlatformNotFound =
         "Selected platform is not available." + BaseRefreshMessage;

        public const string NoApplicationPermission =
        "Selected platform is not available." + BaseRefreshMessage;

        public const string BusinessActivityNotFound =
         "Selected activity is not available." + BaseRefreshMessage;


        public const string PlaceNotFound =
         "Selected place is not available." + BaseRefreshMessage;

        public const string ProgramActionNotFound =
         "Some selected actions are no longer available."+ BaseRefreshMessage;

        public const string RolesNotAvailable =
      "Some selected roles are no longer available." + BaseRefreshMessage;

        public const string PostOfficeNotFound =
       "Some selected post office are no longer available." + BaseRefreshMessage;


        public const string ZipCodesNotAvailable =
       "Some selected zip codes are no longer available." + BaseRefreshMessage;

        public const string DialCodeNotAvailable =
      "Some selected  dial code  are no longer available." + BaseRefreshMessage;

        public const string EmployeeNotAvailable =
    "Some selected  employee  are no longer available." + BaseRefreshMessage;

        public const string ProgramNotFound =
         "Some selected program are no longer available." + BaseRefreshMessage;

        public const string BusinessActivityAlreadyRecovered =
       "Activity is already recovered.Please refresh again.";

        public const string PlanAlreadyRecovered =
     "Plan is already recovered.Please refresh again.";

        public const string PlaceAlreadyRecovered =
       "Place is already recovered.Please refresh again.";

        public const string ProgramAlreadyRecovered =
       "Program is already recovered.Please refresh again.";

        public const string EmployeeAlreadyRecovered =
       "Employee is already recovered.Please refresh again.";

        public const string BusinessActivityAlreadyDeleted =
     "Activity is already deleted.Please refresh again.";

        public const string PlanAlreadyDeleted =
    "Plan is already deleted.Please refresh again.";

        public const string PlaceAlreadyDeleted =
   "Place is already deleted.Please refresh again.";

        public const string ProgramAlreadyDeleted =
    "Program is already deleted.Please refresh again.";

        public const string EmployeeAlreadyDeleted =
"Employee is already deleted.Please refresh again.";

        public const string ProgramActionRestoreSucessfully = "Program action restored sucessfully.";
        public const string ProgramActionDeleteSucessfully = "Program action deleted sucessfully.";
        public const string ProgramActionAlreadyDeleted = "Program action already deleted.";
        public const string ProgramActionAlreadyRestored = "Program action already restored.";
        public const string ProgramActionCreated = "Program action created sucessfully.";
        public const string ProgramActionRetrieveSucessfully = "Program action retrieve sucessfully.";
        public const string ProgramActionUpdatedSucessfully = "Program action updated sucessfully.";
        public const string ProgramActionNameAlreadyExists = "Program action name already exists.";
        public const string ProgramActionNameExistsInTrash = "This program action name exists in trash. Restore it to use again.";

        // ===== SYS ROLES =====
        public const string RoleCreated = "Role created sucessfully.";
        public const string RoleFailed = "Role creation failed.";
        public const string RoleRetrieveSucessfully = "Role retrieve sucessfully.";
        public const string RoleUpdatedSucessfully = "Role updated sucessfully.";
        public const string RoleDeleteSucessfully = "Role deleted sucessfully.";
        public const string RoleRestoreSucessfully = "Role restored sucessfully.";
        public const string PlanAlreadyRemoved = "Plan already removed.";
        public const string PlanAlreadyLinked = "Plan already linked.";
        public const string ProgramActionAlreadyLinked = "Program action already linked.";
        public const string ProgramActionLinkNotFound= "Program action link Not found.";
        public const string RoleNameAlreadyExists = "Role name already exists.";
        public const string RoleNameExistsInTrash = "This role name exists in trash. Restore it to use again.";
        public const string RoleNameDefault = "This is a system default role and cannot be created..";
        public const string RoleAlreadyDeleted = "Role already deleted.";
        public const string RoleAlreadyRestored = "Role already restored.";
        public const string RolesNotFound = "Roles Not found .";
        public const string PlanRoleLinkNotFound = "Plan role link id Not found .";
        public const string CountryRetrieveSucessfully = "Country retrieve sucessfully.";
        public const string DivisionOneRetrieveSucessfully = "Division one retrieve sucessfully.";
        public const string DivisionTwoRetrieveSucessfully = "Division two retrieve sucessfully.";
        public const string DivisionThreeRetrieveSucessfully = "Division three retrieve sucessfully.";

        public const string PostOfficeRestoreSucessfully = "Post office restored sucessfully.";
        public const string RequiredPostOfficeIds = "PostOfficeIds are required.";
        public const string PostOfficeDeleteSucessfully = "Post office deleted sucessfully.";
        public const string PostOfficeZipCodeRestoreSucessfully = "Zip code restored sucessfully.";
        public const string PostOfficeZipCodeDeleteSucessfully = "Zip code deleted sucessfully.";
        public const string PostOfficeAlreadyDeleted = "Post office already deleted.";
        public const string PostOfficeAlreadyRestored = "Post office already restored.";
        public const string PostOfficeCreated = "Post office created sucessfully.";
        public const string PostOfficeRetrieveSucessfully = "Post office retrieve sucessfully.";
        public const string ZipCodesRetrieveSucessfully = "Zip codes retrieve sucessfully.";
        public const string PostOfficeUpdatedSucessfully = "Post office updated sucessfully.";
        public const string PostOfficeNameAlreadyExists = "Post office name already exists.";
        public const string PostOfficeNameExistsInTrash = "This post office name exists in trash. Restore it to use again.";

        // Menu
        public const string MenuProgramLinkNotFound =
         "Some selected program are not link with menu.";
        public const string MenuRestoreSucessfully = "Menu restored sucessfully.";
        public const string MenuDeleteSucessfully = "Menu deleted sucessfully.";
        public const string AppMenuRestoreSucessfully = "App menu restored sucessfully.";
        public const string AppMenuDeleteSucessfully = "App menu deleted sucessfully.";
        public const string WebMenuRestoreSucessfully = "Web menu restored sucessfully.";
        public const string WebMenuDeleteSucessfully = "Web menu deleted sucessfully.";
        public const string MenuAlreadyDeleted = "Menu already deleted.";
        public const string MenuAlreadyRestored = "Menu already restored.";
        public const string AppMenuAlreadyDeleted = "App menu already deleted.";
        public const string AppMenuAlreadyRestored = "App menu already restored.";
        public const string WebMenuAlreadyDeleted = "Web menu already deleted.";
        public const string WebMenuAlreadyRestored = "Web menu already restored.";
        public const string MenuCreated = "Menu created sucessfully.";
        public const string MenuUpdatedSucessfully = "Menu updated sucessfully.";
        public const string WebMenuUpdatedSucessfully = "Web menu updated sucessfully.";
        public const string AppMenuUpdatedSucessfully = "App menu updated sucessfully.";
        public const string MenuRetrieveSucessfully = "Menu retrieve sucessfully.";
        public const string WebMenuRetrieveSucessfully = "Web menu retrieve sucessfully.";
        public const string AppMenuRetrieveSucessfully = "App Menu retrieve sucessfully.";
        public const string MenuNameAlreadyExists = "Menu name already exists in the plateform.";
        public const string MenuRountAlreadyExists = "Menu route already exists in the plateform.u can use it.";
        public const string MenuRountAlreadyExistsWithIsDelete = "Menu route already exists in the plateform.u can use it.";
        public const string MenuMasterAlreadyExistsInWeb = "Menu master already exists in web.";
        public const string MenuMasterAlreadyExistsInApp = "Menu master already exists in app.";
        public const string MenuNameExistsInTrash = "This menu name exists in trash. Restore it to use again.";
        public const string WebMenuExistsInTrash = "This web menu  exists in trash. Restore it to use again.";
        public const string AppMenuExistsInTrash = "This app menu  exists in trash. Restore it to use again.";
        public const string ParentMenuNotFound = "Parent menu not found..";
        public const string CannotAddOrUpdate = "Cannot add/reorder children under a parent that has a route.";

        // Country / Division messagesup
        public const string CountryRequired = "Country is required.";
        public const string CountryNotFound = "Selected country not found.";
        public const string DivisionOneNotFound = "Selected division one not found for the country.";
        public const string DivisionTwoNotFound = "Selected division two not found for the division one.";
        public const string DivisionThreeNotFound = "Selected division three not found for the division two.";
        public const string DivisionOneRequiredForDivisionTwo = "Division one is required when supplying division two.";
        public const string CountryRequiredForDivisionOne = "Country is required when supplying division one.";
        public const string CountryRequiredForDPlace = "Country is required when supplying place.";
        public const string DivisionTwoRequiredForDivisionThree = "Division two is required when supplying division three.";

        // ===== FORGOT / RESET PASSWORD =====
        public static string ForgotPasswordEmailSent(string email)
        {
            return $"Please check the email address {email} for instructions to reset your password.";
        }
        public static string AccountSuspendedEmailSent(string email)
        {
            return $"Please check the email address {email} for instructions to reset your password.";
        }
        public const string ResetTokenValid = "Reset link is valid.";
        public const string ResetPasswordSuccess = "Password has been reset successfully.";
        public const string InvalidOrExpiredToken = "The token is invalid or has expired.";
        public const string InvalidOrExpiredResetLink = "The reset link is invalid or has expired.";
        public const string InvalidResetGrant = "Invalid or expired password reset grant. Please restart the process.";
        public const string PasswordResetFailed = "Password reset failed. Please try again.";

        // ===== SYS ISSUE TYPE =====
        public const string IssueTypeAlreadyExists =
            "Issue type name already exists. Please add another name.";
        public const string IssueTypeAlreadyExistsRecoverable =
            "Issue type already exists but was deleted. You can recover it.";
        public const string IssueTypeCreated =
            "Issue type created successfully.";
        public const string IssueTypeRecovered =
            "Issue type recovered successfully.";
        public const string IssueTypeNotFoundById =
            "Issue type not found.";
        public const string IssueTypesNotFound =
            "Issue types not found.";
        public const string IssueTypeDeleted =
            "Issue type deleted successfully.";
        public const string IssueTypeRetrieved =
            "Issue type retrieved successfully.";
        public const string IssueTypesRetrieved =
            "Issue types retrieved successfully.";
        public const string IssueTypeUpdated =
            "Issue type updated successfully.";
        public const string NoChangesDetected =
            "No changes detected. Please modify at least one field and try again.";
        public const string NoActiveIssueTypes =
            "No active issue types available.";
        public const string IssueTypeAlreadyRecovered =
            "Issue type is already recovered. Please refresh again.";
        public const string IssueTypeAlreadyDeleted =
            "Issue type is already deleted. Please refresh again.";
        public const string IssueTypeNotAvailable =
            "Selected issue type is not available." + BaseRefreshMessage;

        // ===== SYS ISSUE STATUS =====
        public const string IssueStatusAlreadyExists = "Issue status already exists. Please add another name.";
        public const string IssueStatusAlreadyExistsRecoverable = "Issue status already exists but was deleted. You can recover it.";
        public const string IssueStatusCreated = "Issue status created successfully.";
        public const string IssueStatusRecovered = "Issue status recovered successfully.";
        public const string IssueStatusNotFoundById = "Issue status not found.";
        public const string IssueStatusesNotFound = "Issue statuses not found.";
        public const string IssueStatusDeleted = "Issue status deleted successfully.";
        public const string IssueStatusRetrieved = "Issue status retrieved successfully.";
        public const string IssueStatusesRetrieved = "Issue statuses retrieved successfully.";
        public const string IssueStatusUpdated = "Issue status updated successfully.";
        public const string NoActiveIssueStatuses = "No active issue statuses available.";
        public const string IssueStatusAlreadyRecovered = "Issue status is already recovered. Please refresh again.";
        public const string IssueStatusAlreadyDeleted = "Issue status is already deleted. Please refresh again.";
        public const string IssueStatusNotFound = "Selected issue status is not available." + BaseRefreshMessage;

        // ===== SYS ISSUE CHARACTER TYPE =====
        public const string IssueCharacterTypeAlreadyExists = "Issue character type already exists. Please add another name.";
        public const string IssueCharacterTypeAlreadyExistsRecoverable = "Issue character type already exists but was deleted. You can recover it.";
        public const string IssueCharacterTypeCreated = "Issue character type created successfully.";
        public const string IssueCharacterTypeRecovered = "Issue character type recovered successfully.";
        public const string IssueCharacterTypeNotFoundById = "Issue character type not found.";
        public const string IssueCharacterTypesNotFound = "Issue character types not found.";
        public const string IssueCharacterTypeDeleted = "Issue character type deleted successfully.";
        public const string IssueCharacterTypeRetrieved = "Issue character type retrieved successfully.";
        public const string IssueCharacterTypesRetrieved = "Issue character types retrieved successfully.";
        public const string IssueCharacterTypeUpdated = "Issue character type updated successfully.";
        public const string NoActiveIssueCharacterTypes = "No active issue character types available.";
        public const string IssueCharacterTypeAlreadyRecovered = "Issue character type is already recovered. Please refresh again.";
        public const string IssueCharacterTypeAlreadyDeleted = "Issue character type is already deleted. Please refresh again.";
        public const string IssueCharacterTypeNotFound = "Selected issue character type is not available." + BaseRefreshMessage;

        // ===== SYS ISSUE MEDIA TYPE =====
        public const string IssueMediaTypeAlreadyExists = "Issue media type already exists. Please add another name.";
        public const string IssueMediaTypeAlreadyExistsRecoverable = "Issue media type already exists but was deleted. You can recover it.";
        public const string IssueMediaTypeCreated = "Issue media type created successfully.";
        public const string IssueMediaTypeRecovered = "Issue media type recovered successfully.";
        public const string IssueMediaTypeNotFoundById = "Issue media type not found.";
        public const string IssueMediaTypesNotFound = "Issue media types not found.";
        public const string IssueMediaTypeDeleted = "Issue media type deleted successfully.";
        public const string IssueMediaTypeRetrieved = "Issue media type retrieved successfully.";
        public const string IssueMediaTypesRetrieved = "Issue media types retrieved successfully.";
        public const string IssueMediaTypeUpdated = "Issue media type updated successfully.";
        public const string NoActiveIssueMediaTypes = "No active issue media types available.";
        public const string IssueMediaTypeAlreadyRecovered = "Issue media type is already recovered. Please refresh again.";
        public const string IssueMediaTypeAlreadyDeleted = "Issue media type is already deleted. Please refresh again.";
        public const string IssueMediaTypeNotAvailable = "Selected issue media type is not available." + BaseRefreshMessage;
        public const string IssueMediaTypeNotActive = "Selected issue media type is inactive." + BaseRefreshMessage;

        // ===== SYS ISSUE MEDIA FORMAT =====
        public const string IssueMediaFormatAlreadyLinked = "Issue media format is already linked.";
        public const string IssueMediaFormatAlreadyExists = "Issue media format already exists. Please add another name.";
        public const string IssueMediaFormatAlreadyExistsRecoverable = "Issue media format already exists but was deleted. You can recover it.";
        public const string IssueMediaFormatCreated = "Issue media format created successfully.";
        public const string IssueMediaFormatRecovered = "Issue media format recovered successfully.";
        public const string IssueMediaFormatNotFoundById = "Issue media format not found.";
        public const string IssueMediaFormatsNotFound = "Issue media formats not found.";
        public const string IssueMediaFormatDeleted = "Issue media format deleted successfully.";
        public const string IssueMediaFormatRetrieved = "Issue media format retrieved successfully.";
        public const string IssueMediaFormatsRetrieved = "Issue media formats retrieved successfully.";
        public const string IssueMediaFormatUpdated = "Issue media format updated successfully.";
        public const string NoActiveIssueMediaFormats = "No active issue media formats available.";
        public const string IssueMediaFormatAlreadyRecovered = "Issue media format is already recovered. Please refresh again.";
        public const string IssueMediaFormatAlreadyDeleted = "Issue media format is already deleted. Please refresh again.";
        public const string IssueMediaFormatNotFound = "Selected issue media format is not available." + BaseRefreshMessage;
        public const string IssueMediaFormatNotActive = "Selected issue media format is inactive." + BaseRefreshMessage;

        //----------------------- Custom field ------------------------

        public const string CustomFieldAlreadyExists = "Custom field already exists. Please add another name.";
        public const string CustomFieldAlreadyExistsByActivity = "Custom field already exists in this activity. Please add another name.";
        public const string CustomFieldKeyAlreadyExists = "Custom field key already exists. Please add another.";
        public const string CustomFieldNameAlreadyExists = "Custom field name already exists. Please add another.";
        public const string CustomFieldDataTypeNotExists = "Custom field dataType not exists."+ BaseRefreshMessage;
        public const string ValidationRuleNotExists = "Validation rule not exists."+ BaseRefreshMessage;
        public const string CustomFieldAlreadyExistsRecoverable = "Custom field key already exists but was deleted. You can recover it.";
        public const string CustomNameAlreadyExistsRecoverable = "Custom field name already exists but was deleted. You can recover it.";
        public const string CustomNameAndDataTypeAlreadyExistsRecoverable = "Custom field name and data type already linked.";
        public const string CustomFieldCreated = "Custom field created successfully.";
        public const string CustomFieldRecovered = "Custom field recovered successfully.";
        public const string CustomFieldNotFoundById = "Custom field not found.";
        public const string CustomFieldAlreadyRequestedStatus = "Custom field link is already in the requested status.";
        public const string CustomFieldsNotFound = "Custom fields not found.";
        public const string CustomFieldDeleted = "Custom field deleted successfully.";
        public const string CustomFieldRestored = "Custom field restored successfully.";
        public const string CustomFieldDeletionFailed = "Failed to delete custom field.";
        public const string CustomFieldRestoreFailed = "Failed to restore custom field.";
        public const string CustomFieldRetrieved = "Custom field retrieved successfully.";
        public const string CustomFieldsRetrieved = "Custom fields retrieved successfully.";
        public const string CustomFieldUpdated = "Custom field updated successfully.";
        public const string NoActiveCustomFields = "No active custom fields available.";
        public const string CustomFieldAlreadyRecovered = "Custom field is already recovered. Please refresh again.";
        public const string CustomFieldAlreadyDeleted = "Custom field is already deleted. Please refresh again.";
        public const string CustomFieldNotFound = "Selected custom field is not available." + BaseRefreshMessage;
        public const string CustomFieldNotActive = "Selected custom field is inactive." + BaseRefreshMessage;
        public const string CustomFieldValidationRuleNameRequired = "Rule name is required.";
        public const string CustomFieldValidationRuleValueRequired = "Rule value is required.";
        public const string CustomFieldValidationMessageRequired = "Message is required.";
        public const string CustomFieldValidationRequired = "Rule name, rule value, and message are required.";
        public const string CustomFieldOptionLabelRequired = "Label is required.";
        public const string CustomFieldOptionValueRequired = "Value is required.";
        public const string CustomFieldOptionRequired = "Custom Field Option is required.";
        public const string CustomFieldValidationIdRequired = "Custom field validation id is required.";
        public const string CustomFieldValidationAtLeastRequired = "At least one field must be provided for update";
        public const string CustomFieldOptionIdRequired = "Custom field Option id is required.";
        public const string CustomFieldOptionAtLeastRequired = "At least one field must be provided for update";
        public const string CustomFieldOptionInvalidVarchar = "Option value must be a valid text.";
        public const string CustomFieldOptionInvalidNVarchar = "Option value must be a valid unicode text.";
        public const string CustomFieldOptionInvalidInt = "Option value must be a valid integer.";
        public const string CustomFieldOptionInvalidFloat = "Option value must be a valid float.";
        public const string CustomFieldOptionInvalidDecimal = "Option value must be a valid decimal.";
        public const string CustomFieldOptionInvalidBit = "Option value must be a valid boolean (true/false/0/1).";
        public const string CustomFieldOptionInvalidDateTime = "Option value must be a valid date and time.";
        public const string CustomFieldOptionInvalidDate = "Option value must be a valid date.";
        public const string CustomFieldOptionInvalid = "Option value is invalid.";
        public const string CustomFieldValueDataTypeRequired = "Value data type is required.";
        // ===== APPLICATION VERSION =====
        public const string ApplicationVersionAlreadyExists = "This version already exists for the selected application and type.";
        public const string ApplicationVersionCreated = "Application version created successfully.";

    }
}
