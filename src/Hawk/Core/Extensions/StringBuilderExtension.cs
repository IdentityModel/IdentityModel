using System;
using System.Text;

namespace Thinktecture.IdentityModel.Hawk.Core.Extensions
{
    internal static class StringBuilderExtension
    {
        /// <summary>
        /// Appends the key-value in the form of key="value" with a trailing trailer
        /// and a space, if value is not null or String.Empty
        /// </summary>
        internal static StringBuilder AppendIfNotEmpty(this StringBuilder builder, string key, string value, char trailer)
        {
            if (!String.IsNullOrEmpty(value))
            {
                builder.AppendFormat("{0}=\"{1}\"", key, value).Append(trailer).Append(" ");
            }

            return builder;
        }

        /// <summary>
        /// Appends the string followed by a new line (\n) to the StringBuilder object, substituting an empty string in case
        /// the passed in string is null. The out-of-box AppendLine method is not used  because default line terminator is
        /// not just a new line character.
        /// </summary>
        internal static StringBuilder AppendNewLine(this StringBuilder builder, string value)
        {
            builder.Append(value ?? String.Empty).Append("\n");

            return builder;
        }
    }
}
