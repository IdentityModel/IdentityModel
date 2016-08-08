using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Etw;

namespace Thinktecture.IdentityModel.Hawk.WebApi
{
#if NET452
    /// <summary>
    /// The message handler that performs the authentication based on the authenticity of the HMAC.
    /// Add a new instance of this handler to config.MessageHandlers in WebApiConfig.Register().
    /// </summary>
    public class HawkAuthenticationHandler : DelegatingHandler
    {
        private readonly Options options = null;

        /// <summary>
        /// The message handler that authenticates the request using Hawk.
        /// </summary>
        /// <param name="options">Hawk authentication options</param>
        public HawkAuthenticationHandler(Options options)
        {
            if (options == null || options.CredentialsCallback == null)
                throw new ArgumentNullException("Invalid Hawk authentication options. Credentials callback cannot be null.");

            this.options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
                                        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                HawkServer server = new HawkServer(new WebApiRequestMessage(request), options);

                var principal = await server.AuthenticateAsync();

                if (principal != null && principal.Identity.IsAuthenticated)
                {
                    request.GetRequestContext().Principal = principal;

                    HawkEventSource.Log.Debug("Authentication Successful and principal set for " + principal.Identity.Name);
                }

                var response = await base.SendAsync(request, cancellationToken);

                var header = await server.CreateServerAuthorizationAsync(new WebApiResponseMessage(response));
                if (header != null)
                    response.Headers.Add(header.Item1, header.Item2);

                return response;
            }
            catch (Exception exception)
            {
                HawkEventSource.Log.Exception(exception.ToString());

                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(HawkConstants.Scheme));

                return response;
            }
        }
    }
#endif
}