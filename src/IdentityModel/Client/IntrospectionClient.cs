// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class IntrospectionClient
    {
        private readonly HttpClient _client;
        private readonly string _clientId;

        public IntrospectionClient(string endpoint, string clientId = "", string clientSecret = "", HttpMessageHandler innerHttpMessageHandler = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentNullException("endpoint");
            if (innerHttpMessageHandler == null) innerHttpMessageHandler = new HttpClientHandler();

            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                _client.SetBasicAuthentication(clientId, clientSecret);
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
                _client.Timeout = value;
            }
        }

        public async Task<IntrospectionResponse> SendAsync(IntrospectionRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentNullException("token");

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
                var response = await _client.PostAsync("", new FormUrlEncodedContent(form)).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new IntrospectionResponse
                    {
                        IsError = true,
                        Error = response.ReasonPhrase
                    };
                }

                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return new IntrospectionResponse
                {
                    IsError = true,
                    Error = ex.Message
                };
            }
        }
    }
}