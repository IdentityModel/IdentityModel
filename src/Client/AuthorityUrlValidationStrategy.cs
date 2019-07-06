using IdentityModel.Internal;
using System;
using System.Collections.Generic;

namespace IdentityModel.Client
{
    /// <summary>
    /// Implementation of <see cref="IAuthorityValidationStrategy"/> based on <see cref="Uri"/> equality.
    /// </summary>
    public sealed class AuthorityUrlValidationStrategy : IAuthorityValidationStrategy
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static AuthorityUrlValidationStrategy Instance = new AuthorityUrlValidationStrategy();

        private AuthorityUrlValidationStrategy()
        {
            
        }

        public AuthorityValidationResult IsIssuerNameValid(string issuerName, string expectedAuthority)
        {
            if (!Uri.TryCreate(expectedAuthority.RemoveTrailingSlash(), UriKind.Absolute, out var expectedAuthorityUrl))
                throw new ArgumentOutOfRangeException("Authority must be a valid URL.", nameof(expectedAuthority));

            if (string.IsNullOrWhiteSpace(issuerName)) return AuthorityValidationResult.CreateError("Issuer name is missing");

            if (!Uri.TryCreate(issuerName.RemoveTrailingSlash(), UriKind.Absolute, out var issuerUrl))
                return AuthorityValidationResult.CreateError("Issuer name is not a valid URL");

            if (expectedAuthorityUrl.Equals(issuerUrl))
                return AuthorityValidationResult.SuccessResult;

            return AuthorityValidationResult.CreateError("Issuer name does not match authority: " + issuerName);
        }

        public AuthorityValidationResult IsEndpointValid(string endpoint, IEnumerable<string> allowedAuthorities)
        {
            if (string.IsNullOrEmpty(endpoint))
                return AuthorityValidationResult.CreateError("endpoint is empty");

            if (!Uri.TryCreate(endpoint.RemoveTrailingSlash(), UriKind.Absolute, out var endpointUrl))
                return AuthorityValidationResult.CreateError("Endpoint is not a valid URL");

            foreach (string authority in allowedAuthorities)
            {
                if (!Uri.TryCreate(endpoint.RemoveTrailingSlash(), UriKind.Absolute, out var authorityUrl))
                    throw new ArgumentOutOfRangeException("Authority must be a URL.", nameof(allowedAuthorities));

                string expectedString = authorityUrl.ToString();
                string testString = endpointUrl.ToString();

                if (testString.StartsWith(expectedString, StringComparison.Ordinal))
                    return AuthorityValidationResult.SuccessResult;

            }

            return AuthorityValidationResult.CreateError($"Endpoint belongs to different authority: {endpoint}");
        }
    }
}
