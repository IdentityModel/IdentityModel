using IdentityModel.Client;
using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public static class HttpClientTokenIntrospectionExtensions
    {
        public static async Task<IntrospectionResponse> IntrospectTokenAsync(this HttpClient client, TokenIntrospectionRequest request, CancellationToken cancellationToken = default)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address);
            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (request.ClientId.IsPresent())
            {
                if (request.ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
                {
                    if (request.BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                    {
                        httpRequest.SetBasicAuthenticationOAuth(request.ClientId, request?.ClientSecret ?? "");
                    }
                    else if (request.BasicAuthenticationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                    {
                        httpRequest.SetBasicAuthentication(request.ClientId, request?.ClientSecret ?? "");
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

            request.Parameters.AddRequired("token", request.Token);
            request.Parameters.AddOptional("token_type_hint", request.TokenTypeHint);

            httpRequest.Content = new FormUrlEncodedContent(request.Parameters);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new IntrospectionResponse(ex);
            }
            if (response.IsSuccessStatusCode)
            {
                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            else
            {
                return new IntrospectionResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
