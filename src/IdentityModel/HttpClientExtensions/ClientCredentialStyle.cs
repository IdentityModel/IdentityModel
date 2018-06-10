namespace IdentityModel.HttpClientExtensions
{
    public enum ClientCredentialStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        AuthorizationHeader,

        /// <summary>
        /// Post values in body
        /// </summary>
        PostBody,

        /// <summary>
        /// Put credentials on query string
        /// </summary>
        QueryString,
    };
}