namespace IdentityModel.Client;

/// <summary>
/// Request for token using urn:ietf:params:oauth:grant-type:device_code
/// </summary>
/// <seealso cref="TokenRequest" />
public class DeviceTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the device code.
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string DeviceCode { get; set; } = default!;
}