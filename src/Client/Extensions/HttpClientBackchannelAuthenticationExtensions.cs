// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// HttpClient extensions CIBA backchannel authentication
/// </summary>
public static class HttpClientBackchannelAuthenticationExtensions
{
    /// <summary>
    /// Sends a CIBA backchannel authentication request
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task<BackchannelAuthenticationResponse> RequestBackchannelAuthenticationAsync(this HttpMessageInvoker client, BackchannelAuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        if (!string.IsNullOrWhiteSpace(request.RequestObject))
        {
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.Request, request.RequestObject);
        }
        else
        {
            clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.Scope, request.Scope);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.ClientNotificationToken, request.ClientNotificationToken);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.AcrValues, request.AcrValues);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.LoginHintToken, request.LoginHintToken);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.LoginHint, request.LoginHint);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.IdTokenHint, request.IdTokenHint);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.BindingMessage, request.BindingMessage);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.UserCode, request.UserCode);
        
            if (request.RequestedExpiry.HasValue)
            {
                clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.RequestedExpiry, request.RequestedExpiry.ToString());    
            }  
            
            foreach (var resource in request.Resource)
            {
                clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, allowDuplicates: true);
            }
        }
        
        clone.Method = HttpMethod.Post;
        clone.Prepare();
                        
        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<BackchannelAuthenticationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<BackchannelAuthenticationResponse>(response).ConfigureAwait();
    }
}