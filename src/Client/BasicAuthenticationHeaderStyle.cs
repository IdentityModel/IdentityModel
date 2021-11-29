// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Enum for specifying then encoding style of the basic authentication header
/// </summary>
public enum BasicAuthenticationHeaderStyle
{
    /// <summary>
    /// Recommended. Uses the encoding as described in the OAuth 2.0 spec (https://tools.ietf.org/html/rfc6749#section-2.3.1). Base64(urlformencode(client_id) + ":" + urlformencode(client_secret))
    /// </summary>
    Rfc6749,
    /// <summary>
    /// Uses the encoding as described in the original basic authentication spec (https://tools.ietf.org/html/rfc2617#section-2 - used by some non-OAuth 2.0 compliant authorization servers). Base64(client_id + ":" + client_secret). 
    /// </summary>
    Rfc2617
}