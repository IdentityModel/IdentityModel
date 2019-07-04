using System;

namespace IdentityModel.Client
{
    public struct AuthorityValidationResult
    {
        public static readonly AuthorityValidationResult SuccessResult = new AuthorityValidationResult(true, null);

        public string ErrorMessage { get; }

        public bool Success { get; }

        private AuthorityValidationResult(bool success, string message)
        {
            if (!success && string.IsNullOrEmpty(message))
                throw new ArgumentException("A message must be provided if success=false.", nameof(message));

            ErrorMessage = message;
            Success = success;
        }

        public static AuthorityValidationResult CreateError(string message)
        {
            return new AuthorityValidationResult(false, message);
        }

        public override string ToString()
        {
            return Success ? "success" : ErrorMessage;
        }
    }
}
