namespace CleanArchitecture.Application.Common.Responses
{
    public static class ErrorCodes
    {
        public static class Identity
        {
            public const string UserCreationFailed = "IDENTITY_USER_CREATION_FAILED";

            public const string UserUpdateFailed = "IDENTITY_USER_UPDATE_FAILED";

            public const string UserDeletionFailed = "IDENTITY_USER_DELETION_FAILED";
        }


        public static class Student
        {
            public const string EmailAlreadyUsed = "STUDENT_EMAIL_ALREADY_USED";

            public const string PhoneAlreadyUsed = "STUDENT_PHONE_ALREADY_USED";

            public const string NotFound = "STUDENT_NOT_FOUND";
        }


        public static class Course
        {
            public const string NotFound = "COURSE_NOT_FOUND";

            public const string CapacityLessThanActiveEnrollments = "COURSE_CAPACITY_LESS_THAN_ACTIVE_ENROLLMENTS";

            public const string AlreadyExists = "COURSE_ALREADY_EXISTS";

            public const string HasActiveEnrollments = "COURSE_HAS_ACTIVE_ENROLLMENTS";

            public const string NameReservedByDeletedEntity = "COURSE_NAME_RESERVED_BY_DELETED_ENTITY";

            public const string NotActive = "COURSE_NOT_ACTIVE";
        }


        public static class Enrollment
        {
            public const string AlreadyEnrolled = "ENROLLMENT_ALREADY_EXISTS";

            public const string AlreadyCompleted = "ENROLLMENT_ALREADY_COMPLETED";

            public const string CourseFull = "COURSE_FULL";

            public const string NotEnrolled = "ENROLLMENT_NOT_ENROLLED";
        }


        public static class Authentication
        {
            public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";

            public const string EmailNotConfirmed = "AUTH_EMAIL_NOT_CONFIRMED";

            public const string AccountInactive = "AUTH_ACCOUNT_INACTIVE";

            public const string AccountLocked = "AUTH_ACCOUNT_LOCKED";

            public const string InvalidRefreshToken = "AUTH_INVALID_REFRESH_TOKEN";

            public const string RefreshTokenRevoked = "AUTH_REFRESH_TOKEN_REVOKED";

            public const string RefreshTokenExpired = "AUTH_REFRESH_TOKEN_EXPIRED";
        }


        public static class Account
        {
            public const string PasswordChangeFailed = "ACCOUNT_PASSWORD_CHANGE_FAILED";

            public const string EmailAlreadyUsed = "ACCOUNT_EMAIL_ALREADY_USED";

            public const string InvalidCurrentPassword = "ACCOUNT_INVALID_CURRENT_PASSWORD";

            public const string AccountLocked = "ACCOUNT_LOCKED";
            
            public const string ChangeEmailFailed = "ACCOUNT_CHANGE_EMAIL_FAILED";
        }


        public static class Common
        {
            public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";

            public const string ValidationFailed = "VALIDATION_FAILED";

            public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        }

    }
}
