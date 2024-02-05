using System.Collections.Generic;

public class AuthorizeRedirect : IAuthorizeRequest
{
    //
    // This parameter is only allowed when redirecting to the authorize endpoint
    // (it can't be pushed to the PAR endpoint)
    //    

    /// <summary>
    /// Gets or sets the request_uri protocol parameter.
    /// </summary>
    public string? RequestUri { get; set; }
    
    //
    // The rest of these properties are from IAuthorizeRequest (shared with PAR) 
    //

    /// <inheritdoc />
    public string ClientId { get; set; }
    /// <inheritdoc />
    public string ResponseType { get; set; }
    /// <inheritdoc />
    public string? Scope { get; set; }
    /// <inheritdoc />
    public string? RedirectUri { get; set; }
    /// <inheritdoc />
    public string? State { get; set; }
    /// <inheritdoc />
    public string? Nonce { get; set; }
    /// <inheritdoc />
    public string? LoginHint { get; set; }
    /// <inheritdoc />
    public string? AcrValues { get; set; }
    /// <inheritdoc />
    public string? Prompt { get; set; }
    /// <inheritdoc />
    public string? ResponseMode { get; set; }
    /// <inheritdoc />
    public string? CodeChallenge { get; set; }
    /// <inheritdoc />
    public string? CodeChallengeMethod { get; set; }
    /// <inheritdoc />
    public string? Display { get; set; }
    /// <inheritdoc />
    public int? MaxAge { get; set; }
    /// <inheritdoc />
    public string? UiLocales { get; set; }
    /// <inheritdoc />
    public string? IdTokenHint { get; set; }
    /// <inheritdoc />
    public string? Request { get; set; }
    /// <inheritdoc />
    public ICollection<string>? Resource { get; set; }
    /// <inheritdoc />
    public string? DPoPKeyThumbprint { get; set; }
}