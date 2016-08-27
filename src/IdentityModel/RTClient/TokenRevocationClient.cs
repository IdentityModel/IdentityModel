// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using IdentityModel.Client;

namespace Windows.Web.Http
{
    public class TokenRevocationClient
    {
        protected HttpClient _client;
        private readonly Uri _endpoint;

        public AuthenticationStyle AuthenticationStyle { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public TokenRevocationClient(Uri endpoint)
            : this(endpoint, new HttpBaseProtocolFilter())
        { }

        public TokenRevocationClient(Uri endpoint, IHttpFilter innerHttpMessageHandler)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            _endpoint = endpoint;

            _client = new HttpClient(innerHttpMessageHandler);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new HttpMediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.Custom;
        }

        public TokenRevocationClient(Uri endpoint, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(endpoint, clientId, clientSecret, new HttpBaseProtocolFilter(), style)
        { }

        public TokenRevocationClient(Uri endpoint, string clientId, string clientSecret, IHttpFilter innerHttpMessageHandler, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(endpoint, innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;

            if (style == AuthenticationStyle.BasicAuthentication)
            {
                _client.DefaultRequestHeaders.Authorization = HttpCredentialsHeaderValueExtensions.CreateBasic(clientId, clientSecret);
            }
        }

        public virtual async Task<TokenRevocationResponse> RevokeAsync(
            string token,
            string token_type_hint = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = new Dictionary<string, string>
            {
                { "token", token},
            };
            if (string.IsNullOrWhiteSpace(token_type_hint) == false)
            {
                form.Add("token_type_hint", token_type_hint);
            }
            if (AuthenticationStyle == AuthenticationStyle.PostValues)
            {
                form.Add("client_id", ClientId);
                form.Add("client_secret", ClientSecret);
            }

            var response = await _client.PostAsync(_endpoint, new HttpFormUrlEncodedContent(form)).AsTask(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Ok)
            {
                return new TokenRevocationResponse();
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().AsTask(cancellationToken).ConfigureAwait(false);
                return new TokenRevocationResponse(content);
            }
            else
            {
                return new TokenRevocationResponse((ushort)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
