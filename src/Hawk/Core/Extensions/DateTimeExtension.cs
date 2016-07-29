using System;

namespace Thinktecture.IdentityModel.Hawk.Core.Extensions
{
    internal static class DateTimeExtension
    {
        /// <summary>
        /// Converts DateTime to its equivalent UNIX time expressed in milliseconds. Unix time is the number of seconds elapsed since Jan 1, 1970 midnight UTC.
        /// </summary>
        internal static ulong ToUnixTimeMillis(this DateTime dateTime)
        {
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan ts = dateTime - epochStart;
            
            return Convert.ToUInt64(ts.TotalSeconds * 1000);
        }

        /// <summary>
        /// Converts DateTime to its equivalent UNIX time, the number of seconds elapsed since Jan 1, 1970 midnight UTC.
        /// </summary>
        internal static ulong ToUnixTime(this DateTime dateTime)
        {
            return dateTime.ToUnixTimeMillis() / 1000;
        }
    }
}
