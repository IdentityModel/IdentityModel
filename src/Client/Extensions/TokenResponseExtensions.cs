using System.Net.Http;
using System.Runtime.CompilerServices;

namespace IdentityModel.Client.Extensions
{
    /// <summary>
    /// Extensions for TokenResponse.
    /// </summary>
    public static class TokenResponseExtensions
    {
        /// <summary>
        /// Throws an exception if the IsError property for the Token response is true.
        /// </summary>
        /// <param name="tokenResponse">Token Response to validate.</param>
        /// <exception cref="HttpRequestException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureSuccessResponse(this TokenResponse tokenResponse)
        {
            if (tokenResponse.IsError)
                throw new HttpRequestException($"Token Request was not successful: ({tokenResponse.HttpStatusCode}) {tokenResponse.Error}", tokenResponse.Exception);
        }
    }
}
