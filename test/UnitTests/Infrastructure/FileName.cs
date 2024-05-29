// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using System.Runtime.CompilerServices;

namespace IdentityModel.UnitTests
{
    internal static class FileName
    {
        public static string Create(string name) => Path.Combine(UnitTestsPath(), "documents", name);

        private static string UnitTestsPath([CallerFilePath] string path = "") => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), ".."));
    }
}
