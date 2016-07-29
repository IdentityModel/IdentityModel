using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Client
{
    /// <summary>
    /// The counterpart of HawkServer in the client side that creates the HTTP Authorization header in hawk scheme
    /// and authenticates the server response by validating the Server-Authorization response header.
    /// HawkClient is for per-request use.
    /// </summary>
    public class HawkClient
    {
        private static readonly object myPrecious = new object();

        private readonly ClientOptions options = null;

        private ArtifactsContainer artifacts = null;
        private Cryptographer crypto = null;

        /// <summary>
        /// Authenticates the server response by reading the Server-Authorization header and creates 
        /// the the HTTP Authorization header in hawk scheme.
        /// </summary>
        /// <param name="options">Hawk authentication options</param>
        public HawkClient(ClientOptions options)
        {
            if (options == null || options.CredentialsCallback == null)
                throw new ArgumentNullException("Invalid Hawk authentication options. Credentials callback cannot be null.");

            this.options = options;
        }

        /// <summary>
        /// Added to current date time before computing the UNIX time. HawkClient can automatically
        /// adjust the value in this property based on the timestamp sent by the service in the 
        /// WWW-Authenticate header in an attempt to keep the server and the client clocks in sync.
        /// </summary>
        public static int CompensatorySeconds { get; private set; }

        /// <summary>
        /// Returns true, if the HMAC computed for the response payload matches the HMAC in the
        /// Server-Authorization response header. This method also sets the compensation field so 
        /// that the timestamp in the subsequent requests are adjusted to reduce the clock skew.
        /// </summary>
        public async Task<bool> AuthenticateAsync(IResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.Unauthorized &&
                    this.options.EnableResponseValidation &&
                        await this.IsResponseTamperedAsync(artifacts, crypto, response))
                return false;

            if (this.options.EnableAutoCompensationForClockSkew &&
                    this.IsTimestampResponseTampered(artifacts, response))
                return false;

            return true;
        }

        /// <summary>
        /// Creates the HTTP Authorization header in hawk scheme.
        /// The counterpart of the CreateServerAuthorization method in HawkServer.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <returns></returns>
        public async Task CreateClientAuthorizationAsync(IRequestMessage request)
        {
            HawkEventSource.Log.Debug(
                String.Format("HawkClient.CreateClientAuthorizationAsync for {0} {1}",
                                request.Method.ToString(),
                                request.Uri.ToString()));

            await CreateClientAuthorizationInternalAsync(request, DateTime.UtcNow);
        }

        /// <summary>
        /// Adds the bewit to the query string of the specified HttpRequestMessage object and
        /// returns the bewit string.
        /// </summary>
        public string CreateBewit(IRequestMessage request, int lifeSeconds)
        {
            HawkEventSource.Log.Debug(
                String.Format("HawkClient.CreateBewit for {0} {1}",
                                request.Method.ToString(),
                                request.Uri.ToString()));

            return CreateBewitInternal(request, DateTime.UtcNow, lifeSeconds);
        }

        /// <summary>
        /// Adds the bewit to the query string of the specified HttpRequestMessage object and 
        /// returns the bewit string.
        /// </summary>
        internal string CreateBewitInternal(IRequestMessage request, DateTime utcNow, int lifeSeconds)
        {
            string appData = null;
            if (options.NormalizationCallback != null)
                appData = options.NormalizationCallback(request);

            var bewit = new Bewit(request, options.CredentialsCallback(),
                                    utcNow, lifeSeconds, appData, options.LocalTimeOffsetMillis);
            string bewitString = bewit.ToBewitString();

            string parameter = String.Format("{0}={1}", HawkConstants.Bewit, bewitString);

            string queryString = request.Uri.Query;
            queryString = String.IsNullOrWhiteSpace(queryString) ? parameter : queryString.Substring(1) + "&" + parameter;
            request.QueryString = queryString;

            return bewitString;
        }

        /// <summary>
        /// Creates the HTTP Authorization header in hawk scheme.
        /// </summary>
        internal async Task CreateClientAuthorizationInternalAsync(IRequestMessage request, DateTime utcNow)
        {
            var credential = options.CredentialsCallback();
            this.artifacts = new ArtifactsContainer()
            {
                Id = credential.Id,
                Timestamp = utcNow.AddSeconds(HawkClient.CompensatorySeconds).ToUnixTime(),
                Nonce = NonceGenerator.Generate()
            };

            if (options.NormalizationCallback != null)
                this.artifacts.ApplicationSpecificData = options.NormalizationCallback(request);

            var normalizedRequest = new NormalizedRequest(request, this.artifacts);
            this.crypto = new Cryptographer(normalizedRequest, this.artifacts, credential);

            // Sign the request
            bool includePayloadHash = options.RequestPayloadHashabilityCallback != null &&
                                            options.RequestPayloadHashabilityCallback(request);

            string payload = includePayloadHash ? await request.ReadBodyAsStringAsync() : null;
            crypto.Sign(payload, request.ContentType);

            request.Authorization = new AuthenticationHeaderValue(HawkConstants.Scheme,
                                                this.artifacts.ToAuthorizationHeaderParameter());
        }

        /// <summary>
        /// Returns true if the server response HMAC cannot be validated, indicating possible tampering.
        /// </summary>
        private async Task<bool> IsResponseTamperedAsync(ArtifactsContainer artifacts, Cryptographer crypto,
                                                        IResponseMessage response)
        {
            if (response.Headers.ContainsKey(HawkConstants.ServerAuthorizationHeaderName))
            {
                string header = response.Headers[HawkConstants.ServerAuthorizationHeaderName].FirstOrDefault();

                if (!String.IsNullOrWhiteSpace(header) &&
                                    header.Substring(0, HawkConstants.Scheme.Length).ToLower() == HawkConstants.Scheme)
                {
                    ArtifactsContainer serverAuthorizationArtifacts;
                    if (ArtifactsContainer.TryParse(header.Substring(HawkConstants.Scheme.Length + " ".Length),
                                                                                    out serverAuthorizationArtifacts))
                    {
                        // To validate response, ext, hash, and mac in the request artifacts must be
                        // replaced with the ones from the server.
                        artifacts.ApplicationSpecificData = serverAuthorizationArtifacts.ApplicationSpecificData;
                        artifacts.PayloadHash = serverAuthorizationArtifacts.PayloadHash;
                        artifacts.Mac = serverAuthorizationArtifacts.Mac;

                        // Response body is needed only if payload hash is present in the server response.
                        string body = null;
                        if (artifacts.PayloadHash != null && artifacts.PayloadHash.Length > 0)
                        {
                            body = await response.ReadBodyAsStringAsync();
                        }

                        bool isValid = crypto.IsSignatureValid(body, response.ContentType, isServerAuthorization: true);

                        if (isValid)
                        {
                            string appSpecificData = serverAuthorizationArtifacts.ApplicationSpecificData;

                            isValid = options.VerificationCallback == null ||
                                                    options.VerificationCallback(response, appSpecificData);
                        }

                        return !isValid;
                    }
                }
            }

            return true; // Missing header means possible tampered response (to err on the side of caution).
        }

        /// <summary>
        /// Returns true, if there is a WWW-Authenticate header containing ts and tsm but mac
        /// computed for ts does not match tsm, indicating possible tampering. Otherwise, returns false.
        /// This method also sets the compensation field so that the timestamp in the subsequent requests
        /// are adjusted to reduce the clock skew.
        /// </summary>
        private bool IsTimestampResponseTampered(ArtifactsContainer artifacts, IResponseMessage response)
        {
            var wwwHeader = response.WwwAuthenticate;

            if (wwwHeader != null)
            {
                string parameter = wwwHeader.Parameter;

                ArtifactsContainer timestampArtifacts;
                if (!String.IsNullOrWhiteSpace(parameter) &&
                                ArtifactsContainer.TryParse(parameter, out timestampArtifacts))
                {
                    var ts = new NormalizedTimestamp(timestampArtifacts.Timestamp, options.CredentialsCallback(), options.LocalTimeOffsetMillis);

                    if (!ts.IsValid(timestampArtifacts.TimestampMac))
                        return true;

                    lock (myPrecious)
                        HawkClient.CompensatorySeconds = (int)(timestampArtifacts.Timestamp - DateTime.UtcNow.ToUnixTime());

                    HawkEventSource.Log.TimestampMismatch(HawkClient.CompensatorySeconds);
                }
            }

            return false;
        }
    }
}