using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using client_credentials
/// </summary>
/// <seealso cref="TokenRequest" />
public class ClientCredentialsTokenRequest : TokenRequest
{
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