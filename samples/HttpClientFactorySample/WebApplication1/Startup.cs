using System;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpClient();
            services.AddHttpClient("token_client",
                client => client.BaseAddress = new Uri("https://demo.identityserver.io/connect/token"));

            services.Configure<TokenClientOptions>(options =>
            {
                options.Address = "https://demo.identityserver.io/connect/token";
                options.ClientId = "client";
                options.ClientSecret = "secret";
            });

            services.AddHttpClient<TokenClient>()
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.AddDemoIdentityServerClient(
                new Uri("https://demo.identityserver.io/"),
                new ClientCredentialsProvider("https://demo.identityserver.io/connect/token", "client", "secret", "api"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    internal static class IdentityModelClientExtensions
    {
        public static IServiceCollection AddDemoIdentityServerClient(this IServiceCollection services, Uri address, IAuthenticationProvider authenticationProvider, bool sharedAuthenticationCache = true)
        {
            if (sharedAuthenticationCache)
            {
                services
                    .AddHttpClientAuthentication(sp => authenticationProvider)
                    .AddHttpClient<DemoIdentityServerClient>(client => client.BaseAddress = address)
                    .AddAuthenticationHandler();
            }
            else
            {
                var authenticationCache = new AuthenticationCache(authenticationProvider);
                services
                    .AddHttpClient<DemoIdentityServerClient>(client => client.BaseAddress = address)
                    .AddAuthenticationHandler(sp => authenticationCache);
            }
            return services;
        }

        public static IServiceCollection AddHttpClientAuthentication(this IServiceCollection services, Func<IServiceProvider, IAuthenticationProvider> authenticationProviderResolver)
        {
            services.TryAddSingleton<IAuthenticationProvider>(authenticationProviderResolver);
            services.TryAddSingleton<IAuthenticationCache, AuthenticationCache>();
            services.TryAddTransient<AuthenticateDelegatingHandler>();
            return services;
        }

        public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof (builder));
            builder.AddHttpMessageHandler<AuthenticateDelegatingHandler>();
            return builder;
        }

        public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, Func<IServiceProvider, IAuthenticationCache> authenticationCacheResolver)
        {
            if (builder == null) throw new ArgumentNullException(nameof (builder));
            builder.AddHttpMessageHandler(sp => new AuthenticateDelegatingHandler(authenticationCacheResolver.Invoke(sp)));
            return builder;
        }
    }
}
