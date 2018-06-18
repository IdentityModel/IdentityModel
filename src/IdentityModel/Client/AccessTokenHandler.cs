// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HTTP message handler that encapsulates access token handling and renewment
    /// </summary>
    [Obsolete("Use AccesTokenDelegatingHandler (that does not create a default " +
              "inner handler) instead. See " +
              "https://github.com/IdentityModel/IdentityModel2/pull/110", false)]
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly TokenClient _tokenClient;
        private readonly string _scope;

        private string _accessToken;
        private bool _disposed;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        
        /// <summary>
        /// Gets the current access token
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (_lock.Wait(Timeout))
                {
                    try
                    {
                        return _accessToken;
                    }
                    finally
                    {
                        _lock.Release();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Occurs when the tokens were renewed successfully
        /// </summary>
        public event EventHandler<TokenRenewedEventArgs> TokenRenewed = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenHandler"/> class.
        /// </summary>
        /// <param name="tokenEndpoint">The token endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public AccessTokenHandler(string tokenEndpoint, string clientId, string clientSecret, string scope, HttpMessageHandler innerHandler = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret, innerHandler), scope, innerHandler)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenHandler"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public AccessTokenHandler(TokenClient client, string scope, HttpMessageHandler innerHandler = null)
        {
            _tokenClient = client;
            _scope = scope;

            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync(cancellationToken);
            if (accessToken.IsMissing())
            {
                if (await RenewTokensAsync(cancellationToken) == false)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) {RequestMessage = request};
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (await RenewTokensAsync(cancellationToken) == false)
            {
                return response;
            }

            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
          if (disposing && !_disposed) {
              _disposed = true;
              _lock.Dispose();
          }

          base.Dispose(disposing);
        }

        private async Task<bool> RenewTokensAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var response = await _tokenClient.RequestClientCredentialsAsync(_scope, cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (!response.IsError)
                    {
                        _accessToken = response.AccessToken;

#pragma warning disable 4014
                        Task.Run(() =>
                        {
                            foreach (EventHandler<TokenRenewedEventArgs> del in TokenRenewed.GetInvocationList())
                            {
                                try
                                {
                                    del(this, new TokenRenewedEventArgs(response.AccessToken, response.ExpiresIn));
                                }
                                catch { }
                            }
                        }).ConfigureAwait(false);
#pragma warning restore 4014

                        return true;
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return false;
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    return _accessToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
        }
    }
}