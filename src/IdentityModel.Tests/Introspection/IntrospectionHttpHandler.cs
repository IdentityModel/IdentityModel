// Copyright (c) Dominick Baier & Brock Allen. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Tests
{
    class IntrospectionHttpHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, object> _payload;

        public IntrospectionHttpHandler()
        {
            _payload = null; // 401
        }

        public IntrospectionHttpHandler(Dictionary<string, object> payload)
        {
            _payload = payload;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_payload == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return Task.FromResult(response);
            }
            else
            {
                var json = SimpleJson.SimpleJson.SerializeObject(_payload);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return Task.FromResult(response);
            }
        }
    }
}