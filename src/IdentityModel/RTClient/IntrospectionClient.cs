// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using IdentityModel.Client;

namespace Windows.Web.Http
{
    public class IntrospectionClient
    {
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly Uri _endpoint;

        public IntrospectionClient(Uri endpoint, string clientId = "", string clientSecret = "", IHttpFilter innerHttpFilter = null)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpFilter == null) innerHttpFilter = new HttpBaseProtocolFilter();

            _endpoint = endpoint;

            _client = new HttpClient(innerHttpFilter);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new HttpMediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                _client.SetBasicAuthentication(clientId, clientSecret);
            }
            else if (!string.IsNullOrWhiteSpace(clientId))
            {
                _clientId = clientId;
            }
        }

        public async Task<IntrospectionResponse> SendRequestAsync(IntrospectionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentNullException("Token");

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
                var response = await _client.PostAsync(_endpoint, new HttpFormUrlEncodedContent(form)).AsTask(cancellationToken).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.Ok)
                {
                    return new IntrospectionResponse
                    {
                        IsError = true,
                        Error = response.ReasonPhrase
                    };
                }

                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().AsTask(cancellationToken).ConfigureAwait(false));
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