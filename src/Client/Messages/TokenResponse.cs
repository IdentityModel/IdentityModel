// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Models a response from an OpenID Connect/OAuth 2 token endpoint
/// </summary>
/// <seealso cref="IdentityModel.Client.ProtocolResponse" />
public class TokenResponse : ProtocolResponse
{
    /// <summary>
    /// Gets the access token.
    /// </summary>
    /// <value>
    /// The access token.
    /// </value>
    public string? AccessToken => TryGet(OidcConstants.TokenResponse.AccessToken);

    /// <summary>
    /// Gets the identity token.
    /// </summary>
    /// <value>
    /// The identity token.
    /// </value>
    public string? IdentityToken => TryGet(OidcConstants.TokenResponse.IdentityToken);

    /// <summary>
    /// Gets the scope.
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string? Scope => TryGet(OidcConstants.TokenResponse.Scope);
        
    /// <summary>
    /// Gets the issued token type.
    /// </summary>
    /// <value>
    /// The issued token type.
    /// </value>
    public string? IssuedTokenType => TryGet(OidcConstants.TokenResponse.IssuedTokenType);
        
    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    /// <value>
    /// The type of the token.
    /// </value>
    public string? TokenType => TryGet(OidcConstants.TokenResponse.TokenType);

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    /// <value>
    /// The refresh token.
    /// </value>
    public string? RefreshToken => TryGet(OidcConstants.TokenResponse.RefreshToken);

    /// <summary>
    /// Gets the error description.
    /// </summary>
    /// <value>
    /// The error description.
    /// </value>
    public string? ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription);

    /// <summary>
    /// Gets the expires in.
    /// </summary>
    /// <value>
    /// The expires in.
    /// </value>
    public int ExpiresIn
    {
        get
        {
            var value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

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
}