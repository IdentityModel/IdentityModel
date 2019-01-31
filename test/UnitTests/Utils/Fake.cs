// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace IdentityModel.UnitTests
{
    internal static class Fake
    {
        public static string ReferenceToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string String()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}