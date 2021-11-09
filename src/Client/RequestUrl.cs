// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System.Linq;

namespace IdentityModel.Client;

/// <summary>
/// Helper class for creating request URLs
/// </summary>
public class RequestUrl
{
    private readonly string _baseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestUrl"/> class.
    /// </summary>
    /// <param name="baseUrl">The authorize endpoint.</param>
    public RequestUrl(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Creates URL based on key/value input pairs.
    /// </summary>
    /// <param name="parameters">The query string parameters.</param>
    /// <returns></returns>
    public string Create(Parameters parameters)
    {
        if (parameters == null || !parameters.Any())
        {
            return _baseUrl;
        }

        return QueryHelpers.AddQueryString(_baseUrl, parameters);
    }
}