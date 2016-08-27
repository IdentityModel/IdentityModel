// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using IdentityModel.Client;

namespace Windows.Web.Http
{
    public class UserInfoClient
    {
        private readonly HttpClient _client;
        private readonly Uri _endpoint;

        public UserInfoClient(Uri endpoint, string token)
            : this(endpoint, token, new HttpBaseProtocolFilter())
        { }

        public UserInfoClient(Uri endpoint, string token, IHttpFilter innerHttpMessageHandler)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            _endpoint = endpoint;

            _client = new HttpClient(innerHttpMessageHandler);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new HttpMediaTypeWithQualityHeaderValue("application/json"));

            _client.SetBearerToken(token);
        }

        public async Task<UserInfoResponse> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.GetAsync(_endpoint).AsTask(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Ok)
            {
                return new UserInfoResponse((ushort)response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync().AsTask(cancellationToken).ConfigureAwait(false);
            return new UserInfoResponse(content);
        }
    }
}
