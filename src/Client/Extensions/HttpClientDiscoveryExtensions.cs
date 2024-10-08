// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// HttpClient extentions for OIDC discovery
/// </summary>
public static class HttpClientDiscoveryExtensions
{
    /// <summary>
    /// Sends a discovery document request
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="address">The address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(this HttpClient client, string? address = null, CancellationToken cancellationToken = default)
    {
        if (address == null && client.BaseAddress == null)
            throw new ArgumentException("Either the address parameter or the HttpClient BaseAddress must not be null.");
        return await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest { Address = address }, cancellationToken).ConfigureAwait();
    }

    /// <summary>
    /// Sends a discovery document request
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(this HttpMessageInvoker client, DiscoveryDocumentRequest request, CancellationToken cancellationToken = default)
    {
        string address;
        if (request.Address.IsPresent())
        {
            address = request.Address!;
        }
        else if (client is HttpClient httpClient && httpClient.BaseAddress != null)
        {
            address = httpClient.BaseAddress!.GetLeftPart(UriPartial.Authority);
        }
        else
        {
            throw new ArgumentException("Either the DiscoveryDocumentRequest Address or the HttpClient BaseAddress must not be null.");
        }

        var parsed = DiscoveryEndpoint.ParseUrl(address, request.Policy.DiscoveryDocumentPath);
        var authority = parsed.Authority;
        var url = parsed.Url;

        if (request.Policy.Authority.IsMissing())
        {
            request.Policy.Authority = authority;
        }

        var jwkUrl = "";

        if (!DiscoveryEndpoint.IsSecureScheme(new Uri(url), request.Policy))
        {
            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(new InvalidOperationException("HTTPS required"), $"Error connecting to {url}. HTTPS required.");
        }

        try
        {
            var clone = request.Clone();

            clone.Method = HttpMethod.Get;
            clone.Prepare();

            clone.RequestUri = new Uri(url);

            var response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();

            if (!response.IsSuccessStatusCode)
            {
                return await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(response, $"Error connecting to {url}: {response.ReasonPhrase}").ConfigureAwait();
            }

            var disco = await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(response, request.Policy).ConfigureAwait();

            if (disco.IsError)
            {
                return disco;
            }

            try
            {
                jwkUrl = disco.JwksUri;
                if (jwkUrl != null)
                {
                    var jwkClone = request.Clone<JsonWebKeySetRequest>();
                    jwkClone.Method = HttpMethod.Get;
                    jwkClone.Address = jwkUrl;
                    jwkClone.Prepare();

                    var jwkResponse = await client.GetJsonWebKeySetAsync(jwkClone, cancellationToken).ConfigureAwait();

                    if (jwkResponse.IsError)
                    {
                        if (jwkResponse.Exception != null)
                        {
                            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(jwkResponse.Exception, jwkResponse.Error);
                        }
                        else if(jwkResponse.HttpResponse != null)
                        {
                            return await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(jwkResponse.HttpResponse, $"Error connecting to {jwkUrl}: {jwkResponse.HttpErrorReason}").ConfigureAwait();
                        }
                        // If IsError is true, but we have neither an Exception nor an HttpResponse, something very weird is going on
                        // I don't think it is actually possible for this to occur, but just in case...
                        else
                        {
                            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(
                                new ArgumentNullException(nameof(jwkResponse.HttpResponse)), $"Unknown error retrieving JWKS - neither an exception nor an HttpResponse is available");
                        }
                    }

                    disco.KeySet = jwkResponse.KeySet;
                }

                return disco;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<DiscoveryDocumentResponse>(ex, $"Error connecting to {jwkUrl}. {ex.Message}.");
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(ex, $"Error connecting to {url}. {ex.Message}.");
        }
    }
}