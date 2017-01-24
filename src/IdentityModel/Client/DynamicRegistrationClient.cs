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
    public class DynamicRegistrationClient : IDisposable
    {
        protected HttpClient Client;
        private bool _disposed;

        public string Address { get; set; }

        public DynamicRegistrationClient(string address)
            : this(address, new HttpClientHandler())
        { }

        public DynamicRegistrationClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            Client = new HttpClient(innerHttpMessageHandler);

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            Address = address;
        }

        public TimeSpan Timeout
        {
            set
            {
                Client.Timeout = value;
            }
        }

        public virtual async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request, string token = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Address);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

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
                Client.Dispose();
            }
        }
    }
}