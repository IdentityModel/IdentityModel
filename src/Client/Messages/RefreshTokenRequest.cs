using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using refresh_token
/// </summary>
/// <seealso cref="TokenRequest" />
public class RefreshTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <value>
    /// The refresh token.
    /// </value>
    public string RefreshToken { get; set; } = default!;

    /// <summary>
    /// Space separated list of the requested scopes.  The Scope attribute cannot be used to extend the scopes granted by the resource owner
    /// </summary>
    /// <remarks>
    /// See https://datatracker.ietf.org/doc/html/rfc6749#section-6 for further detail on restrictions
    /// </remarks>
    /// <value>
    /// The scope.
    /// </value>
    public string? Scope { get; set; }

    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The resources.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}