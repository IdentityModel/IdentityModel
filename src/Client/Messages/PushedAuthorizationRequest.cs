// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Models the parameters that can be pushed in a Pushed Authorization Request.
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class PushedAuthorizationRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the response_type protocol parameter.
    /// </summary>
    public string? ResponseType { get; set; }
    
    /// <summary>
    /// Gets or sets the scope protocol parameter.
    /// </summary>
    public string? Scope { get; set; }
    
    /// <summary>
    /// Gets or sets the redirect_uri protocol parameter.
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
    
    /// <summary>
    /// Gets or sets the resource protocol parameter.
    /// </summary>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
    
    /// <summary>
    /// Gets or sets the dpop_jkt protocol parameter.
    /// </summary>
    public string? DPoPKeyThumbprint { get; set; }
    
    /// <summary>
    /// Gets or sets the request protocol parameter.
    /// </summary>
    public string? Request { get; set; }

    
    /// <summary>
    /// Copies properties from a request into a Parameters collection. 
    /// </summary>
    /// <param name="targetParameters">The parameters to copy into.</param>
    public Parameters MergeInto(Parameters targetParameters)
    {
        
        return targetParameters;
    }
}
