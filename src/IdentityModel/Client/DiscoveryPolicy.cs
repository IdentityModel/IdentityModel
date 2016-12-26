// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client
{
    public class DiscoveryPolicy
    {
        public bool ValidateIssuerName { get; set; } = true;
        public bool ValidateEndpoints { get; set; } = true;

        public bool RequireHttps { get; set; } = true;
    }
}