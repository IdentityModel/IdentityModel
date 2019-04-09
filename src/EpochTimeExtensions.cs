// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace IdentityModel
{
    /// <summary>
    /// Extensions for converting epoch/unix time to DateTime and DateTimeOffset
    /// </summary>
    public static class EpochTimeExtensions
    {
        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        [Obsolete("This functionality will be removed in a future version - use https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=netframework-4.7.2 and https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=netframework-4.7.2 instead")]
        public static int ToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return (int)ts;
        }

        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        [Obsolete("This functionality will be removed in a future version - use https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=netframework-4.7.2 and https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=netframework-4.7.2 instead")]
        public static int ToEpochTime(this DateTimeOffset dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return (int)ts;
        }

        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        [Obsolete("This functionality will be removed in a future version - use https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=netframework-4.7.2 and https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=netframework-4.7.2 instead")]
        public static DateTime ToDateTimeFromEpoch(this int intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
        }

        /// <summary>
        /// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        [Obsolete("This functionality will be removed in a future version - use https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=netframework-4.7.2 and https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=netframework-4.7.2 instead")]
        public static DateTimeOffset ToDateTimeOffsetFromEpoch(this int intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(timeInTicks);
        }
    }
}