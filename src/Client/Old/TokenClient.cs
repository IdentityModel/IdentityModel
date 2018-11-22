// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client for an OpenID Connect/OAuth 2.0 token endpoint
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    [Obsolete("This type will be deprecated or changed in a future version. It is recommended that you switch to the new extension methods for HttpClient. They give you much more control over the HttpClient lifetime and configuration. See the docs here: https://identitymodel.readthedocs.io")]
    public class TokenClient : IDisposable
    {
        private bool _disposed;
        
        /// <summary>
        /// The HTTP client
        /// </summary>
        protected HttpClient Client;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public TokenClient(string address)
            : this(address, new HttpClientHandler())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">
        /// address
        /// or
        /// innerHttpMessageHandler
        /// </exception>
        public TokenClient(string address, HttpMessageHandler innerHttpMessageHandler = null)
        {
            innerHttpMessageHandler = innerHttpMessageHandler ?? new HttpClientHandler();

            Client = new HttpClient(innerHttpMessageHandler);

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.Custom;
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="style">The authentication style.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        public TokenClient(string address, string clientId, HttpMessageHandler innerHttpMessageHandler = null, AuthenticationStyle style = AuthenticationStyle.PostValues)
            : this(address, clientId, string.Empty, style: style, innerHttpMessageHandler: innerHttpMessageHandler)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <param name="style">The authentication style.</param>
        /// <exception cref="System.ArgumentNullException">clientId</exception>
        public TokenClient(string address, string clientId, string clientSecret, HttpMessageHandler innerHttpMessageHandler = null, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, innerHttpMessageHandler)
        {
            if (clientId.IsMissing()) throw new ArgumentNullException(nameof(clientId));

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; }

        /// <summary>
        /// Gets or sets the basic authentication header style.
        /// </summary>
        /// <value>
        /// The basic authentication header style.
        /// </value>
        public BasicAuthenticationHeaderStyle BasicAuthenticationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; }

        /// <summary>
        /// Gets or sets the authentication style.
        /// </summary>
        /// <value>
        /// The authentication style.
        /// </value>
        public AuthenticationStyle AuthenticationStyle { get; }

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
        /// Sends a token request.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<TokenResponse> RequestAsync(IDictionary<string, string> form, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            var request = new HttpRequestMessage(HttpMethod.Post, Address)
            {
                Content = new FormUrlEncodedContent(form)
            };

            if (AuthenticationStyle == AuthenticationStyle.BasicAuthentication)
            {
                if (BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                {
                    request.SetBasicAuthenticationOAuth(ClientId, ClientSecret);
                }
                else if (BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                {
                    request.SetBasicAuthentication(ClientId, ClientSecret);
                }
                else
                {
                    throw new InvalidOperationException("Invalid basic authentication header style");
                }
            }

            try
            {
                response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new TokenResponse(ex);
            }

            string content = null;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new TokenResponse(content);
            }
            else
            {
                return new TokenResponse(response.StatusCode, response.ReasonPhrase, content);
            }
        }

        /// <summary>
        /// Merges the explicitly provided values with the extra object
        /// </summary>
        /// <param name="explicitValues">The explicit values.</param>
        /// <param name="extra">The extra.</param>
        /// <returns></returns>
        public Dictionary<string, string> Merge(Dictionary<string, string> explicitValues, object extra = null)
        {
            var merged = explicitValues;

            if (AuthenticationStyle == AuthenticationStyle.PostValues)
            {
                merged.Add(OidcConstants.TokenRequest.ClientId, ClientId);

                if (ClientSecret.IsPresent())
                {
                    merged.Add(OidcConstants.TokenRequest.ClientSecret, ClientSecret);
                }
            }

            var additionalValues = ValuesHelper.ObjectToDictionary(extra);

            if (additionalValues != null)
            {
                merged =
                    explicitValues.Concat(additionalValues.Where(add => !explicitValues.ContainsKey(add.Key)))
                                         .ToDictionary(final => final.Key, final => final.Value);
            }

            return merged;
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