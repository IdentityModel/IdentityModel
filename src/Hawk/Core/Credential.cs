using System;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Hawk credential
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Identifier that can be used by a client to uniquely identify an entity (user).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Shared symmetric key that is exchanged out-of-band between the service and the client.
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Owner of the credential. If the owner is a human user, user name can be stored here.
        /// Else, the application name.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The hashing algorithm that the client and the service have agreed to use to create the 
        /// payload hash as well as HMAC of the request and timestamp.
        /// </summary>
        public SupportedAlgorithms Algorithm { get; set; }

        /// <summary>
        /// Returns true, if the identifier is not empty, the shared secret key is not empty, and
        /// the hashing algorithm is one of the algorithms defined in the SupportedAlgorithms enum.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isIdValid = !String.IsNullOrWhiteSpace(this.Id);
                bool isKeyValid = this.Key != null;
                bool isAlogorithmValid = Enum.IsDefined(typeof(SupportedAlgorithms), this.Algorithm);

                return isIdValid && isKeyValid && isAlogorithmValid;
            }
        }
    }
}
