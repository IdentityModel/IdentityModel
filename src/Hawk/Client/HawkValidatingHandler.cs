using System;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Etw;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;
using Thinktecture.IdentityModel.Hawk.WebApi;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;

namespace Thinktecture.IdentityModel.Hawk.Client
{
    /// <summary>
    /// The client side message handler that adds the Authorization header to the request and validates the response.
    /// </summary>
    public class HawkValidationHandler : DelegatingHandler
    {
        private readonly ClientOptions options = null;

        /// <summary>
        /// The client side message handler that adds the Authorization header to the request and validates the response.
        /// </summary>
        /// <param name="options">Hawk authenitcation options</param>
        public HawkValidationHandler(ClientOptions options)
        {
            if (options == null || options.CredentialsCallback == null)
                throw new ArgumentNullException("Invalid Hawk authentication options. Credentials callback cannot be null.");

            this.options = options;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var client = new HawkClient(options);
            await client.CreateClientAuthorizationAsync(new WebApiRequestMessage(request));

            var response = await base.SendAsync(request, cancellationToken);
            var responseMessage = new WebApiResponseMessage(response);

            HawkEventSource.Log.Debug(String.Format("Response received with status of {0}", (int)responseMessage.StatusCode));

            if (!await client.AuthenticateAsync(responseMessage))
            {
                string header = responseMessage.Headers.FirstOrDefault(HawkConstants.ServerAuthorizationHeaderName);
                HawkEventSource.Log.ServerResponse((int)responseMessage.StatusCode, await responseMessage.ReadBodyAsStringAsync(), header ?? String.Empty);

                throw new SecurityException("Invalid Mac and/or hash. Response possibly tampered.");
            }

            return response;
        }
    }
}