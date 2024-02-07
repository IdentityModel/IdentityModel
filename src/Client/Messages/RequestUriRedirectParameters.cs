/// <summary>
/// Models the parameters that can be sent in an authorize redirect when passing
/// a request object by reference in the request uri parameter, for use with
/// Pushed Authorization Requests and/or JWT-Secured Authorization Requests.
/// </summary>
public class RequestUriRedirectParameters
{
    /// <summary>
    /// ctor
    /// </summary>
    public RequestUriRedirectParameters(string clientId, string requestUri)
    {
        ClientId = clientId;
        RequestUri = requestUri;
    }
    
    /// <summary>
    /// Gets or sets the client_id protocol parameter.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the request_uri protocol parameter.
    /// </summary>
    public string RequestUri { get; set; }
}
