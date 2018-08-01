using FluentAssertions;
using IdentityModel.Client;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryCacheTests
    {
        NetworkHandler _successHandler;
        string _authority = "https://demo.identityserver.io";

        public DiscoveryCacheTests()
        {
            var discoFileName = FileName.Create("discovery.json");
            var document = File.ReadAllText(discoFileName);

            var jwksFileName = FileName.Create("discovery_jwks.json");
            var jwks = File.ReadAllText(jwksFileName);

            _successHandler = new NetworkHandler(request =>
            {
                if (request.RequestUri.AbsoluteUri.EndsWith("jwks"))
                {
                    return jwks;
                }

                return document;
            }, HttpStatusCode.OK);
        }

        [Fact]
        public async Task New_initialization_should_work()
        {
            var client = new HttpClient(_successHandler);
            var cache = new DiscoveryCache(_authority, client);

            var disco = await cache.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Old_initialization_should_work()
        {
            var client = new DiscoveryClient(_authority, _successHandler);
            var cache = new DiscoveryCache(client);

            var disco = await cache.GetAsync();

            disco.IsError.Should().BeFalse();
        }
    }
}
