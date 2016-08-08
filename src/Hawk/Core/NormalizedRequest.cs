using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Represents the normalized request, in the following format.
    /// hawk.1.header\n
    /// timestamp\n
    /// nonce\n
    /// HTTP method\n
    /// uri path and query string\n
    /// host name\n
    /// port\n
    /// payload hash\n
    /// application specific data\n
    /// </summary>
    internal class NormalizedRequest
    {
        private const string REQUEST_PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".header"; // hawk.1.header
        private const string BEWIT_PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".bewit"; // hawk.1.bewit
        private const string RESPONSE_PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".response"; // hawk.1.response

        private readonly ArtifactsContainer artifacts = null;

        private readonly string method = null;
        private readonly string path = null;
        private readonly string hostName = null;
        private readonly string port = null;

        internal NormalizedRequest(IRequestMessage request, ArtifactsContainer artifacts, string hostName = null, string port = null)
        {
            this.artifacts = artifacts;

            this.hostName = (hostName ?? request.Uri.Host).ToLower();
            this.port = port ?? request.Uri.Port.ToString();
            this.method = request.Method.Method.ToUpper();
            this.path = WebUtility.UrlDecode(request.Uri.AbsolutePath) + request.Uri.Query;
        }

        /// <summary>
        /// Set to true, if this instance is for a bewit.
        /// </summary>
        internal bool IsBewit { get; set; }

        /// <summary>
        /// Set to true, if this instance is for server authorization response.
        /// </summary>
        internal bool IsServerAuthorization { get; set; }

        /// <summary>
        /// Returns the normalized request string.
        /// </summary>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result
                .AppendNewLine(this.GetPreamble())
                .AppendNewLine(artifacts.Timestamp.ToString())
                .AppendNewLine(artifacts.Nonce)
                .AppendNewLine(this.method)
                .AppendNewLine(this.path)
                .AppendNewLine(this.hostName)
                .AppendNewLine(this.port)
                .AppendNewLine(artifacts.PayloadHash == null ? null : artifacts.PayloadHash.ToBase64String())
                .AppendNewLine(artifacts.ApplicationSpecificData);

            string normalizedRequest = result.ToString();

            HawkEventSource.Log.NormalizedRequest(normalizedRequest);

            return normalizedRequest;
        }

        /// <summary>
        /// Returns the normalized request bytes.
        /// </summary>
        internal byte[] ToBytes()
        {
            return this.ToString().ToBytesFromUtf8();
        }

        private string GetPreamble()
        {
            string preamble = REQUEST_PREAMBLE;
            if (this.IsBewit)
                preamble = BEWIT_PREAMBLE;
            else if (this.IsServerAuthorization)
                preamble = RESPONSE_PREAMBLE;

            return preamble;
        }
    }
}
