using System;

namespace IdentityModel.Client
{
    /// <summary>
    /// Event argument with the refreshed token
    /// </summary>
    public class TokenRenewedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRenewedEventArgs" /> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expiresIn">The expires in.</param>
        public TokenRenewedEventArgs(string accessToken, int expiresIn)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public int ExpiresIn { get; }
    }
}