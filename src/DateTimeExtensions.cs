// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace IdentityModel;

/// <summary>
/// Extensions for converting epoch/unix time to DateTime and DateTimeOffset
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts the given date value to epoch time.
    /// </summary>
    public static long ToEpochTime(this DateTime dateTime)
    {
        var date = dateTime.ToUniversalTime();
        var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var ts = ticks / TimeSpan.TicksPerSecond;
        return ts;
    }

    /// <summary>
    /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
    /// </summary>
    public static DateTime ToDateTimeFromEpoch(this long date)
    {
        var timeInTicks = date * TimeSpan.TicksPerSecond;
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
    }
}