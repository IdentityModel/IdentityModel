using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net.Http;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Represents the query parameter bewit used by Hawk for granting temporary access.
    /// </summary>
    internal class Bewit
    {
        private readonly Credential credential = null;
        private readonly IRequestMessage request = null;
        private readonly DateTime utcNow = DateTime.UtcNow;
        private readonly int lifeSeconds = 60;
        private readonly string applicationSpecificData = String.Empty;
        private readonly int localOffset = 0;

        /// <summary>
        /// Represents the query parameter bewit used by Hawk for granting temporary access.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="credential">Hawk credential to use for creating and validating bewit.</param>
        /// <param name="utcNow">Current date and time in UTC.</param>
        /// <param name="lifeSeconds">Bewit life time (time to live in seconds).</param>
        /// <param name="applicationSpecificData">Application specific data to be sent in the bewit</param>
        /// <param name="localOffset">Local offset in milliseconds.</param>
        internal Bewit(IRequestMessage request, Credential credential,
                            DateTime utcNow, int lifeSeconds, string applicationSpecificData, int localOffset = 0)
        {
            this.credential = credential;
            this.request = request;
            this.utcNow = utcNow;
            this.lifeSeconds = lifeSeconds;
            this.applicationSpecificData = applicationSpecificData;
            this.localOffset = localOffset;
        }

        /// <summary>
        /// Returns the string representation of the bewit, which is a base64 URL encoded string of format
        /// id\exp\mac\ext, where id is the user identifier, exp is the UNIX time until which bewit is
        /// valid, mac is the HMAC of the bewit to protect integrity, and ext is the application specific data.
        /// </summary>
        public string ToBewitString()
        {
            if (request.Method != HttpMethod.Get) // Not supporting HEAD
                throw new InvalidOperationException("Bewit not allowed for methods other than GET");

            ulong now = utcNow.ToUnixTime() + Convert.ToUInt64(this.localOffset);

            var artifacts = new ArtifactsContainer()
            {
                Id = credential.Id,
                Timestamp = now + (ulong)lifeSeconds,
                Nonce = String.Empty,
                ApplicationSpecificData = this.applicationSpecificData ?? String.Empty
            };

            var normalizedRequest = new NormalizedRequest(request, artifacts) { IsBewit = true };
            var crypto = new Cryptographer(normalizedRequest, artifacts, credential);

            // Sign the request
            crypto.Sign(); // Bewit is for GET and GET must have no request body

            // bewit: id\exp\mac\ext
            string bewit = String.Format(@"{0}\{1}\{2}\{3}",
                                credential.Id,
                                artifacts.Timestamp,
                                artifacts.Mac.ToBase64String(),
                                artifacts.ApplicationSpecificData);

            return bewit.ToBytesFromUtf8().ToBase64UrlString();
        }

        /// <summary>
        /// Returns true, if a query string parameter with a name of 'bewit' exists and that there
        /// is no HTTP Authorization header present in the request. Also, returns the value of the
        /// bewit parameter from the query string.
        /// </summary>
        internal static bool TryGetBewit(IRequestMessage request, out string bewit)
        {
            bewit = QueryHelpers.ParseQuery(request.Uri.Query)[HawkConstants.Bewit];
            
            return !String.IsNullOrWhiteSpace(bewit) &&
                        (request.Authorization == null);
        }

        /// <summary>
        /// Returns an AuthenticationResult object corresponding to the result of authentication done
        /// using the client supplied artifacts in the bewit query string parameter.
        /// </summary>
        /// <param name="bewit">Value of the query string parameter with the name of 'bewit'.</param>
        /// <param name="now">Date and time in UTC to be used as the base for computing bewit life.</param>
        /// <param name="request">Request object.</param>
        /// <param name="options">Hawk authentication options</param>
        internal static AuthenticationResult Authenticate(string bewit, ulong now, IRequestMessage request, Options options)
        {
            if (!String.IsNullOrWhiteSpace(bewit))
            {
                if (request.Method == HttpMethod.Get)
                {
                    if (options != null && options.CredentialsCallback != null)
                    {
                        var parts = bewit.ToUtf8StringFromBase64Url().Split('\\');

                        if (parts.Length == 4)
                        {
                            ulong timestamp = 0;
                            if (UInt64.TryParse(parts[1], out timestamp) && timestamp * 1000 > now)
                            {
                                string id = parts[0];
                                string mac = parts[2];
                                string ext = parts[3];

                                if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(mac))
                                {
                                    RemoveBewitFromUri(request);

                                    Credential credential = options.CredentialsCallback(id);
                                    if (credential != null && credential.IsValid)
                                    {
                                        var artifacts = new ArtifactsContainer()
                                        {
                                            Id = id,
                                            Nonce = String.Empty,
                                            Timestamp = timestamp,
                                            Mac = mac.ToBytesFromBase64(),
                                            ApplicationSpecificData = ext ?? String.Empty
                                        };

                                        var normalizedRequest = new NormalizedRequest(request, artifacts) { IsBewit = true };
                                        var crypto = new Cryptographer(normalizedRequest, artifacts, credential);

                                        if (crypto.IsSignatureValid()) // Bewit is for GET and GET must have no request body
                                        {
                                            return new AuthenticationResult()
                                            {
                                                IsAuthentic = true,
                                                Credential = credential,
                                                Artifacts = artifacts,
                                                ApplicationSpecificData = ext
                                            };
                                        }   
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new AuthenticationResult() { IsAuthentic = false };
        }

        /// <summary>
        /// Removes the bewit parameter from the URI of the specified IRequestMessage object.
        /// </summary>
        private static void RemoveBewitFromUri(IRequestMessage request)
        {
            string query = request.Uri.Query;
            string bewit = QueryHelpers.ParseQuery(request.Uri.Query)[HawkConstants.Bewit];

            query = query.Replace(HawkConstants.Bewit + "=" + bewit, String.Empty)
                            .Replace("&&", "&")
                            .Replace("?&", "?")
                            .Trim('&').Trim('?');

            request.QueryString = query;
        }
    }
}
