Token Endpoint
==============
The client library for the token endpoint (`OAuth 2.0 <https://tools.ietf.org/html/rfc6749#section-3.2>`_ and `OpenID Connect <https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint>`_) 
is provided as a set of extension methods for ``HttpClient``.
This allows creating and managing the lifetime of the ``HttpClient`` the way you prefer - 
e.g. statically or via a factory like the Microsoft ``HttpClientFactory``.

Requesting a token
------------------
The main extension method is called ``RequestTokenAsync`` - it has direct support for standard parameters 
like client ID/secret (or assertion) and grant type, but it also allows setting arbitrary other parameters via a dictionary.
All other extensions methods ultimately call this method internally::

    var client = new HttpClient();

    var response = await client.RequestTokenAsync(new TokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",
        GrantType = "custom",

        ClientId = "client",
        ClientSecret = "secret",

        Parameters =
        {
            { "custom_parameter", "custom value"},
            { "scope", "api1" }
        }
    });

The response is of type ``TokenResponse`` and has properties for the standard token response parameters 
like ``access_token``, ``expires_in`` etc. You also have access to the the raw response as well as to a parsed JSON document 
(via the ``Raw`` and ``Json`` properties).

Before using the response, you should always check the ``IsError`` property to make sure the request was successful::

    if (response.IsError) throw new Exception(response.Error);

    var token = response.AccessToken;
    var custom = response.Json.TryGetString("custom_parameter");

Requesting a token using the ``client_credentials`` Grant Type
--------------------------------------------------------------
The ``RequestClientCredentialsToken`` extension method has convenience properties for the ``client_credentials`` grant type::

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",
        
        ClientId = "client",
        ClientSecret = "secret",
        Scope = "api1"
    });

Requesting a token using the ``password`` Grant Type
----------------------------------------------------
The ``RequestPasswordToken`` extension method has convenience properties for the ``password`` grant type::

    var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",

        ClientId = "client",
        ClientSecret = "secret",
        Scope = "api1",

        UserName = "bob",
        Password = "bob"
    });

Requesting a token using the ``authorization_code`` Grant Type
--------------------------------------------------------------
The ``RequestAuthorizationCodeToken`` extension method has convenience properties for the ``authorization_code`` grant type and PKCE::

    var response = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
    {
        Address = IdentityServerPipeline.TokenEndpoint,

        ClientId = "client",
        ClientSecret = "secret",

        Code = code,
        RedirectUri = "https://app.com/callback",

        // optional PKCE parameter
        CodeVerifier = "xyz"
    });

Requesting a token using the ``refresh_token`` Grant Type
--------------------------------------------------------------
The ``RequestRefreshToken`` extension method has convenience properties for the ``refresh_token`` grant type::

    var response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
    {
        Address = TokenEndpoint,

        ClientId = "client",
        ClientSecret = "secret",

        RefreshToken = "xyz"
    });

Requesting a Device Token
-------------------------
The ``RequestDeviceToken`` extension method has convenience properties for the ``urn:ietf:params:oauth:grant-type:device_code`` grant type::

    var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
    {
        Address = disco.TokenEndpoint,
        
        ClientId = "device",
        DeviceCode = authorizeResponse.DeviceCode
    });