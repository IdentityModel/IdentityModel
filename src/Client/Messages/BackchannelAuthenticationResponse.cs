// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Models a CIBA backchannel authentication response
/// </summary>
/// <seealso cref="IdentityModel.Client.ProtocolResponse" />
public class BackchannelAuthenticationResponse : ProtocolResponse
{
    /// <summary>
    /// REQUIRED. This is a unique identifier to identify the authentication request made by the Client.
    /// </summary>
    public string? AuthenticationRequestId =>
        Json.TryGetString(OidcConstants.BackchannelAuthenticationResponse.AuthenticationRequestId);

    /// <summary>
    /// REQUIRED. A JSON number with a positive integer value indicating the expiration time of the "auth_req_id" in seconds since the authentication request was received.
    /// </summary>
    public int ExpiresIn
    {
        get
        {
            var value = TryGet(OidcConstants.BackchannelAuthenticationResponse.ExpiresIn);

            if (value != null)
            {
                if (int.TryParse(value, out var theValue))
                {
                    return theValue;
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// OPTIONAL. A JSON number with a positive integer value indicating the minimum amount of time in seconds that the Client MUST wait between polling requests to the token endpoint.
    /// </summary>
    public int? Interval => Json.TryGetInt(OidcConstants.BackchannelAuthenticationResponse.Interval);
}