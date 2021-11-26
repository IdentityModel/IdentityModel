// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class TokenRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the type of the grant.
    /// </summary>
    /// <value>
    /// The type of the grant.
    /// </value>
    public string GrantType { get; set; }
}

/// <summary>
/// Request for token using client_credentials
/// </summary>
/// <seealso cref="TokenRequest" />
public class ClientCredentialsTokenRequest : TokenRequest
{
    /// <summary>
    /// Space separated list of the requested scopes 
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string Scope { get; set; }
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}

/// <summary>
/// Request for token using urn:ietf:params:oauth:grant-type:device_code
/// </summary>
/// <seealso cref="TokenRequest" />
public class DeviceTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the device code.
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string DeviceCode { get; set; }
}

/// <summary>
/// Request for token using password
/// </summary>
/// <seealso cref="TokenRequest" />
public class PasswordTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string Password { get; set; }

    /// <summary>
    /// Space separated list of the requested scopes
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string Scope { get; set; }
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}

/// <summary>
/// Request for token using authorization_code
/// </summary>
/// <seealso cref="TokenRequest" />
public class AuthorizationCodeTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>
    /// The code.
    /// </value>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    /// <value>
    /// The redirect URI.
    /// </value>
    public string RedirectUri { get; set; }
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();

    /// <summary>
    /// Gets or sets the code verifier.
    /// </summary>
    /// <value>
    /// The code verifier.
    /// </value>
    public string CodeVerifier { get; set; }
}

/// <summary>
/// Request for token using refresh_token
/// </summary>
/// <seealso cref="TokenRequest" />
public class RefreshTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <value>
    /// The refresh token.
    /// </value>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Space separated list of the requested scopes.  The Scope attribute cannot be used to extend the scopes granted by the resource owner
    /// </summary>
    /// <remarks>
    /// See https://datatracker.ietf.org/doc/html/rfc6749#section-6 for further detail on restrictions
    /// </remarks>
    /// <value>
    /// The scope.
    /// </value>
    public string Scope { get; set; }

    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The resources.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}
    
/// <summary>
/// Request for token using urn:ietf:params:oauth:grant-type:token-exchange
/// </summary>
/// <seealso cref="TokenRequest" />
public class TokenExchangeTokenRequest : TokenRequest
{
    /// <summary>
    /// OPTIONAL.  A URI that indicates the target service or resource.
    /// </summary>
    public string Resource { get; set; }

    /// <summary>
    /// OPTIONAL.  The logical name of the target service where the client intends to use the requested security token.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// OPTIONAL. Space separated list of the requested scopes
    /// </summary>
    public string Scope { get; set; }
        
    /// <summary>
    /// OPTIONAL.  An identifier for the type of the requested security token.
    /// </summary>
    public string RequestedTokenType { get; set; }

    /// <summary>
    /// REQUIRED.  A security token that represents the identity of the party on behalf of whom the request is being made.
    /// </summary>
    public string SubjectToken { get; set; }

    /// <summary>
    /// REQUIRED.  An identifier that indicates the type of the security token in the "subject_token" parameter.
    /// </summary>
    public string SubjectTokenType { get; set; }

    /// <summary>
    /// OPTIONAL.  A security token that represents the identity of the acting party.
    /// </summary>
    public string ActorToken { get; set; }

    /// <summary>
    /// An identifier that indicates the type of the security token in the "actor_token" parameter. This is REQUIRED when the "actor_token" parameter is present in the request but MUST NOT be included otherwise.
    /// </summary>
    public string ActorTokenType { get; set; }
}

/// <summary>
/// Request for token using urn:openid:params:grant-type:ciba grant type
/// </summary>
/// <seealso cref="TokenRequest" />
public class BackchannelAuthenticationTokenRequest : TokenRequest
{
    /// <summary>
    /// REQUIRED. It is the unique identifier to identify the authentication request (transaction) made by the Client.
    /// </summary>
    public string AuthenticationRequestId { get; set; }
}