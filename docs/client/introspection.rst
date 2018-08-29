Token Introspection Endpoint
============================
The client library for `OAuth 2.0 token introspection <https://tools.ietf.org/html/rfc7662>`_ is provided as an extension method for ``HttpClient``.

The following code sends a reference token to an introspection endpoint::

    var client = new HttpClient();

    var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
    {
        Address = "https://demo.identityserver.io/connect/introspect",
        ClientId = "api1",
        ClientSecret = "secret",

        Token = accessToken
    });

The response is of type ``IntrospectionResponse`` and has properties for the standard response parameters.
You also have access to the the raw response as well as to a parsed JSON document 
(via the ``Raw`` and ``Json`` properties).

Before using the response, you should always check the ``IsError`` property to make sure the request was successful::

    if (response.IsError) throw new Exception(response.Error);

    var isActive = response.IsActive;
    var claims = response.Claims;
