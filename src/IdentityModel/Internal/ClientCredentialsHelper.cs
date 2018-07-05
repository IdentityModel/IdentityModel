using IdentityModel.Client;
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
                    if (request.AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                    {
                        httpRequest.SetBasicAuthenticationOAuth(request.ClientId, request.ClientSecret ?? "");
                    }
                    else if (request.AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
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
                    request.Parameters.AddRequired(OidcConstants.TokenRequest.ClientId, request.ClientId);
                    request.Parameters.AddOptional(OidcConstants.TokenRequest.ClientSecret, request.ClientSecret);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported client credential style");
                }
            }

            request.Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertionType, request.ClientAssertion.Type);
            request.Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertion, request.ClientAssertion.Value);
        }
    }
}