// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Runtime.CompilerServices;

namespace IdentityModel
{
    /// <summary>
    /// Helper class to do equality checks without leaking timing information
    /// </summary>
    public static class TimeConstantComparer
    {
        /// <summary>
        /// Checks two strings for equality without leaking timing information.
        /// </summary>
        /// <param name="s1">string 1.</param>
        /// <param name="s2">string 2.</param>
        /// <returns>
        /// 	<c>true</c> if the specified strings are equal; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool IsEqual(string s1, string s2)
        {
            if (s1 == null && s2 == null)
            {
                return true;
            }

            if (s1 == null || s2 == null)
            {
                return false;
            }

            if (s1.Length != s2.Length)
            {
                return false;
            }

            var s1chars = s1.ToCharArray();
            var s2chars = s2.ToCharArray();

            int hits = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1chars[i].Equals(s2chars[i]))
                {
                    hits += 2;
                }
                else
                {
                    hits += 1;
                }
            }

            bool same = (hits == s1.Length * 2);

            return same;
        }
    }
}