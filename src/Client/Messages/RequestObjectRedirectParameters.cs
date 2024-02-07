// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

/// <summary>
/// Models the parameters that can be sent in an authorize redirect when using a
/// JWT-Secured Authorization Request (JAR) and passing the request object by
/// value in the request parameter.
/// </summary>
public class RequestObjectRedirectParameters
{
    /// <summary>
    /// ctor
    /// </summary>
    public RequestObjectRedirectParameters(string clientId, string request)
    {
        ClientId = clientId;
        Request = request;
    }

    /// <summary>
    /// Gets or sets the client_id protocol parameter.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the request protocol parameter. 
    /// </summary>
    public string Request { get; set; }
}
