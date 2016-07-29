using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Authenticates the incoming request based on the Authorize request header or bewit query string parameter
    /// and sets the Server-Authorization or the WWW-Authenticate response header. HawkServer is for per-request use.
    /// </summary>
    public class HawkServer
    {
        private readonly IRequestMessage request = null;
        private readonly Options options = null;

        private ulong now = 0;
        private AuthenticationResult result = null;

        internal bool IsBewitRequest { get; set; }

        /// <summary>
        /// Authenticates the incoming request based on the Authorize request header or bewit query string parameter
        /// </summary>
        /// <param name="request">The request object to be authenticated</param>
        /// <param name="options">Hawk authentication options</param>
        public HawkServer(IRequestMessage request, Options options)
        {
            now = DateTime.UtcNow.ToUnixTimeMillis(); // Record time before doing anything else

            if (options == null || options.CredentialsCallback == null)
                throw new ArgumentNullException("Invalid Hawk authentication options. Credentials callback cannot be null.");

            this.request = request;
            this.options = options;
        }

        /// <summary>
        /// Returns a ClaimsPrincipal object with the NameIdentifier and Name claims, if the request can be
        /// successfully authenticated based on query string parameter bewit or HTTP Authorization header (hawk scheme).
        /// </summary>
        public async Task<ClaimsPrincipal> AuthenticateAsync()
        {
            HawkEventSource.Log.Debug(
                String.Format("Begin HawkServer.AuthenticateAsync for {0} {1}",
                                request.Method.ToString(),
                                request.Uri.ToString()));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, String.Empty) }));

            string bewit;
            bool isBewit = Bewit.TryGetBewit(this.request, out bewit);

            this.result = isBewit ?
                                Bewit.Authenticate(bewit, now, request, options) :
                                    await HawkSchemeHeader.AuthenticateAsync(now, request, options);

            if (result.IsAuthentic)
            {
                HawkEventSource.Log.Debug("Authentication Successful");

                // At this point, authentication is successful but make sure the request parts match what is in the
                // application specific data 'ext' parameter by invoking the callback passing in the request object and 'ext'.
                // The application specific data is considered verified, if the callback is not set or it returns true.
                bool isAppSpecificDataVerified = options.VerificationCallback == null ||
                                                    options.VerificationCallback(request, result.ApplicationSpecificData);

                if (isAppSpecificDataVerified)
                {
                    // Set the flag so that Server-Authorization header is not sent for bewit requests.
                    this.IsBewitRequest = isBewit;

                    var idClaim = new Claim(ClaimTypes.NameIdentifier, result.Credential.Id);
                    var nameClaim = new Claim(ClaimTypes.Name, result.Credential.User);

                    var identity = new ClaimsIdentity(new[] { idClaim, nameClaim }, HawkConstants.Scheme);

                    principal = new ClaimsPrincipal(identity);
                }
                else
                    HawkEventSource.Log.Debug("Invalid Application Specific Data, though authentication is successful");
            }

            HawkEventSource.Log.Debug("End HawkServer.AuthenticateAsync");

            return principal;
        }

        /// <summary>
        /// Returns the name of the response header (WWW-Authenticate or Server-Authorization) and the corresponding
        /// value, respectively for an unauthorized and a successful request.
        /// </summary>
        public async Task<Tuple<string, string>> CreateServerAuthorizationAsync(IResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                string challenge = String.Format(" {0}", request.ChallengeParameter ?? String.Empty);
                string headerValue = HawkConstants.Scheme + challenge.TrimEnd(' ');

                return new Tuple<string, string>(HawkConstants.WwwAuthenticateHeaderName, headerValue);
            }
            else
            {
                // No Server-Authorization header for the following:
                // (1) There is no result or failed authentication.
                // (2) The credential is a bewit.
                // (3) The server is configured to not send the header.
                bool createHeader = this.result != null
                                        && this.result.IsAuthentic
                                            && (!this.IsBewitRequest)
                                                && options.EnableServerAuthorization;

                if (createHeader)
                {
                    if (options.NormalizationCallback != null)
                        this.result.Artifacts.ApplicationSpecificData = options.NormalizationCallback(response);

                    // Sign the response
                    Tuple<string, string> hostAndPort = options.DetermineHostDetailsCallback(request);
                    var normalizedRequest = new NormalizedRequest(request, this.result.Artifacts, hostAndPort.Item1, hostAndPort.Item2)
                    {
                        IsServerAuthorization = true
                    };
                    var crypto = new Cryptographer(normalizedRequest, this.result.Artifacts, this.result.Credential);

                    // Response body is needed only if payload hash must be included in the response MAC.
                    string body = null;
                    if (options.ResponsePayloadHashabilityCallback != null &&
                            options.ResponsePayloadHashabilityCallback(this.request))
                    {
                        body = await response.ReadBodyAsStringAsync();
                    }

                    crypto.Sign(body, response.ContentType);

                    string authorization = this.result.Artifacts.ToServerAuthorizationHeaderParameter();

                    if (!String.IsNullOrWhiteSpace(authorization))
                    {
                        return new Tuple<string, string>(HawkConstants.ServerAuthorizationHeaderName,
                            String.Format("{0} {1}", HawkConstants.Scheme, authorization));
                    }
                }
            }

            return null;
        }
    }
}