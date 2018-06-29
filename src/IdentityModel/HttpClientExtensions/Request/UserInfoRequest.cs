namespace IdentityModel.HttpClientExtensions
{
    public class UserInfoRequest
    {
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
    }
}
