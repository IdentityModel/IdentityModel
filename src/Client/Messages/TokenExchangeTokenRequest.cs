namespace IdentityModel.Client;

/// <summary>
/// Request for token using urn:ietf:params:oauth:grant-type:token-exchange
/// </summary>
/// <seealso cref="TokenRequest" />
public class TokenExchangeTokenRequest : TokenRequest
{
    /// <summary>
    /// OPTIONAL.  A URI that indicates the target service or resource.
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// OPTIONAL.  The logical name of the target service where the client intends to use the requested security token.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// OPTIONAL. Space separated list of the requested scopes
    /// </summary>
    public string? Scope { get; set; }
        
    /// <summary>
    /// OPTIONAL.  An identifier for the type of the requested security token.
    /// </summary>
    public string? RequestedTokenType { get; set; }

    /// <summary>
    /// REQUIRED.  A security token that represents the identity of the party on behalf of whom the request is being made.
    /// </summary>
    public string SubjectToken { get; set; } = default!;

    /// <summary>
    /// REQUIRED.  An identifier that indicates the type of the security token in the "subject_token" parameter.
    /// </summary>
    public string SubjectTokenType { get; set; } = default!;

    /// <summary>
    /// OPTIONAL.  A security token that represents the identity of the acting party.
    /// </summary>
    public string? ActorToken { get; set; }

    /// <summary>
    /// An identifier that indicates the type of the security token in the "actor_token" parameter. This is REQUIRED when the "actor_token" parameter is present in the request but MUST NOT be included otherwise.
    /// </summary>
    public string? ActorTokenType { get; set; }
}