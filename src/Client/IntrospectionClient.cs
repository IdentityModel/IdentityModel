using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client library for the OAuth 2 introspection endpoint
    /// </summary>
    public class IntrospectionClient
    {
        private readonly Func<HttpMessageInvoker> _client;
        private readonly IntrospectionClientOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="options"></param>
        public IntrospectionClient(HttpMessageInvoker client, IntrospectionClientOptions options)
            : this(() => client, options)
        { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="client func"></param>
        /// <param name="options"></param>
        public IntrospectionClient(Func<HttpMessageInvoker> client, IntrospectionClientOptions options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? new IntrospectionClientOptions();
        }

        /// <summary>
        /// Sets request parameters from the options.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parameters">The parameters.</param>
        internal void ApplyRequestParameters(TokenIntrospectionRequest request, IDictionary<string, string> parameters)
        {
            request.Address = _options.Address;
            request.ClientId = _options.ClientId;
            request.ClientSecret = _options.ClientSecret;
            request.ClientAssertion = _options.ClientAssertion;
            request.ClientCredentialStyle = _options.ClientCredentialStyle;
            request.AuthorizationHeaderStyle = _options.AuthorizationHeaderStyle;
            request.Parameters = _options.Parameters;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// Introspects a token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenTypeHint"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TokenIntrospectionResponse> Introspect(string token, string tokenTypeHint = null, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new TokenIntrospectionRequest
            {
                Token = token,
                TokenTypeHint = tokenTypeHint
            };
            ApplyRequestParameters(request, parameters);

            return _client().IntrospectTokenAsync(request, cancellationToken);
        }
    }
}