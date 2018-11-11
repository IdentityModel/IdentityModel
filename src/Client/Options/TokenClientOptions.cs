namespace IdentityModel.Client
{
    public class TokenClientOptions
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the client assertion.
        /// </summary>
        /// <value>
        /// The assertion.
        /// </value>
        public ClientAssertion ClientAssertion { get; set; } = new ClientAssertion();

        /// <summary>
        /// Gets or sets the client credential style.
        /// </summary>
        /// <value>
        /// The client credential style.
        /// </value>
        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

        /// <summary>
        /// Gets or sets the basic authentication header style.
        /// </summary>
        /// <value>
        /// The basic authentication header style.
        /// </value>
        public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;
    }
}