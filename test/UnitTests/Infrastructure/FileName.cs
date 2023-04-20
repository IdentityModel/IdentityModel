// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;

namespace IdentityModel.UnitTests
{
    internal static class FileName
    {
        public static string Create(string name)
        {
#if NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0 || NET6_0 || NET7_0
            var fullName = Path.Combine(System.AppContext.BaseDirectory, "documents", name);
#else
            var fullName = Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "documents", name);
#endif

            return fullName;
        }
    }
}
