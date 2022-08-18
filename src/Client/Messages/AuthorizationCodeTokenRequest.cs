using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using authorization_code
/// </summary>
/// <seealso cref="TokenRequest" />
public class AuthorizationCodeTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>
    /// The code.
    /// </value>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    /// <value>
    /// The redirect URI.
    /// </value>
    public string RedirectUri { get; set; } = default!;
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();

    /// <summary>
    /// Gets or sets the code verifier.
    /// </summary>
    /// <value>
    /// The code verifier.
    /// </value>
    public string? CodeVerifier { get; set; }
}