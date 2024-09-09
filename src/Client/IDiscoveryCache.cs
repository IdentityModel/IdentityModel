// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// Interface for discovery cache
/// </summary>
public interface IDiscoveryCache
{
    /// <summary>
    /// Gets or sets the duration of the cache.
    /// </summary>
    /// <value>
    /// The duration of the cache.
    /// </value>
    TimeSpan CacheDuration { get; set; }

    /// <summary>
    /// Retrieves the discovery document
    /// </summary>
    /// <returns></returns>
    Task<DiscoveryDocumentResponse> GetAsync();

    /// <summary>
    /// Forces a refresh on the next get.
    /// </summary>
    void Refresh();
}