#if NET452
using Microsoft.Owin;
#endif
using System;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;

namespace Hawk.Middleware.Extensions
{
#if NET452
    internal static class OwinRequestExtension
    {
        internal static bool IsPayloadHashPresent(this IOwinRequest request)
        {
            string authorization = request.Headers.Get(HawkConstants.AuthorizationHeaderName);

            if (!String.IsNullOrWhiteSpace(authorization))
            {
                string parameter = AuthenticationHeaderValue.Parse(authorization).Parameter;
                return ArtifactsContainer.IsPayloadHashPresent(parameter);
            }

            return false;
        }
    }
#endif
}
