// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// Client library for the OpenID Connect / OAuth 2 token endpoint
/// </summary>
public class TokenClient
{
    private readonly Func<HttpMessageInvoker> _client;
    private readonly TokenClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenClient"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">client</exception>
    public TokenClient(HttpMessageInvoker client, TokenClientOptions options)
        : this(() => client, options)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenClient"/> class.
    /// </summary>
    /// <param name="client">The client func.</param>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">client</exception>
    public TokenClient(Func<HttpMessageInvoker> client, TokenClientOptions options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Sets request parameters from the options.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="parameters">The parameters.</param>
    internal void ApplyRequestParameters(TokenRequest request, Parameters? parameters)
    {
        request.Address = _options.Address;
        request.ClientId = _options.ClientId;
        request.ClientSecret = _options.ClientSecret!;
        request.ClientAssertion = _options.ClientAssertion!;
        request.ClientCredentialStyle = _options.ClientCredentialStyle;
        request.AuthorizationHeaderStyle = _options.AuthorizationHeaderStyle;
        request.Parameters = new Parameters(_options.Parameters);

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                request.Parameters.Add(parameter);
            }
        }
    }

    /// <summary>
    /// Sends a token request using the client_credentials grant type.
    /// </summary>
    /// <param name="scope">The scope (space separated string).</param>
    /// <param name="parameters">Extra parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestClientCredentialsTokenAsync(string? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new ClientCredentialsTokenRequest
        {
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestClientCredentialsTokenAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a token request using the urn:ietf:params:oauth:grant-type:device_code grant type.
    /// </summary>
    /// <param name="deviceCode">The device code.</param>
    /// <param name="parameters">Extra parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestDeviceTokenAsync(string deviceCode, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new DeviceTokenRequest
        {
            DeviceCode = deviceCode
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestDeviceTokenAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a token request using the password grant type.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="scope">The scope (space separated string).</param>
    /// <param name="parameters">Extra parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestPasswordTokenAsync(string userName, string? password = null, string? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new PasswordTokenRequest
        {
            UserName = userName,
            Password = password,
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestPasswordTokenAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a token request using the authorization_code grant type.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="codeVerifier">The code verifier.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestAuthorizationCodeTokenAsync(string code, string redirectUri, string? codeVerifier = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new AuthorizationCodeTokenRequest
        {
            Code = code,
            RedirectUri = redirectUri,
            CodeVerifier = codeVerifier
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestAuthorizationCodeTokenAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a token request using the refresh_token grant type.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The scope (space separated string).</param>
    /// <param name="parameters">Extra parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken, string? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new RefreshTokenRequest
        {
            RefreshToken = refreshToken,
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestRefreshTokenAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a token request.
    /// </summary>
    /// <param name="grantType">Type of the grant.</param>
    /// <param name="parameters">Extra parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<TokenResponse> RequestTokenAsync(string grantType, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        var request = new TokenRequest
        {
            GrantType = grantType
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestTokenAsync(request, cancellationToken);
    }
}