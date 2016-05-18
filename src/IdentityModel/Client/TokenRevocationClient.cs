// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class TokenRevocationClient
    {
        protected HttpClient _client;
        
        public AuthenticationStyle AuthenticationStyle { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public TokenRevocationClient(string address)
            : this(address, new HttpClientHandler())
        { }

        public TokenRevocationClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (innerHttpMessageHandler == null) throw new ArgumentNullException("innerHttpMessageHandler");
            
            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(address)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.None;
        }

        public TokenRevocationClient(string address, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, clientId, clientSecret, new HttpClientHandler(), style)
        { }

        public TokenRevocationClient(string address, string clientId, string clientSecret, HttpMessageHandler innerHttpMessageHandler, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;

            if (style == AuthenticationStyle.BasicAuthentication)
            {
                _client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
            }
        }

        public TimeSpan Timeout 
        { 
            set
            {
                _client.Timeout = value;
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
            if (String.IsNullOrWhiteSpace(token_type_hint) == false)
            {
                form.Add("token_type_hint", token_type_hint);
            }
            if (AuthenticationStyle == AuthenticationStyle.PostValues)
            {
                form.Add("client_id", ClientId);
                form.Add("client_secret", ClientSecret);
            }

            var response = await _client.PostAsync(string.Empty, new FormUrlEncodedContent(form), cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new TokenRevocationResponse();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new TokenRevocationResponse(content);
            }
            else
            {
                return new TokenRevocationResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}