
namespace Thinktecture.IdentityModel.Hawk.Core.Helpers
{
    /// <summary>
    /// Global constants
    /// </summary>
    public static class HawkConstants
    {
        /// <summary>"hawk"</summary>
        public const string Scheme = "hawk";
        
        /// <summary>"1"</summary>
        public const string Version = "1";

        /// <summary>"Server-Authorization"</summary>
        public const string ServerAuthorizationHeaderName = "Server-Authorization";

        /// <summary>"WWW-Authenticate"</summary>
        public const string WwwAuthenticateHeaderName = "WWW-Authenticate";

		/// <summary>"Content-Type"</summary>
        public const string ContentTypeHeaderName = "Content-Type";

        /// <summary>"Authorization"</summary>
        public const string AuthorizationHeaderName = "Authorization";

        /// <summary>"bewit"</summary>
        public const string Bewit = "bewit";
    }

    /// <summary>
    /// The hashing algorithms currently supported by this implementation.
    /// </summary>
    public enum SupportedAlgorithms
    {
        SHA1,
        SHA256
    }
}
