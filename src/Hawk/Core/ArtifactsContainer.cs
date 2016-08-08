using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// CLR representation of the parameter data of Authorization, WWW-Authenticate, and Server-Authorization headers.
    /// </summary>
    internal class ArtifactsContainer
    {
        private const string VALUE_MATCH_PATTERN = @"^[ \w\!#\$%&'\(\)\*\+,\-\.\/\:;<\=>\?@\[\]\^`\{\|\}~]+$";
        private const string PARAMETER_MATCH_PATTERN = @"(\w+)=""([^""\\]*)""\s*(?:,\s*|$)";
        private const string SPECIFIC_PARAMETER_MATCH_PATTERN = @"({0})=""([^""\\]*)""\s*(?:,\s*|$)";

        private const string ID = "id";
        private const string TS = "ts";
        private const string NONCE = "nonce";
        private const string EXT = "ext";
        private const string MAC = "mac";
        private const string HASH = "hash";
        private const string TSM = "tsm";

        /// <summary>
        /// Hawk credentials identifier.
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        /// Timestamp (UNIX time).
        /// </summary>
        internal ulong Timestamp { get; set; }

        /// <summary>
        /// Client generated nonce.
        /// </summary>
        internal string Nonce { get; set; }

        /// <summary>
        /// Application specific data that the client or the service can send along.
        /// </summary>
        internal string ApplicationSpecificData { get; set; }

        /// <summary>
        /// Hash-based message authentication code of the normalized request or response, created respectively
        /// by the client or the service, using the shared secret key.
        /// </summary>
        internal byte[] Mac { get; set; }

        /// <summary>
        /// Hash of the normalized payload (request or response body). It is just a plain hash and not a 
        /// hash-based message authentication code.
        /// </summary>
        internal byte[] PayloadHash { get; set; }

        /// <summary>
        /// Hash-based message authentication code of the normalized timestamp, created using the shared secret key
        /// </summary>
        internal byte[] TimestampMac { get; set; }

        /// <summary>
        /// Returns true if the client supplied artifacts of identifier, nonce, and MAC are non-empty and 
        /// timestamp is greater than 0.
        /// </summary>
        internal bool AreClientArtifactsValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(this.Id) &&
                                !String.IsNullOrWhiteSpace(this.Nonce) &&
                                    (this.Mac != null && this.Mac.Length > 0) &&
                                        this.Timestamp > 0;
            }
        }


        /// <summary>
        /// Attempts to convert the passed in header parameter (string) into the CLR equivalent, which is an
        /// instance of ArtifactsContainer. The return value indicates whether the conversion succeeded.
        /// </summary>
        internal static bool TryParse(string headerParameter, out ArtifactsContainer container)
        {
            HawkEventSource.Log.Debug("Hawk Authorization: " + headerParameter);

            ArtifactsContainer result = new ArtifactsContainer();

            var keysToBeProcessed = new HashSet<string>() { ID, TS, NONCE, EXT, MAC, HASH, TSM };

            var replacedString = Regex.Replace(headerParameter, PARAMETER_MATCH_PATTERN, (Match match) =>
            {
                string key = match.Groups[1].Value.Trim();
                string value = match.Groups[2].Value.Trim();

                bool isValidValue = Regex.Match(value, VALUE_MATCH_PATTERN).Success;
                bool isValidKey = keysToBeProcessed.Any(k => k == key); // Key is neither duplicate nor bad

                if (isValidValue && isValidKey)
                {
                    switch (key)
                    {
                        case ID: result.Id = value; break;

                        case TS:
                            {
                                ulong timestamp;
                                if (UInt64.TryParse(value, out timestamp))
                                {
                                    result.Timestamp = timestamp;
                                    break;
                                }
                                else
                                    return value;
                            }

                        case NONCE: result.Nonce = value; break;
                        case EXT: result.ApplicationSpecificData = value; break;
                        case MAC: result.Mac = value.ToBytesFromBase64(); break;
                        case HASH: result.PayloadHash = value.ToBytesFromBase64(); break;
                        case TSM: result.TimestampMac = value.ToBytesFromBase64(); break;
                    }

                    keysToBeProcessed.Remove(key); // Processed

                    return String.Empty;
                }
                else
                    return value;
            });

            if (replacedString == String.Empty) // No more, no less -> valid parameter data
            {
                container = result;
                return true;
            }

            HawkEventSource.Log.UnparsedArtifact(replacedString);

            container = null;
            return false;
        }

        /// <summary>
        /// Returns true, if the passed in header parameter contains the hash field.
        /// </summary>
        internal static bool IsPayloadHashPresent(string headerParameter)
        {
            string pattern = String.Format(SPECIFIC_PARAMETER_MATCH_PATTERN, HASH);

            if (!String.IsNullOrWhiteSpace(headerParameter))
                if (Regex.IsMatch(headerParameter, pattern))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns the header parameter to be put into the HTTP Authorization header in hawk scheme.
        /// </summary>
        internal string ToAuthorizationHeaderParameter()
        {
            return this.ToHeaderParameter(true);
        }

        /// <summary>
        /// Returns the header parameter to be put into the Server-Authorization header.
        /// Intended to be used only by HawkServer, while creating the Server-Authorization header
        /// and hence the scope of this method is internal.
        /// </summary>
        internal string ToServerAuthorizationHeaderParameter()
        {
            return this.ToHeaderParameter(
                                includeClientArtifacts: false); // Do not need the client supplied id, ts, and nonce
        }

        /// <summary>
        /// Returns the serialized form of this object.
        /// </summary>
        /// <param name="includeClientArtifacts">If true, the client supplied artifacts of id, timestamp, and nonce are included.</param>
        /// <returns></returns>
        private string ToHeaderParameter(bool includeClientArtifacts)
        {
            char trailer = ',';
            StringBuilder result = new StringBuilder();

            if (includeClientArtifacts)
            {
                result
                    .AppendIfNotEmpty(ID, this.Id, trailer)
                    .AppendIfNotEmpty(TS, (this.Timestamp > 0 ? this.Timestamp.ToString() : String.Empty), trailer)
                    .AppendIfNotEmpty(NONCE, this.Nonce, trailer);
            }

            result
                .AppendIfNotEmpty(EXT, this.ApplicationSpecificData, trailer)
                .AppendIfNotEmpty(MAC, this.Mac == null ? null : this.Mac.ToBase64String(), trailer)
                .AppendIfNotEmpty(HASH, this.PayloadHash == null ? null : this.PayloadHash.ToBase64String(), trailer);

            return result.ToString()
                .Trim().Trim(trailer); // Remove the trailing trailer and space for the last pair
        }
    }
}
