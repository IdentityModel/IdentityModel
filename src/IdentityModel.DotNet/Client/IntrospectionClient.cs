/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                _client.SetBasicAuthentication(clientId, clientSecret);
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

            if (string.IsNullOrWhiteSpace(request.ClientId))
            {
                form.Add("client_id", request.ClientId);
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