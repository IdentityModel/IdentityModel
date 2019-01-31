// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using IdentityModel.Client;

namespace IdentityModel.UnitTests
{
    internal static class TokenConvert
    {
        public static TokenResponse ToTokenResponse(string accessToken, string refreshToken)
        {
            return new TokenResponse(ToJson(accessToken, refreshToken));
        }

        public static string ToJson(string accessToken, string refreshToken = null)
        {
            if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
            if (refreshToken == null)
            {
                return $"{{\"access_token\":\"{accessToken}\"}}";
            }
            return $"{{\"access_token\":\"{accessToken}\",\"refresh_token\":\"{refreshToken}\"}}";
        }
    }
}