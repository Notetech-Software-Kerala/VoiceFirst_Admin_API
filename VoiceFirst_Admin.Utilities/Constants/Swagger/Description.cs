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


        // Success / operation
        public const string ROLE_CREATED = "Creates a new role with the provided details.";
        public const string ROLE_FAILD = "Faild to create a new role with the provided details";
        public const string CONFLICT_409 = "Conflict. Role already exists or the request violates a uniqueness constraint.";




    }
}
