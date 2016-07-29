using System;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;

namespace Thinktecture.IdentityModel.Hawk.Client
{
    public class ClientOptions
    {
        public ClientOptions()
        {
            this.EnableResponseValidation = true;
            this.EnableAutoCompensationForClockSkew = true;
        }

        /// <summary>
        /// Local time offset in milliseconds.
        /// </summary>
        public int LocalTimeOffsetMillis { get; set; }

        /// <summary>
        /// Set this to true for the server response to be validated using the artifacts in Server-Authorization header.
        /// </summary>
        public bool EnableResponseValidation { get; set; }

        /// <summary>
        /// Set this true for the skew between the client and the server clocks to be automatically compensated.
        /// </summary>
        public bool EnableAutoCompensationForClockSkew { get; set; }

        /// <summary>
        /// Func delegate that returns the Credential.
        /// </summary>
        public Func<Credential> CredentialsCallback { get; set; }

        /// <summary>
        /// Func delegate that returns the normalized form of the request message to be used
        /// as application specific data ('ext' field) in the Authorization request header.
        /// </summary>
        public Func<IRequestMessage, string> NormalizationCallback { get; set; }

        /// <summary>
        /// Func delegate that returns true if the specified normalized form of the response
        /// message matches the normalized form of the specified response message.
        /// </summary>
        public Func<IResponseMessage, string, bool> VerificationCallback { get; set; }

        /// <summary>
        /// Func delegate that returns true, if the request body must be hashed and included
        /// in the MAC ('mac' field) sent in the Authorization request header.
        /// </summary>
        public Func<IRequestMessage, bool> RequestPayloadHashabilityCallback { get; set; }
	}
}
