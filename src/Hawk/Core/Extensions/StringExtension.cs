using System;
using System.Text;

namespace Thinktecture.IdentityModel.Hawk.Core.Extensions
{
    internal static class StringExtension
    {
        /// <summary>
        /// Returns the byte array corresponding to the UTF-8 encoded string.
        /// </summary>
        internal static byte[] ToBytesFromUtf8(this string utf8String)
        {
            return Encoding.UTF8.GetBytes(utf8String);
        }

        /// <summary>
        /// Returns the byte array corresponding to the base-64 encoded string.
        /// </summary>
        internal static byte[] ToBytesFromBase64(this string base64EncodedString)
        {
            return Convert.FromBase64String(base64EncodedString);
        }

        /// <summary>
        /// Returns the UTF-8 string corresponding to the base-64 URL encoded string.
        /// </summary>
        internal static string ToUtf8StringFromBase64Url(this string base64UrlEncodedString)
        {
            string input = base64UrlEncodedString;

            input = input.Replace('-', '+').Replace('_', '/');
            int pad = 4 - (input.Length % 4);
            pad = pad > 2 ? 0 : pad;
            input = input.PadRight(input.Length + pad, '=');
            
            return Encoding.UTF8.GetString(
                            Convert.FromBase64String(input));
        }

        /// <summary>
        /// Returns true, if the specified string can be parsed to System.Boolean.
        /// </summary>
        internal static bool ToBool(this string input)
        {
            return Boolean.Parse(input);
        }
    }
}
