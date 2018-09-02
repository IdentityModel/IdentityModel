Time-Constant String Comparison
===============================
When comparing strings in a security context (e.g. comparing keys), you should try to avoid leaking timing information.

The ``TimeConstantComparer`` class can help with that:: 

    var isEqual = TimeConstantComparer.IsEqual(key1, key2);

.. note:: Starting with .NET Core 2.1 this functionality is built in via `CryptographicOperations.FixedTimeEquals <https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=netcore-2.1>`_