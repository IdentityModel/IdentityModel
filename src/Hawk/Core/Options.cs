using System;
using System.Text.RegularExpressions;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Etw;
using Microsoft.Extensions.Caching.Memory;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Hawk authentication options.
    /// </summary>
    public class Options
    {
        public Options()
        {
            this.ClockSkewSeconds = 60;
            this.EnableServerAuthorization = true;
            this.DetermineHostDetailsCallback = DefaultBehavior.DetermineHostDetails;
            this.DetermineNonceReplayCallback = DefaultBehavior.GetLastUsedId;
            this.StoreNonceCallback = DefaultBehavior.StoreNonce;
        }

        /// <summary>
        /// Local time offset in milliseconds.
        /// </summary>
        public int LocalTimeOffsetMillis { get; set; }

        /// <summary>
        /// Skew allowed between the client and the server clocks in seconds. Default is 60 seconds.
        /// </summary>
        public int ClockSkewSeconds { get; set; }

        /// <summary>
        /// If true, the Server-Authorization header is sent in the response. Default is true.
        /// </summary>
        public bool EnableServerAuthorization { get; set; }

        /// <summary>
        /// Func delegate that returns Credential for the given user identifier.
        /// </summary>
        public Func<string, Credential> CredentialsCallback { get; set; }

        /// <summary>
        /// Func delegate that returns the normalized form of the response message to be used
        /// as application specific data ('ext' field) in the Server-Authorization response header.
        /// </summary>
        public Func<IResponseMessage, string> NormalizationCallback { get; set; }

        /// <summary>
        /// Func delegate that returns true if the specified normalized form of the request
        /// message matches the normalized form of the specified request message.
        /// </summary>
        public Func<IRequestMessage, string, bool> VerificationCallback { get; set; }

        /// <summary>
        /// Func delegate that returns true, if the response body must be hashed and included
        /// in the MAC ('mac' field) sent in the Server-Authorization response header.
        /// </summary>
        public Func<IRequestMessage, bool> ResponsePayloadHashabilityCallback { get; set; }

        /// <summary>
        /// Func delegate that returns the host name and port number.
        /// </summary>
        public Func<IRequestMessage, Tuple<string, string>> DetermineHostDetailsCallback { get; set; }

        /// <summary>
        /// Action delegate that stores the nonce from the request for a specific period to 
        /// detect replay of old requests, against the identifier.
        /// </summary>
        public Action<string, string, int> StoreNonceCallback { get; set; }

        /// <summary>
        /// Func delegate that returns the identifier used with an earlier request,
        /// if a nonce was replayed. Returns null, if nonce is fresh.
        /// By default, the nonce values from the requests are stored in the memory in-proc.
        /// If you use multiple servers or processes for load balancing and if the replay
        /// request goes to a server that did not service the request before, nonce will not
        ///  be rejected. In such cases, store the nonce as part of StoreNonceCallback into a
        ///  store common to all instances and check the nonce as part of this callback.
        /// </summary>
        public Func<string, string> DetermineNonceReplayCallback { get; set; }

        public class DefaultBehavior
        {
            static readonly object cacheLock = new object();
            static readonly Lazy<MemoryCache> s_DefaultCache = new Lazy<MemoryCache>(() => new MemoryCache(new MemoryCacheOptions())); 

            internal static Tuple<string, string> DetermineHostDetails(IRequestMessage request)
            {
                string host = request.Headers.FirstOrDefault("X-Forwarded-Host");

                HawkEventSource.Log.Debug("X-Forwarded-Host=" + (host ?? String.Empty));

                if (String.IsNullOrWhiteSpace(host))
                    host = request.Host;

                HawkEventSource.Log.Debug("Host=" + (host ?? String.Empty));

                if (String.IsNullOrWhiteSpace(host))
                    host = request.Uri.Host;

                string hostName = String.Empty;
                string port = String.Empty;

                string pattern = @"^(?:(?:\r\n)?\s)*((?:[^:]+)|(?:\[[^\]]+\]))(?::(\d+))?(?:(?:\r\n)?\s)*$";
                var match = Regex.Match(host, pattern);

                if (match.Success && match.Groups.Count == 3)
                {
                    hostName = match.Groups[1].Value;

                    if (!String.IsNullOrWhiteSpace(hostName))
                    {
                        port = match.Groups[2].Value;
                    }
                }

                if (String.IsNullOrWhiteSpace(port))
                {
                    port = request.Headers.FirstOrDefault("X-Forwarded-Port");

                    HawkEventSource.Log.Debug("X-Forwarded-Port=" + (port ?? String.Empty));
                }

                if (String.IsNullOrWhiteSpace(port))
                {
                    string scheme = request.Headers.FirstOrDefault("X-Forwarded-Proto");

                    HawkEventSource.Log.Debug("X-Forwarded-Proto=" + (scheme ?? String.Empty));

                    if (String.IsNullOrWhiteSpace(scheme))
                    {
                        scheme = request.Scheme;
                    }

                    port = "https".Equals(scheme, StringComparison.OrdinalIgnoreCase) ? "443" : "80";

                    HawkEventSource.Log.Debug("Port chosen based on HTTP scheme: " + port);
                }

                return new Tuple<string, string>(hostName, port);
            }

            internal static void StoreNonce(string nonce, string id, int lifeSeconds)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(new DateTimeOffset(DateTime.Now.AddSeconds(lifeSeconds)));
                lock (cacheLock)
                {
                    s_DefaultCache.Value.Set(nonce, id, options);
                }
            }

            internal static string GetLastUsedId(string nonce)
            {
                return s_DefaultCache.Value.Get(nonce) as string;
            }
        }
    }
}
