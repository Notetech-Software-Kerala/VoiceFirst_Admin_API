using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Constants
{
    public class Messages
    {
        // General
        public const string Success = "Success.";
        public const string Failed = "Failed.";
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
        public const string NotFound = "Not found.";
        public const string PostOfficesAreAlreadyLinked = "Some post offices are already linked.Please refresh again.";
        public const string ZipCodesAreAlreadyLinked = "Some Zip codes  are already linked.Please refresh again.";
        public const string ZipCodesAlreadyLinkedWithPlace = "These zip codes are already linked with this place.";

        public const string ActionsAreAlreadyLinked = "Some actions are already linked.Please refresh again.";
        public const string Updated = "Updated.";
        public const string ActionsNotFound = "Actions Not found.";
        public const string PostOfficesNotFound = "Post offices Not found.";
        public const string ZipCodesNotFound = "ZipCodes Not found.";
        public const string MobileCountryCodeNotFound = "Mobile country code Not found.";
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
        public const string ForgotPasswordLimitExceeded = "You have exceeded the maximum number of password reset requests for today. Please try again tomorrow.";
        public const string OtpAttemptsExceeded = "Too many invalid OTP attempts. Please request a new OTP.";
        public const string ForgotPasswordCooldown = "Please wait before requesting another OTP.";

        // Login / Logout
        public const string LogoutSuccess = "Logged out successfully.";
        public const string AccountLockedOut = "Account is locked due to too many failed login attempts. Please try again after {0} minutes.";
        public const string InvalidCredentialsWithAttempts = "Invalid credentials. {0} attempt(s) remaining before lockout.";
        public const string AccountInactive = "Your account is inactive. Please contact support.";
        public const string AccountDeleted = "Your account has been deleted. Please contact support.";
        public const string DeviceInfoRequired = "Device information is required.";

        // Change Password
        public const string ChangePasswordSuccess = "Password changed successfully. Please login again on other devices.";
        public const string OldPasswordIncorrect = "Current password is incorrect.";
        public const string NewPasswordSameAsOld = "New password cannot be the same as the current password.";
        public const string ChangePasswordFailed = "Password change failed. Please try again.";
 


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


        public const string ApplicationRetrieved =
            "Platform retrieve sucessfully.";

        public const string PlanRetrieved =
           "Plan retrieve sucessfully.";

        public const string PlanUpdated =
           "Plan updated sucessfully.";

        // ===== SYS Program  =====
        public const string InvalidActionLinksForApplication =
            "Invalid ActionLinks For selected Application.";
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


        public const string ZipCodesNotAvaliable =
       "Some selected zipCodes are no longer available." + BaseRefreshMessage;

        public const string MobileCountryCodeNotAvailable =
      "Some selected  country code  are no longer available." + BaseRefreshMessage;

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
        public const string MenuNameAlreadyExists = "Menu name already exists.";
        public const string MenuMasterAlreadyExistsInWeb = "Menu master already exists in web.";
        public const string MenuMasterAlreadyExistsInApp = "Menu master already exists in app.";
        public const string MenuNameExistsInTrash = "This menu name exists in trash. Restore it to use again.";
        public const string WebMenuExistsInTrash = "This web menu  exists in trash. Restore it to use again.";
        public const string AppMenuExistsInTrash = "This app menu  exists in trash. Restore it to use again.";
        public const string ParentMenuNotFound = "Parent menu not found..";
        public const string CannotAddOrUpdate = "Cannot add/reorder children under a parent that has a route.";

        // Country / Division messages
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
        public const string ForgotPasswordEmailSent = "If this email is registered, a password-reset OTP has been sent.";
        public const string ResetPasswordSuccess = "Password has been reset successfully.";
        public const string InvalidOrExpiredToken = "The OTP is invalid or has expired.";
        public const string PasswordResetFailed = "Password reset failed. Please try again.";

    }
}
