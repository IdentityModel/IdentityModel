using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
#if NETSTANDARD1_6
using Microsoft.AspNetCore.Http.Authentication;
#elif NET452
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security;
#endif
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Etw;
using Hawk.Middleware.Extensions;

namespace Hawk.Middleware
{
#if NETSTANDARD1_6
#elif NET452
    public class HawkAuthenticationHandler : AuthenticationHandler<HawkAuthenticationOptions>
    {
        private HawkServer server = null;
        private Stream stream = null;
        private MemoryStream requestBuffer = null, responseBuffer = null;

        protected async override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                if (Request.IsPayloadHashPresent())
                {
                    // buffer the request body
                    requestBuffer = new MemoryStream();
                    await Request.Body.CopyToAsync(requestBuffer);
                    Request.Body = requestBuffer;
                }

                IRequestMessage requestMessage = new OwinRequestMessage(Request);

                server = new HawkServer(requestMessage, Options.HawkOptions);

                var principal = await server.AuthenticateAsync();

                if (principal != null && principal.Identity.IsAuthenticated)
                {
                    if (!server.IsBewitRequest) // Bewit means no server authorization and hence no need for buffering.
                    {
                        var callback = Options.HawkOptions.ResponsePayloadHashabilityCallback;

                        if (callback != null && callback(requestMessage)) // buffer the response body
                        {
                            stream = Response.Body;
                            responseBuffer = new MemoryStream();
                            Response.Body = responseBuffer;

                            HawkEventSource.Log.Debug("Response Body Buffered");
                        }
                    }

                    return new AuthenticationTicket(principal.Identity as ClaimsIdentity, (AuthenticationProperties)null);
                }
            }
            catch (Exception exception)
            {
                HawkEventSource.Log.Exception(exception.ToString());

                if (responseBuffer != null)
                {
                    Response.Body = this.stream;
                }

                throw;
            }

            return new AuthenticationTicket(null, (AuthenticationProperties)null);
        }

        protected override async Task TeardownCoreAsync()
        {
            if (responseBuffer != null)
            {
                responseBuffer.Seek(0, SeekOrigin.Begin);
                await responseBuffer.CopyToAsync(stream); // Write buffer onto the stream ...

                // ... and switch back to the original stream
                Response.Body = this.stream;

                HawkEventSource.Log.Debug("Response Body UnBuffered");
            }
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            // In case of 401, we do not add WWW-Authenticate, if authentication mode is passive.
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                if (challenge == null)
                {
                    return;
                }
            }

            IResponseMessage responseMessage = new OwinResponseMessage(Response);

            var header = await server.CreateServerAuthorizationAsync(responseMessage);

            if (header != null)
                responseMessage.AddHeader(header.Item1, header.Item2);
        }
    }
#endif
}
