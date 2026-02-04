    using System;
using System.Collections.Generic;
using System.Text;

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

        // New generic messages
        public const string PayloadRequired = "Payload is required.";
        public const string NotFound = "Not found.";
        public const string PostOfficesAreAlreadyLinked = "Some post offices are already linked.Please refresh again.";

        public const string ActionsAreAlreadyLinked = "Some actions are already linked.Please refresh again.";
        public const string Updated = "Updated.";
        public const string ActionsNotFound = "Actions Not found.";
        public const string PostOfficesNotFound = "Post offices Not found.";
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

        public const string NoActivePrograms =
       "No active  programs available.";

        public const string BusinessActivitiesRetrieved =
            "Activity activities retrieved successfully.";

        public const string PlansRetrieved =
               "Plans retrieved successfully.";


        public const string PlacesRetrieved =
       "Places retrieved successfully.";


        public const string ApplicationRetrieved =
            "Platform retrieve sucessfully.";

        public const string PlanRetrieved =
           "Plan retrieve sucessfully.";

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
        public const string RoleRetrieveSucessfully = "Role retrieve sucessfully.";
        public const string RoleUpdatedSucessfully = "Role updated sucessfully.";
        public const string RoleDeleteSucessfully = "Role deleted sucessfully.";
        public const string RoleRestoreSucessfully = "Role restored sucessfully.";
        public const string RoleNameAlreadyExists = "Role name already exists.";
        public const string RoleNameExistsInTrash = "This role name exists in trash. Restore it to use again.";
        public const string RoleNameDefault = "This is a system default role and cannot be created..";
        public const string RoleAlreadyDeleted = "Role already deleted.";
        public const string RoleAlreadyRestored = "Role already restored.";
        public const string RolesNotFound = "Roles Not found .";
        public const string CountryRetrieveSucessfully = "Country retrieve sucessfully.";
        public const string DivisionOneRetrieveSucessfully = "Division one retrieve sucessfully.";
        public const string DivisionTwoRetrieveSucessfully = "Division two retrieve sucessfully.";
        public const string DivisionThreeRetrieveSucessfully = "Division three retrieve sucessfully.";

        public const string PostOfficeRestoreSucessfully = "Post office restored sucessfully.";
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

        // Country / Division messages
        public const string CountryRequired = "Country is required.";
        public const string CountryNotFound = "Selected country not found.";
        public const string DivisionOneNotFound = "Selected division one not found for the country.";
        public const string DivisionTwoNotFound = "Selected division two not found for the division one.";
        public const string DivisionThreeNotFound = "Selected division three not found for the division two.";
        public const string DivisionOneRequiredForDivisionTwo = "Division one is required when supplying division two.";
        public const string DivisionTwoRequiredForDivisionThree = "Division two is required when supplying division three.";

    }
}
