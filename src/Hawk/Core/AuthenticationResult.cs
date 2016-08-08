
namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// The result of the authentication process.
    /// </summary>
    internal class AuthenticationResult
    {
        /// <summary>
        /// The Credential object corresponding to the identifier in the request
        /// </summary>
        internal Credential Credential { get; set; }

        /// <summary>
        /// The ArtifactsContainer object containing the client supplied artifacts such as timestamp, hash, mac, etc.
        /// </summary>
        internal ArtifactsContainer Artifacts { get; set; }

        /// <summary>
        /// True, if the authentication process is successful.
        /// </summary>
        internal bool IsAuthentic { get; set; }

        /// <summary>
        /// The application specific data sent by the client (if any).
        /// </summary>
        internal string ApplicationSpecificData { get; set; }
    }
}
