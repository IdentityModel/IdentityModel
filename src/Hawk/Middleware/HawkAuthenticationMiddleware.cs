
using System.Threading.Tasks;
#if NETSTANDARD1_6
using Microsoft.AspNetCore.Http;
#elif NET452
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
#endif

namespace Hawk.Middleware
{
#if NETSTANDARD1_6
    public class HawkAuthenticationMiddleware
    {
        private readonly RequestDelegate next;

        public HawkAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
  
            return this.next(context);
        }

    }
#elif NET452
    public class HawkAuthenticationMiddleware : AuthenticationMiddleware<HawkAuthenticationOptions>
    {
        public HawkAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, HawkAuthenticationOptions options)
            : base(next, options)
        { }

        protected override AuthenticationHandler<HawkAuthenticationOptions> CreateHandler()
        {
            return new HawkAuthenticationHandler();
        }
    }
#endif
}
