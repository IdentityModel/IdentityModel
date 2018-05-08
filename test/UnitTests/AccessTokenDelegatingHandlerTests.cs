using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AccessTokenDelegatingHandlerTests
    {
        [Fact]
        public async Task The_401_response_that_causes_token_refresh_and_retry_should_be_disposed_to_unblock_socket()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            using (var tokenClient = new TokenClient(
                "http://server/token",
                "client",
                handler))
            {
                var indirectOutputOfHttpResponses = new StubHttpResponsesHandler();
                var accessTokenHandler = new AccessTokenDelegatingHandler(
                    tokenClient,
                    "scope",
                    innerHandler: indirectOutputOfHttpResponses);

                var apiClient = new HttpClient(accessTokenHandler);

                await apiClient.GetStringAsync("http://someapi/somecall");

                indirectOutputOfHttpResponses.FirstAttempt401Response
                    .Disposed
                    .Should()
                    .BeTrue("Unauthorized response should be disposed to avoid socket blocking");
            }
        }
    }
}