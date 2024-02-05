namespace IdentityModel.Client;

/// <summary>
/// Options for TokenClient
/// </summary>
public class TokenClientOptions : ClientOptions
{ }

/// <summary>
/// Options for IntrospectionClient
/// </summary>
public class IntrospectionClientOptions : ClientOptions
{ }

/// <summary>
/// Base-class protocol client options
/// </summary>
public abstract class ClientOptions
{
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public string Address { get; set; } = default!;

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>
    /// The client secret.
    /// </value>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the client assertion.
    /// </summary>
    /// <value>
    /// The assertion.
    /// </value>
    public ClientAssertion? ClientAssertion { get; set; } = new();

    /// <summary>
    /// Gets or sets the client credential style.
    /// </summary>
    /// <value>
    /// The client credential style.
    /// </value>
    public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

    /// <summary>
    /// Gets or sets the basic authentication header style.
    /// </summary>
    /// <value>
    /// The basic authentication header style.
    /// </value>
    public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

    /// <summary>
    /// Gets or sets additional request parameters (must not conflict with locally set parameters)
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    public Parameters Parameters { get; set; } = new();
}