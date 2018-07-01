using IdentityModel.Client;
using IdentityModel.HttpClientExtensions;
using System;
using System.Net.Http;

namespace IdentityModel.Internal
{
    internal static class ClientCredentialsHelper
    {
        internal static void PopulateClientCredentials(
            Request request,
            HttpRequestMessage httpRequest)
        {
            if (request.ClientId.IsPresent())
            {
                if (request.ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
                {
                    if (request.BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                    {
                        httpRequest.SetBasicAuthenticationOAuth(request.ClientId, request.ClientSecret ?? "");
                    }
                    else if (request.BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                    {
                        httpRequest.SetBasicAuthentication(request.ClientId, request.ClientSecret ?? "");
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported basic authentication header style");
                    }
                }
                else if (request.ClientCredentialStyle == ClientCredentialStyle.PostBody)
                {
                    request.Parameters.Add(OidcConstants.TokenRequest.ClientId, request.ClientId);
                    request.Parameters.AddOptional(OidcConstants.TokenRequest.ClientSecret, request.ClientSecret);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported client credential style");
                }
            }

            if (request.Assertion.Type.IsPresent() && request.Assertion.Value.IsPresent())
            {
                request.Parameters.Add(OidcConstants.TokenRequest.ClientAssertionType, request.Assertion.Type);
                request.Parameters.Add(OidcConstants.TokenRequest.ClientAssertion, request.Assertion.Value);
            }
        }
    }
}
