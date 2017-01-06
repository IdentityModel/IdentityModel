// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client
{
    public class DiscoveryPolicy
    {
        internal string Authority;

        public bool ValidateIssuerName { get; set; } = true;
        public bool ValidateEndpoints { get; set; } = true;
        public bool RequireKeySet { get; set; } = true;

        public bool RequireHttps { get; set; } = true;
        public bool AllowHttpOnLoopback { get; set; } = true;
        public ICollection<string> LoopbackAddresses = new HashSet<string> { "localhost", "127.0.0.1" };
    }
}