using System;
using System.Linq;
using System.Text;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Normalized representation of the timestamp data to be sent in the WWW-Authenticate header,
    /// when authentication failed on account of stale timestamp sent in by the client.
    /// hawk.1.ts\n
    /// timestamp\n
    /// </summary>
    internal class NormalizedTimestamp
    {
        private const string PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".ts"; // hawk.1.ts
        private const string TS = "ts";
        private const string TSM = "tsm";

        private readonly ulong unixTimeMillis;
        private readonly Credential credential;

        private readonly double fresh;

        internal NormalizedTimestamp(ulong unixTime, Credential credential, int localOffset = 0)
        {
            this.unixTimeMillis = unixTime * 1000;
            this.credential = credential;

            fresh = Math.Floor((this.unixTimeMillis + Convert.ToUInt64(localOffset)) / 1000.0);
        }

        internal NormalizedTimestamp(DateTime utcNow, Credential credential, int localOffset = 0) :
            this(utcNow.ToUnixTime(), credential, localOffset) { }

        /// <summary>
        /// Returns the normalized representation of the timestamp data.
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendNewLine(PREAMBLE)
                .AppendNewLine(fresh.ToString());

            string normalizedTimestamp = builder.ToString();

            HawkEventSource.Log.NormalizedTimestamp(normalizedTimestamp);

            return normalizedTimestamp;
        }

        /// <summary>
        /// Returns true, if the HMAC calculated for the normalized representation of the timestamp data that
        /// this instance represents matches the passed in HMAC.
        /// </summary>
        internal bool IsValid(byte[] hmacToValidateAgainst)
        {
            Hasher hasher = new Hasher(credential.Algorithm);
            byte[] computedMac = hasher.ComputeHmac(this.ToString().ToBytesFromUtf8(), credential.Key);

            // Okay not to use the constant-time comparison, since the timestamp
            // HMAC validation is done in the client side
            return computedMac.SequenceEqual(hmacToValidateAgainst);
        }

        /// <summary>
        /// Returns the header parameter to be put into the HTTP WWW-Authenticate header. The field ts has the timestamp
        /// in UNIX time corresponding to the server clock and the field tsm is the MAC calculated for the normalized
        /// timestamp data using the shared symmetric key and the algorithm agreed upon.
        /// </summary>
        /// <returns></returns>
        internal string ToWwwAuthenticateHeaderParameter()
        {
            Hasher hasher = new Hasher(credential.Algorithm);

            byte[] data = this.ToString().ToBytesFromUtf8();

            string tsm = hasher.ComputeHmac(data, credential.Key).ToBase64String();

            char trailer = ',';

            StringBuilder result = new StringBuilder();
            result.AppendIfNotEmpty(TS, fresh.ToString(), trailer)
                .AppendIfNotEmpty(TSM, tsm, trailer);

            return result.ToString().Trim().Trim(trailer);
        }
    }
}
