Epoch Time Conversion
=====================
JWT tokens use so called `Epoch or Unix time <https://en.wikipedia.org/wiki/Unix_time>`_ to represent date/times.

IdentityModel contains extensions methods for ``DateTime`` and ``DateTimeOffset`` or convert to/from Unix time::

    var dt = DateTime.UtcNow;
    var unix = dt.ToEpochTime();

.. note:: Starting with .NET Framework 4.6 and .NET Core 1.0 this functionality is built-in via `DateTimeOffset.FromUnixTimeSeconds <https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds>`_ and `DateTimeOffset.ToUnixTimeSeconds <https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds>`_.