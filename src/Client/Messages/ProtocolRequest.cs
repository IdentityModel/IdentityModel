// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IdentityModel.Client;

/// <summary>
/// Models a base OAuth/OIDC request with client credentials
/// </summary>
public class ProtocolRequest : HttpRequestMessage
{
    /// <summary>
    /// Initializes an the HTTP protocol request and sets the accept header to application/json
    /// </summary>
    public ProtocolRequest()
    {
        Headers.Accept.Clear();
        Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Gets or sets the endpoint address (you can also set the RequestUri instead or leave blank to use the HttpClient base address).
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>
    /// The client secret.
    /// </value>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the client assertion.
    /// </summary>
    /// <value>
    /// The assertion.
    /// </value>
    public ClientAssertion ClientAssertion { get; set; } = new();

    /// <summary>
    /// Gets or sets the client credential style (post body vs authorization header).
    /// </summary>
    /// <value>
    /// The client credential style.
    /// </value>
    public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.AuthorizationHeader;

    /// <summary>
    /// Gets or sets the basic authentication header style (classic HTTP vs OAuth 2).
    /// </summary>
    /// <value>
    /// The basic authentication header style.
    /// </value>
    public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

    /// <summary>
    /// The DPoP proof token to use on the token endpoint.
    /// </summary>
    public string? DPoPProofToken { get; set; }

    /// <summary>
    /// Gets or sets additional protocol parameters.
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    public Parameters Parameters { get; set; } = new Parameters();

    /// <summary>
    /// Clones this instance.
    /// </summary>
    public ProtocolRequest Clone()
    {
        return Clone<ProtocolRequest>();
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    public T Clone<T>()
        where T: ProtocolRequest, new()
    {
        var clone = new T
        {
            RequestUri = RequestUri,
            Version = Version,
            Method = Method,

            Address = Address,
            AuthorizationHeaderStyle = AuthorizationHeaderStyle,
            ClientAssertion = ClientAssertion,
            ClientCredentialStyle = ClientCredentialStyle,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            DPoPProofToken = DPoPProofToken,
            Parameters = new Parameters()
        };

        if (Parameters != null)
        {
            foreach (var item in Parameters) clone.Parameters.Add(item);
        }

        clone.Headers.Clear();
        foreach (var header in Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

#if NET5_0_OR_GREATER
        if (Options.Any())
        {
            foreach (var property in Options)
            {
                clone.Options.TryAdd(property.Key, property.Value);
            }
        }          
#else
            if (Properties != null && Properties.Any())
            {
                foreach (var property in Properties)
                {
                    clone.Properties.Add(property);
                }
            }
#endif
        return clone;
    }

    /// <summary>
    /// Applies protocol parameters to HTTP request
    /// </summary>
    public void Prepare()
    {
        if (ClientId.IsPresent())
        {
            if (ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
            {
                if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                {
                    this.SetBasicAuthenticationOAuth(ClientId, ClientSecret ?? "");
                }
                else if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                {
                    this.SetBasicAuthentication(ClientId, ClientSecret ?? "");
                }
                else
                {
                    throw new InvalidOperationException("Unsupported basic authentication header style");
                }
            }
            else if (ClientCredentialStyle == ClientCredentialStyle.PostBody)
            {
                Parameters.AddRequired(OidcConstants.TokenRequest.ClientId, ClientId);
                Parameters.AddOptional(OidcConstants.TokenRequest.ClientSecret, ClientSecret);
            }
            else
            {
                throw new InvalidOperationException("Unsupported client credential style");
            }
        }

        if (ClientAssertion?.Type != null && ClientAssertion.Value != null)
        {
            if (ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader && !string.IsNullOrEmpty(ClientId))
            {
                throw new InvalidOperationException(
                    "CredentialStyle.AuthorizationHeader and client assertions are not compatible");
            }
            
            Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertionType, ClientAssertion.Type);
            Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertion, ClientAssertion.Value);
        }

        if (Address!.IsPresent())
        {
            RequestUri = new Uri(Address!, UriKind.RelativeOrAbsolute);
        }

        if (DPoPProofToken!.IsPresent())
        {
            Headers.Add(OidcConstants.HttpHeaders.DPoP, DPoPProofToken);
        }

        if (Parameters.Any())
        {
            Content = new FormUrlEncodedContent(Parameters);
        }
    }
}