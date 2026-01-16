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
        public const string ProgramActionNameAlreadyExists = "ProgramActionName already exists.";

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


        public const string SysBusinessActivityAlreadyExists = "Name already exists.";
        public const string SysBusinessActivityCreated = "BusinessActivity created sucessfully.";
    }
}
