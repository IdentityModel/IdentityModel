using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for CIBA backchannel authentication
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class BackchannelAuthenticationRequest : ProtocolRequest
{
    /// <summary>
    /// REQUIRED. The scope of the access request as described by Section 3.3 of RFC6749.
    /// </summary>
    public string Scope { get; set; } = default!;

    /// <summary>
    /// REQUIRED if the Client is registered to use Ping or Push modes.
    /// It is a bearer token provided by the Client that will be used by the OpenID Provider to authenticate the callback request to the Client.
    /// The length of the token MUST NOT exceed 1024 characters and it MUST conform to the syntax for Bearer credentials as defined in Section 2.1 of RFC6750.
    /// </summary>
    public string? ClientNotificationToken { get; set; }

    /// <summary>
    /// OPTIONAL. Requested Authentication Context Class Reference values.
    /// A space-separated string that specifies the acr values that the OpenID Provider is being requested to use for processing this Authentication Request, with the values appearing in order of preference.
    /// </summary>
    public string? AcrValues { get; set; }

    /// <summary>
    /// OPTIONAL. A token containing information identifying the end-user for whom authentication is being requested.
    /// </summary>
    public string? LoginHintToken { get; set; }

    /// <summary>
    /// OPTIONAL. An ID Token previously issued to the Client by the OpenID Provider being passed back as a hint to identify the end-user for whom authentication is being requested.
    /// </summary>
    public string? IdTokenHint { get; set; }

    /// <summary>
    /// OPTIONAL. A hint to the OpenID Provider regarding the end-user for whom authentication is being requested.
    /// </summary>
    public string? LoginHint { get; set; }

    /// <summary>
    /// OPTIONAL. A human-readable identifier or message intended to be displayed on both the consumption device and the authentication device to interlock them together for the transaction by way of a visual cue for the end-user.
    /// </summary>
    public string? BindingMessage { get; set; }

    /// <summary>
    /// OPTIONAL. A secret code, such as a password or pin, that is known only to the user but verifiable by the OP.
    /// </summary>
    public string? UserCode { get; set; }

    /// <summary>
    /// OPTIONAL. A positive integer allowing the client to request the expires_in value for the auth_req_id the server will return.
    /// </summary>
    public int? RequestedExpiry { get; set; }

    /// <summary>
    /// OPTIONAL. A signed authentication request is made by encoding all of the authentication request parameters as claims of a signed JWT with each parameter name as the claim name and its value as a JSON string.
    /// </summary>
    public string? RequestObject { get; set; }
    
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The resources.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}