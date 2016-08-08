using System;

namespace Thinktecture.IdentityModel.Hawk.Core.Extensions
{
    internal static class ByteExtension
    {
        /// <summary>
        /// Compare two byte arrays using constant-time algorithm (to prevent time-based analysis of MAC digest match).
        /// </summary>
        /// <param name="a">The first byte array</param>
        /// <param name="b">The second byte array (base64 encoded)</param>
        /// <returns>true, if the arrays are equal byte for byte</returns>
        internal static bool IsConstantTimeEqualTo(this byte[] a, byte[] b)
        {
            bool areEqual = (a.Length == b.Length ? true : false);
            if (!areEqual)
                b = a; // Not equal but we do not bail out until we inspect all the elements in the array
            
            for (int i = 0; i < a.Length; i++)
                areEqual = areEqual && (a[i] == b[i]);

            return areEqual;
        }

        /// <summary>
        /// Converts a byte array to its equivalent string representation that is base-64 encoded.
        /// </summary>
        internal static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a byte array to its equivalent string representation that is base-64 URL encoded.
        /// </summary>
        internal static string ToBase64UrlString(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
