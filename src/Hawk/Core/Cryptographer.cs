using System;
using System.Text;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// The class responsible for validating the HMAC in the request (in case of service) and the response (in case of the client)
    /// and signing the request (in case of client) and response (in case of service) by creating an HMAC.
    /// </summary>
    internal class Cryptographer
    {
        private readonly ArtifactsContainer artifacts = null;
        private readonly Credential credential = null;
        private readonly NormalizedRequest normalizedRequest = null;
        private readonly Hasher hasher = null;

        internal Cryptographer(NormalizedRequest request, ArtifactsContainer artifacts, Credential credential)
        {
            this.normalizedRequest = request;
            this.artifacts = artifacts;
            this.credential = credential;
            this.hasher = new Hasher(credential.Algorithm);
        }

        /// <summary>
        /// Returns true, if the HMAC computed for the normalized request matches the HMAC 
        /// sent in by the client (ArtifactsContainer.Mac) and if a payload hash is present,
        /// it matches the hash computed for the normalized payload as well.
        /// </summary>
        internal bool IsSignatureValid(string body = null, string contentType = null, bool isServerAuthorization = false)
        {
            return this.IsMacValid(isServerAuthorization) && (this.IsHashNotPresent() || this.IsHashValid(body, contentType));
        }

        /// <summary>
        /// Creates the payload hash and the corresponding HMAC, and updates the artifacts object
        /// with these new values.
        /// </summary>
        internal void Sign(string body = null, string contentType = null)
        {
            byte[] responsePayloadHash = null;
            if (body != null && contentType != null)
            {
                var payload = new NormalizedPayload(body, contentType);
                byte[] data = payload.ToBytes();

                responsePayloadHash = hasher.ComputeHash(data);
            }

            artifacts.PayloadHash = responsePayloadHash;

            byte[] normalizedRequest = this.normalizedRequest.ToBytes();
            artifacts.Mac = hasher.ComputeHmac(normalizedRequest, credential.Key);
        }

        private bool IsMacValid(bool isServerAuthorization = false)
        {
            this.normalizedRequest.IsServerAuthorization = isServerAuthorization;
            byte[] data = this.normalizedRequest.ToBytes();
            // data, at this point has the hash coming in over the wire and hence mac computed is
            // based on the hash over the wire and not over the computed hash

            bool isMacValid = this.hasher.IsValidMac(data, credential.Key, artifacts.Mac);

            if (!isMacValid)
                HawkEventSource.Log.Debug(
                    String.Format("Invalid Mac {0} for data {1}",
                                        artifacts.Mac.ToBase64String(), Encoding.UTF8.GetString(data)));
            return isMacValid;
        }

        private bool IsHashNotPresent()
        {
            bool isHashAbsent = artifacts.PayloadHash == null || artifacts.PayloadHash.Length == 0;

            HawkEventSource.Log.Debug(isHashAbsent ? "Payload Hash absent" : "Payload Hash present");
            
            return isHashAbsent;
        }

        private bool IsHashValid(string body, string contentType)
        {
            var normalizedPayload = new NormalizedPayload(body, contentType);
            byte[] data = normalizedPayload.ToBytes();

            bool isHashValid = this.hasher.IsValidHash(data, artifacts.PayloadHash);

            if (!isHashValid)
                HawkEventSource.Log.Debug(
                    String.Format("Invalid payload hash {0} for data {1}",
                                    artifacts.PayloadHash.ToBase64String(), Encoding.UTF8.GetString(data)));

            return isHashValid;
        }
    }
}