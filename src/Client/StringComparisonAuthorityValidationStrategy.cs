using IdentityModel.Internal;
using System;
using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Implementation of <see cref="IAuthorityValidationStrategy"/> based on <see cref="StringComparison"/>.
/// </summary>
/// <seealso cref="AuthorityUrlValidationStrategy"/>
public sealed class StringComparisonAuthorityValidationStrategy : IAuthorityValidationStrategy
{
    private readonly StringComparison _stringComparison;

    /// <summary>
    /// Constructor with <see cref="StringComparison"/> argument.
    /// </summary>
    /// <param name="stringComparison"></param>
    public StringComparisonAuthorityValidationStrategy(StringComparison stringComparison = StringComparison.Ordinal)
    {
        _stringComparison = stringComparison;
    }

    /// <summary>
    /// String comparison between issuer and authority (trailing slash ignored).
    /// </summary>
    /// <param name="issuerName"></param>
    /// <param name="expectedAuthority"></param>
    /// <returns></returns>
    public AuthorityValidationResult IsIssuerNameValid(string issuerName, string expectedAuthority)
    {
        if (string.IsNullOrWhiteSpace(issuerName)) return AuthorityValidationResult.CreateError("Issuer name is missing");

        if (string.Equals(issuerName.RemoveTrailingSlash(), expectedAuthority.RemoveTrailingSlash(), _stringComparison))
            return AuthorityValidationResult.SuccessResult;

        return AuthorityValidationResult.CreateError("Issuer name does not match authority: " + issuerName);
    }

    /// <summary>
    /// String "starts with" comparison between endpoint and allowed authorities.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="allowedAuthorities"></param>
    /// <returns></returns>
    public AuthorityValidationResult IsEndpointValid(string endpoint, IEnumerable<string> allowedAuthorities)
    {
        if (string.IsNullOrEmpty(endpoint))
            return AuthorityValidationResult.CreateError("endpoint is empty");

        foreach (string authority in allowedAuthorities)
        {
            if (endpoint.StartsWith(authority, _stringComparison))
                return AuthorityValidationResult.SuccessResult;
        }

        return AuthorityValidationResult.CreateError($"Endpoint belongs to different authority: {endpoint}");
    }
}