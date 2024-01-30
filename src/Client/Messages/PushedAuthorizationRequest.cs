namespace IdentityModel.Client;

/// <summary>
/// Request for PAR
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class PushedAuthorizationRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the response_type protocol parameter.
    /// </summary>
    public string? ResponseType { get; set; } = default!;

    /// <summary>
    /// Gets or sets the scope protocol parameter.
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// The redirect_uri
    /// </summary>
    public string? RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the state protocol parameter.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the nonce protocol parameter.
    /// </summary>
    public string? Nonce { get; set; }

    /// <summary>
    /// Gets or sets the login_hint protocol parameter.
    /// </summary>
    public string? LoginHint { get; set; }

    /// <summary>
    /// Gets or sets the acr_values protocol parameter.
    /// </summary>
    public string? AcrValues { get; set; }

    /// <summary>
    /// Gets or sets the prompt protocol parameter.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets the response_mode protocol parameter.
    /// </summary>
    public string? ResponseMode { get; set; }

    /// <summary>
    /// Gets or sets the code_challenge protocol parameter.
    /// </summary>
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// Gets or sets the code_challenge_method protocol parameter.
    /// </summary>
    public string? CodeChallengeMethod { get; set; }

    /// <summary>
    /// Gets or sets the display protocol parameter.
    /// </summary>
    public string? Display { get; set; }

    /// <summary>
    /// Gets or sets the max_age protocol parameter.
    /// </summary>
    public int? MaxAge { get; set; }

    /// <summary>
    /// Gets or sets the ui_locales protocol parameter.
    /// </summary>
    public string? UiLocales { get; set; }

    /// <summary>
    /// Gets or sets the id_token_hint protocol parameter.
    /// </summary>
    public string? IdTokenHint { get; set; }
    
    // TODO - Do we want JAR related properties?
 }