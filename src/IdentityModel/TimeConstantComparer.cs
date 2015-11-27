/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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