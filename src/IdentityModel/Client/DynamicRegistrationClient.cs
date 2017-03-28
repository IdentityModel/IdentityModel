// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client for the OpenID Connect dynamic client registration endpoint
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DynamicRegistrationClient : IDisposable
    {
        /// <summary>
        /// The client
        /// </summary>
        protected HttpClient Client;

        private bool _disposed;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRegistrationClient"/> class.
        /// </summary>
        /// <param name="address">The endpoint address.</param>
        public DynamicRegistrationClient(string address)
            : this(address, new HttpClientHandler())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRegistrationClient"/> class.
        /// </summary>
        /// <param name="address">The endpoint address.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">
        /// address
        /// or
        /// innerHttpMessageHandler
        /// </exception>
        public DynamicRegistrationClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            Client = new HttpClient(innerHttpMessageHandler);

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            Address = address ?? throw new ArgumentNullException(nameof(address));
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
        /// Send a registration message to the endpoint.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request, string token = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Address)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                response = await Client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new RegistrationResponse(ex);
            }

            if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new RegistrationResponse(content);
            }
            else
            {
                return new RegistrationResponse(response.StatusCode, response.ReasonPhrase);
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