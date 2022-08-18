using System.Text.Json;

namespace IdentityModel.Client;

/// <summary>
/// MTLS endpoint aliases
/// </summary>
public class MtlsEndpointAliases
{
    /// <summary>
    /// The raw JSON
    /// </summary>
    public JsonElement Json { get; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="json"></param>
    public MtlsEndpointAliases(JsonElement json)
    {
        Json = json;
    }

    /// <summary>
    /// Returns the token endpoint address
    /// </summary>
    public string? TokenEndpoint => Json.TryGetString(OidcConstants.Discovery.TokenEndpoint);
        
    /// <summary>
    /// Returns the revocation endpoint address
    /// </summary>
    public string? RevocationEndpoint => Json.TryGetString(OidcConstants.Discovery.RevocationEndpoint);
        
    /// <summary>
    /// Returns the device authorization endpoint address
    /// </summary>
    public string? DeviceAuthorizationEndpoint => Json.TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);
        
    /// <summary>
    /// Returns the introspection endpoint address
    /// </summary>
    public string? IntrospectionEndpoint => Json.TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);
        
}