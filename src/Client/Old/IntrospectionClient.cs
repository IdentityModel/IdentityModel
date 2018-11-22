// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client for the OAuth 2.0 introspection endpoint
    /// </summary>
    [Obsolete("This type will be deprecated or changed in a future version. It is recommended that you switch to the new extension methods for HttpClient. They give you much more control over the HttpClient lifetime and configuration. See the docs here: https://identitymodel.readthedocs.io")]
    public class IntrospectionClient : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// The HTTP client
        /// </summary>
        protected readonly HttpClient Client;

        /// <summary>
        /// The client identifier
        /// </summary>
        protected readonly string ClientId;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionClient" /> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <param name="headerStyle">The header style.</param>
        /// <exception cref="ArgumentNullException">endpoint</exception>
        /// <exception cref="ArgumentException">Invalid header style - headerStyle</exception>
        public IntrospectionClient(string endpoint, string clientId = "", string clientSecret = "", HttpMessageHandler innerHttpMessageHandler = null, BasicAuthenticationHeaderStyle headerStyle = BasicAuthenticationHeaderStyle.Rfc6749)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) innerHttpMessageHandler = new HttpClientHandler();

            Client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (clientId.IsPresent() && clientSecret.IsPresent())
            {
                if (headerStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                {
                    Client.SetBasicAuthenticationOAuth(clientId, clientSecret);
                }
                else if (headerStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                {
                    Client.SetBasicAuthentication(clientId, clientSecret);
                }
                else
                {
                    throw new ArgumentException("Invalid header style", nameof(headerStyle));
                }
                
            }
            else if (!string.IsNullOrWhiteSpace(clientId))
            {
                ClientId = clientId;
            }
        }

        /// <summary>
        /// Sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout
        {
            set
            {
                Client.Timeout = value;
            }
        }

        /// <summary>
        /// Sends the introspection request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// Token
        /// </exception>
        public virtual async Task<IntrospectionResponse> SendAsync(IntrospectionRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Token.IsMissing()) throw new ArgumentNullException(nameof(request.Token));

            IDictionary<string, string> form;
            if (request.Parameters == null)
            {
                form = new Dictionary<string, string>();
            }
            else
            {
                form = request.Parameters;
            }

            form.Add("token", request.Token);
            
            if (request.ClientId.IsPresent())
            {
                form.Add("client_id", request.ClientId);
            }
            else if (ClientId.IsPresent())
            {
                form.Add("client_id", ClientId);
            }

            form.AddIfPresent("token_type_hint", request.TokenTypeHint);
            form.AddIfPresent("client_secret", request.ClientSecret);

            HttpResponseMessage response;
            try
            {
                response = await Client.PostAsync("", new FormUrlEncodedContent(form)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new IntrospectionResponse(ex);
            }
            if (response.IsSuccessStatusCode)
            {
                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            else
            {
                return new IntrospectionResponse(response.StatusCode, response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                Client.Dispose();
            }
        }
    }
}