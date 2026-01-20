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
        public const string Nil = "NIL.";

        // New generic messages
        public const string PayloadRequired = "Payload is required.";
        public const string NotFound = "Not found.";
        public const string Updated = "Updated.";

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
            "Business activity name already exists.";

        public const string BusinessActivityAlreadyExistsRecoverable =
            "Business activity already exists but was deleted. You can recover it.";

        public const string BusinessActivityCreated =
            "Business activity created successfully.";

        public const string BusinessActivityRecovered =
            "Business activity recovered successfully.";

        public const string BusinessActivityNotFound =
            "Business activity not found.";
        public const string BusinessActivityDeleted =
            "Business activity deleted sucessfully.";

        public const string BusinessActivityRetrieved = 
            "Business activity retrieve sucessfully.";

        public const string BusinessActivityUpdated = 
            "Business activity updated sucessfully.";


        public const string ProgramActionRestoreSucessfully = "Program action restored sucessfully.";
        public const string ProgramActionDeleteSucessfully = "Program action deleted sucessfully.";
        public const string ProgramActionAlreadyDeleted = "Program action already deleted.";
        public const string ProgramActionAlreadyRestored = "Program action already restored.";
        public const string ProgramActionCreated = "Program action created sucessfully.";
        public const string ProgramActionCreatedRetrieveSucessfully = "Program action retrieve sucessfully.";
        public const string ProgramActionCreatedUpdatedSucessfully = "Program action updated sucessfully.";

    }
}
