// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.UnitTests
{
    internal sealed class InProcHttpServer : IDisposable
    {
        public sealed class HttpContext
        {
            public HttpContext(HttpRequestMessage request, int requestIndex, string requestBody, CancellationToken cancellationToken)
            {
                Request = request ?? throw new ArgumentNullException(nameof(request));
                RequestIndex = requestIndex;
                RequestBody = requestBody ?? string.Empty;
                CancellationToken = cancellationToken;
            }

            public CancellationToken CancellationToken { get; }
            public HttpRequestMessage Request { get; }
            public int RequestIndex { get; }
            public string RequestBody { get; }
            public HttpResponseMessage Response { get; set; }

            public HttpResponseMessage Json(string json)
            {
                var response = StatusCode(HttpStatusCode.OK);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return response;
            }

            public HttpResponseMessage StatusCode(HttpStatusCode httpStatusCode)
            {
                return new HttpResponseMessage(httpStatusCode)
                {
                    RequestMessage = Request
                };
            }
        }

        private readonly List<Func<HttpContext, Task>> _handlers = new List<Func<HttpContext, Task>>();
        private int _requestIndex;

        public void Dispose()
        {
            _handlers.Clear();
        }

        public InProcHttpServer Handle(Action<HttpContext> handler)
        {
            return Handle(context =>
            {
                handler?.Invoke(context);
                return Task.FromResult(true);
            });
        }

        public InProcHttpServer Handle(Func<HttpContext, Task> handler)
        {
            _handlers.Add(handler ?? throw new ArgumentNullException(nameof(handler)));
            return this;
        }

        public HttpClient CreateClient(Func<HttpMessageHandler, HttpMessageHandler> decorate = null)
        {
            var handler = CreateHandler();
            if (decorate != null)
            {
                handler = decorate(handler);
            }
            return new HttpClient(handler);
        }

        public HttpMessageHandler CreateHandler()
        {
            return new InProcHttpMessageHandler(this);
        }

        private async Task<HttpResponseMessage> ServeAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestIndex = Interlocked.Increment(ref _requestIndex);
            var requestBody = request.Content != null
                ? await request.Content.ReadAsStringAsync().ConfigureAwait(false)
                : string.Empty;
            var httpContext = new HttpContext(request, requestIndex, requestBody, cancellationToken);
            try
            {
                foreach (var handler in _handlers)
                {
                    await handler.Invoke(httpContext).ConfigureAwait(false);
                    if (httpContext.Response != null)
                    {
                        return httpContext.Response;
                    }
                }
            }
            catch
            {
                return httpContext.StatusCode(HttpStatusCode.InternalServerError);
            }
            return httpContext.StatusCode(HttpStatusCode.NotAcceptable);
        }

        private sealed class InProcHttpMessageHandler : HttpMessageHandler
        {
            private readonly InProcHttpServer _server;

            public InProcHttpMessageHandler(InProcHttpServer server)
            {
                _server = server ?? throw new ArgumentNullException(nameof(server));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _server.ServeAsync(request, cancellationToken);
            }
        }
    }
}