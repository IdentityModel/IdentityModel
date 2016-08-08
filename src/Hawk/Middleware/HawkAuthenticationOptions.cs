#if NET452
using Microsoft.Owin.Security;
#endif
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;

namespace Hawk.Middleware
{
#if NETSTANDARD1_6
#elif NET452
    public class HawkAuthenticationOptions : AuthenticationOptions
    {
        public HawkAuthenticationOptions(Options hawkOptions)
            : base(HawkConstants.Scheme)
        {
            this.HawkOptions = hawkOptions;
        }

        public Options HawkOptions { get; set; }
    }
#endif
}
