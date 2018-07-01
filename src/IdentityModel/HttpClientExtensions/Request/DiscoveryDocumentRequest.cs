// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;

namespace IdentityModel.HttpClientExtensions
{
    public class DiscoveryDocumentRequest
    {
        public string Address { get; set; }
        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();
    }
}
