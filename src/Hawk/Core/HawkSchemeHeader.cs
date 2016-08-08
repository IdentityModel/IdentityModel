using System;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Represents the HTTP authorization header in hawk scheme.
    /// </summary>
    internal class HawkSchemeHeader
    {
        /// <summary>
        /// Returns an AuthenticationResult object corresponding to the result of authentication done
        /// using the client supplied artifacts in the HTTP authorization header in hawk scheme.
        /// </summary>
        /// <param name="now">Current UNIX time in milliseconds.</param>
        /// <param name="request">Request object.</param>
        /// <param name="options">Hawk authentication options</param>
        /// <returns></returns>
        internal static async Task<AuthenticationResult> AuthenticateAsync(ulong now, IRequestMessage request, Options options)
        {
            ArtifactsContainer artifacts = null;
            Credential credential = null;

            if (request.HasValidHawkScheme())
            {
                if (ArtifactsContainer.TryParse(request.Authorization.Parameter, out artifacts))
                {
                    if (artifacts != null && artifacts.AreClientArtifactsValid)
                    {
                        string lastUsedBy = options.DetermineNonceReplayCallback(artifacts.Nonce);

                        if (String.IsNullOrEmpty(lastUsedBy)) // Not an old nonce, and hence not a replay.
                        {
                            credential = options.CredentialsCallback(artifacts.Id);
                            if (credential != null && credential.IsValid)
                            {
                                HawkEventSource.Log.Debug(
                                    String.Format("Algorithm={0} Key={1} ID={2}",
                                                        credential.Algorithm.ToString(),
                                                        Convert.ToBase64String(credential.Key),
                                                        credential.Id));

                                Tuple<string, string> hostAndPort = options.DetermineHostDetailsCallback(request);
                                var normalizedRequest = new NormalizedRequest(request, artifacts, hostAndPort.Item1, hostAndPort.Item2);
                                var crypto = new Cryptographer(normalizedRequest, artifacts, credential);

                                // Request body is needed only when payload hash is present in the request
                                string body = null;
                                if (artifacts.PayloadHash != null && artifacts.PayloadHash.Length > 0)
                                {
                                    body = await request.ReadBodyAsStringAsync();
                                }

                                if (crypto.IsSignatureValid(body, request.ContentType)) // MAC and hash checks
                                {
                                    if (IsTimestampFresh(now, artifacts, options))
                                    {
                                        // If you get this far, you are authentic. Welcome and thanks for flying Hawk!

                                        // Before returning the result, store nonce to detect replays.
                                        options.StoreNonceCallback(artifacts.Nonce, credential.Id, options.ClockSkewSeconds);

                                        return new AuthenticationResult()
                                        {
                                            IsAuthentic = true,
                                            Artifacts = artifacts,
                                            Credential = credential,
                                            ApplicationSpecificData = artifacts.ApplicationSpecificData
                                        };
                                    }
                                    else
                                    {
                                        // Authentic but for the timestamp freshness.
                                        // Give a chance to the client to correct the clocks skew.
                                        var timestamp = new NormalizedTimestamp(DateTime.UtcNow, credential, options.LocalTimeOffsetMillis);
                                        request.ChallengeParameter = timestamp.ToWwwAuthenticateHeaderParameter();
                                    }
                                }
                            }
                        }
                        else
                        {
                            HawkEventSource.Log.NonceReplay(artifacts.Nonce, lastUsedBy);
                        }
                    }
                }
            }

            return new AuthenticationResult() { IsAuthentic = false };
        }

        /// <summary>
        /// Returns true if the timestamp sent in by the client is fresh subject to the 
        /// maximum allowed skew and the adjustment offset.
        /// </summary>
        private static bool IsTimestampFresh(ulong now, ArtifactsContainer artifacts, Options options)
        {
            now = now + Convert.ToUInt64(options.LocalTimeOffsetMillis);

            ulong shelfLife = (Convert.ToUInt64(options.ClockSkewSeconds) * 1000);
            var age = Math.Abs((artifacts.Timestamp * 1000.0) - now);

            bool isFresh = (age <= shelfLife);

            if (!isFresh)
                HawkEventSource.Log.StaleTimestamp(age.ToString(), shelfLife.ToString());

            return isFresh;
        }
    }
}