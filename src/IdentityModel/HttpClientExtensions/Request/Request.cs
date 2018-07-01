// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class Request
    {
        public string Address { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientAssertion Assertion { get; set; } = new ClientAssertion();

        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;
        public BasicAuthenticationHeaderStyle BasicAuthenticationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public class ClientAssertion
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}