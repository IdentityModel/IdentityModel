using System;

namespace IdentityModel.Client
{
    /// <summary>
    /// Event argument with the refreshed token
    /// </summary>
    public class TokenRefreshedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRefreshedEventArgs"/> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        public TokenRefreshedEventArgs(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; }
    }
}