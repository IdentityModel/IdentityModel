using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class HttpResponseMessageMock : HttpResponseMessage
    {
        public HttpResponseMessageMock(HttpStatusCode code) : base(code)
        {
        }

        public bool Disposed { get; set; }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    public class StubHttpResponsesHandler : DelegatingHandler
    {
        private bool _first401ResponseSent;

        public StubHttpResponsesHandler()
        {
            FirstAttempt401Response = new HttpResponseMessageMock(HttpStatusCode.Unauthorized);
        }

        public HttpResponseMessageMock FirstAttempt401Response { get; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (false == _first401ResponseSent)
            {
                _first401ResponseSent = true;
                return Task.FromResult(FirstAttempt401Response as HttpResponseMessage);
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    public class RefreshTokenHandlerTests
    {
        [Fact]
        public async Task The_401_response_that_causes_token_refresh_and_retry_should_be_disposed_to_unblock_socket()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var tokenClient = new TokenClient(
                "http://server/token",
                "client",
                handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync();

            var indirectOutputOfHttpResponses = new StubHttpResponsesHandler();
            var refreshHandler = new RefreshTokenHandler(
                tokenClient,
                tokenResponse.RefreshToken,
                tokenResponse.AccessToken,
                indirectOutputOfHttpResponses);

            var apiClient = new HttpClient(refreshHandler);

            await apiClient.GetStringAsync("http://someapi/somecall");

            indirectOutputOfHttpResponses.FirstAttempt401Response
                .Disposed
                .Should()
                .BeTrue("Unauthorized response should be disposed to avoid socket blocking");
        }
    }
}