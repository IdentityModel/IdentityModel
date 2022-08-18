using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using password
/// </summary>
/// <seealso cref="TokenRequest" />
public class PasswordTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string? Password { get; set; }

    /// <summary>
    /// Space separated list of the requested scopes
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string? Scope { get; set; }
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}