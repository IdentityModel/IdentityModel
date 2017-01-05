// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using System;

namespace IdentityModel.Internal
{
    internal static class DiscoveryUrlHelper
    {
        public static bool IsValidScheme(Uri url, DiscoveryPolicy policy)
        {
            if (string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsSecureScheme(Uri url, DiscoveryPolicy policy)
        {
            if (policy.RequireHttps == true)
            {
                if (policy.AllowHttpOnLoopback == true)
                {
                    if (string.Equals(url.DnsSafeHost, "localhost", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(url.DnsSafeHost, "127.0.0.1", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}