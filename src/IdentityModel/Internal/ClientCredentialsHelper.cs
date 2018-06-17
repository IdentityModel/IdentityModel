using IdentityModel.Client;
using IdentityModel.HttpClientExtensions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityModel.Internal
{
    internal static class ClientCredentialsHelper
    {
        internal static void PopulateClientCredentials(
            string clientId,
            string clientSecret,
            ClientCredentialStyle credentialStyle,
            BasicAuthenticationHeaderStyle headerStyle,
            HttpRequestMessage httpRequest,
            IDictionary<string, string> parameters)
        {
            if (clientId.IsPresent())
            {
                if (credentialStyle == ClientCredentialStyle.AuthorizationHeader)
                {
                    if (headerStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                    {
                        httpRequest.SetBasicAuthenticationOAuth(clientId, clientSecret ?? "");
                    }
                    else if (headerStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                    {
                        httpRequest.SetBasicAuthentication(clientId, clientSecret ?? "");
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported basic authentication header style");
                    }
                }
                else if (credentialStyle == ClientCredentialStyle.PostBody)
                {
                    parameters.Add(OidcConstants.TokenRequest.ClientId, clientId);
                    parameters.AddOptional(OidcConstants.TokenRequest.ClientSecret, clientSecret);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported client credential style");
                }
            }
        }
    }
}
