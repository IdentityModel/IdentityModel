// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using IdentityModel;
using IdentityModel.Client;

namespace Windows.Web.Http
{
    public class TokenClient : IDisposable
    {
        protected HttpClient _client;
        private readonly Uri _endpoint;

        private bool _disposed;

        public TokenClient(Uri endpoint)
            : this(endpoint, new HttpBaseProtocolFilter())
        { }

        public TokenClient(Uri endpoint, IHttpFilter innerHttpFilter)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpFilter == null) throw new ArgumentNullException(nameof(innerHttpFilter));

            _endpoint = endpoint;

            _client = new HttpClient(innerHttpFilter);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new HttpMediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.Custom;
        }

        public TokenClient(Uri endpoint, string clientId, AuthenticationStyle style = AuthenticationStyle.PostValues)
            : this(endpoint, clientId, string.Empty, new HttpBaseProtocolFilter(), style)
        { }

        public TokenClient(Uri endpoint, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(endpoint, clientId, clientSecret, new HttpBaseProtocolFilter(), style)
        { }

        public TokenClient(Uri endpoint, string clientId, IHttpFilter innerHttpFilter)
            : this(endpoint, clientId, string.Empty, innerHttpFilter, AuthenticationStyle.PostValues)
        { }

        public TokenClient(Uri endpoint, string clientId, string clientSecret, IHttpFilter innerHttpFilter, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(endpoint, innerHttpFilter)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;

            if (style == AuthenticationStyle.BasicAuthentication)
            {
                _client.DefaultRequestHeaders.Authorization = HttpCredentialsHeaderValueExtensions.CreateBasic(clientId, clientSecret);
            }
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public AuthenticationStyle AuthenticationStyle { get; set; }

        public virtual async Task<TokenResponse> RequestAsync(IDictionary<string, string> form, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.PostAsync(_endpoint, new HttpFormUrlEncodedContent(form)).AsTask(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Ok || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().AsTask(cancellationToken).ConfigureAwait(false);
                return new TokenResponse(content);
            }
            else
            {
                return new TokenResponse((ushort)response.StatusCode, response.ReasonPhrase);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _client.Dispose();
            }
        }
    }
}
