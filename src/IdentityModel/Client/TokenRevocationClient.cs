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
        protected HttpClient Client;
        private readonly string _clientId;

        public AuthenticationStyle AuthenticationStyle { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public TokenRevocationClient(string endpoint, string clientId = "", string clientSecret = "", HttpMessageHandler innerHttpMessageHandler = null)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) innerHttpMessageHandler = new HttpClientHandler();

            Client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                Client.SetBasicAuthentication(clientId, clientSecret);
            }
            else if (!string.IsNullOrWhiteSpace(clientId))
            {
                _clientId = clientId;
            }
        }

        public TimeSpan Timeout
        {
            set
            {
                Client.Timeout = value;
            }
        }

        public virtual async Task<TokenRevocationResponse> RevokeAsync(
            TokenRevocationRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentNullException(nameof(request.Token));

            var form = new Dictionary<string, string>();
            form.Add("token", request.Token);

            if (!string.IsNullOrWhiteSpace(request.TokenTypeHint))
            {
                form.Add("token_type_hint", request.TokenTypeHint);
            }

            if (!string.IsNullOrWhiteSpace(request.ClientId))
            {
                form.Add("client_id", request.ClientId);
            }
            else if (!string.IsNullOrWhiteSpace(_clientId))
            {
                form.Add("client_id", _clientId);
            }

            if (!string.IsNullOrWhiteSpace(request.ClientSecret))
            {
                form.Add("client_secret", request.ClientSecret);
            }

            try
            {
                var response = await Client.PostAsync("", new FormUrlEncodedContent(form), cancellationToken).ConfigureAwait(false);

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
            catch (Exception ex)
            {
                return new TokenRevocationResponse(ex);
            }
        }
    }
}