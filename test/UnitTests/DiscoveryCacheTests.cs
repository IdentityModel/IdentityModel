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
        private readonly NetworkHandler _successHandler;
        private const string _authority = "https://demo.identityserver.io";

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
            var cache = new DiscoveryCache(_authority, () => client);

            var disco = await cache.GetAsync();
            disco.IsError.Should().BeFalse();
        }
        
        [Fact]
        public async Task GetAsync_should_return_same_discovery_document_if_cached()
        {
            var client = new HttpClient(_successHandler);
            var cache = new DiscoveryCache(_authority, () => client);
            
            var disco = await cache.GetAsync();
            (await cache.GetAsync()).Should().BeSameAs(disco);
        }
        
        [Fact]
        public async Task Refresh_should_cause_subsequent_calls_to_return_new_discovery_document()
        {
            var client = new HttpClient(_successHandler);
            var cache = new DiscoveryCache(_authority, () => client);
            
            var disco = await cache.GetAsync();
            cache.Refresh();
            (await cache.GetAsync()).Should().NotBeSameAs(disco);
        }
        
        [Fact]
        public async Task Refresh_should_cause_authority_to_be_reevaluated_on_subsequent_calls()
        {
            var numberOfTimesCalled = 0;
            var client = new HttpClient(_successHandler);
            var cache = new DiscoveryCache(AuthorityFunc, () => client);
            
            _ = await cache.GetAsync();
            _ = await cache.GetAsync();
            cache.Refresh();
            _ = await cache.GetAsync();
            
            numberOfTimesCalled.Should().Be(2);
            
            string AuthorityFunc()
            {
                numberOfTimesCalled++;
                return _authority;
            }
        }
    }
}
