using IdentityModel.Client;
using IdentityModel.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public static class HttpClientTokenExtensions
    {
        public static async Task<TokenResponse> RequestTokenAsync(this HttpClient client, TokenRequest request, CancellationToken cancellationToken = default)
        {
            IDictionary<string, string> form = new Dictionary<string, string>();

            if (request.Parameters != null)
            {
                form = ValuesHelper.ObjectToDictionary(request.Parameters);
            }
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address);

            if (request.ClientId.IsPresent())
            {
                if (request.CredentialStyle == CredentialStyle.AuthorizationHeader)
                {
                    httpRequest.SetBasicAuthenticationOAuth(request.ClientId, request?.ClientSecret ?? "");
                }
                else if (request.CredentialStyle == CredentialStyle.PostBody)
                {
                    form.AddIfPresent("client_id", request.ClientId);

                    if (request.ClientSecret.IsPresent())
                    {
                        form.AddIfPresent("client_secret", request.ClientSecret);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid credential style");
                }
            }

            form.AddIfPresent("scope", request.Scope);

            httpRequest.Content = new FormUrlEncodedContent(form);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new TokenResponse(ex);
            }

            string content = null;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new TokenResponse(content);
            }
            else
            {
                return new TokenResponse(response.StatusCode, response.ReasonPhrase, content);
            }
        }
    }
}