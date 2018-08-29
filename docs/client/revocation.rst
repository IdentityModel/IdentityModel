Token Revocation Endpoint
=========================
The client library for `OAuth 2.0 token revocation <https://tools.ietf.org/html/rfc7009>`_ is provided as an extension method for ``HttpClient``.

The following code revokes an access token token at a revocation endpoint::

    var client = new HttpClient();

    var result = await client.RevokeTokenAsync(new TokenRevocationRequest
    {
        Address = "https://demo.identityserver.io/connect/revocation",
        ClientId = "client",
        ClientSecret = "secret",

        Token = accessToken
    });

The response is of type ``TokenRevocationResponse`` gives you access to the the raw response as well as to a parsed JSON document 
(via the ``Raw`` and ``Json`` properties).

Before using the response, you should always check the ``IsError`` property to make sure the request was successful::

    if (response.IsError) throw new Exception(response.Error);
