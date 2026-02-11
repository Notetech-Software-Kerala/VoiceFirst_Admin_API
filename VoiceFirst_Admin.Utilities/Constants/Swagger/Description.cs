using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Constants.Swagger
{
    public class Description
    {
        // Basic error descriptions (Swagger response descriptions)
        public const string BADREQUEST_400 = "Invalid request payload. Required fields are missing or validation failed.";
        
        public const string UNAUTHORIZED_401 = "Unauthorized. A valid access token is required.";
        public const string FORBIDDEN_403 = "Forbidden. You do not have permission to perform this action.";
        public const string NOTFOUND_404 = "Not found. The requested resource does not exist.";
        public const string SERVERERROR_500 = "Internal server error. An unexpected error occurred on the server.";
        public const string UNPROCESSABLE_422 = "Unprocessable entity. Validation passed but the resource exists in a recoverable (deleted) state.";


        // Success / operation (role)
        public const string ROLE_CREATED = "Creates a new role with the provided details.";
        public const string ROLE_UPDATED = "Update role with the provided details.";
        public const string ROLE_FAILD = "Faild to create a new role with the provided details";
        public const string CONFLICT_409 = "Conflict. Resource already exists or the request violates a uniqueness constraint.";
        public const string CONFLICT_WITH_DELETED_422 = "This  name exists in DELETED LIST. Restore it to use again the application.";
        public const string SYSTEM_ROLE_403 = "This is a system default role name and you cannot be created ..";

        // Success / operation (business activity)
        public const string ACTIVITY_CREATED = "Creates a new  activity.";
        public const string ACTIVITY_RETRIEVED = "Returns the requested  activity.";
        public const string ACTIVITIES_RETRIEVED = "Returns a list of  activities.";
        public const string ACTIVITY_UPDATED = "Updates the specified  activity.";
        public const string ACTIVITY_RECOVERED = "Recovers a previously deleted  activity.";
        public const string ACTIVITY_DELETED = "Deletes the specified  activity.";
        public const string ACTIVITY_ALREADY_RECOVERED_409 = "Conflict. The  activity is already recovered .";
        public const string ACTIVITY_ALREADY_DELETED_409 = "Conflict. The  activity is already deleted.";
        // Success / operation (user/employee)
        public const string USER_CREATED = "Creates a new employee user.";
        public const string USER_RETRIEVED = "Returns the requested employee user.";
        public const string USERS_RETRIEVED = "Returns a list of employee users.";
        public const string USER_UPDATED = "Updates the specified employee user.";
        public const string USER_RECOVERED = "Recovers a previously deleted employee user.";
        public const string USER_DELETED = "Deletes the specified employee user.";
        public const string USER_ALREADY_RECOVERED_409 = "Conflict. The employee user is already recovered.";
        public const string USER_ALREADY_DELETED_409 = "Conflict. The employee user is already deleted.";

        // Success / operation (program)
        public const string PROGRAM_CREATED = "Creates a new program.";
        public const string PROGRAM_RETRIEVED = "Returns the requested program.";
        public const string PROGRAMS_RETRIEVED = "Returns a list of programs.";
        public const string PROGRAM_UPDATED = "Updates the specified program.";
        public const string PROGRAM_RECOVERED = "Recovers a previously deleted program.";
        public const string PROGRAM_DELETED = "Deletes the specified program.";

        // Success / operation (plan)
        public const string PLAN_CREATED = "Creates a new plan.";
        public const string PLAN_RETRIEVED = "Returns the requested plan.";
        public const string PLANS_RETRIEVED = "Returns a list of plans.";
        public const string PLAN_UPDATED = "Updates the specified plan.";
        public const string PLAN_RECOVERED = "Recovers a previously deleted plan.";
        public const string PLAN_DELETED = "Deletes the specified plan.";

        // Success / operation (place)
        public const string PLACE_CREATED = "Creates a new place.";
        public const string PLACE_RETRIEVED = "Returns the requested place.";
        public const string PLACES_RETRIEVED = "Returns a list of places.";
        public const string PLACE_UPDATED = "Updates the specified place.";
        public const string PLACE_RECOVERED = "Recovers a previously deleted place.";
        public const string PLACE_DELETED = "Deletes the specified place.";




    }
}
