namespace IdentityModel.HttpClientExtensions
{
    public enum ClientCredentialStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        AuthorizationHeader,

        /// <summary>
        /// post values in body
        /// </summary>
        PostBody,
    };
}