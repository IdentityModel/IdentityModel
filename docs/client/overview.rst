Overview
========
IdentityModel contains client libraries for many interactions with endpoints defined in OpenID Connect and OAuth 2.0.
All of these libraries have a common design, let's examine the various layers using the client for the token endpoint.

Request and response objects
----------------------------
All protocol request are modelled as request objects and have a common base class called ``ProtocolRequest`` which has properties to set the 
endpoint address, client ID, client secret, client assertion, and the details of how client secrets are transmitted (e.g. authorization header vs POST body).
``ProtocolRequest`` derives from ``HttpRequestMessage`` and thus also allows setting custom headers etc.

The following code snippet creates a request for a client credentials grant type::

    var request = new ClientCredentialsTokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",
        ClientId = "client",
        ClientSecret = "secret"
    });

While in theory you could now call ``Prepare`` (which internally sets the headers, body and adress) and send the request via a plain ``HttpClient``,
typically there are more parameters with special semantics and encoding required. That's why we provide extension methods to do the low level work.

Equally, a protocol response has a corresponding ``ProtocolResponse`` implementation that parses the status codes and response content.
The following code snippet would parse the raw HTTP response from a token endpoint and turn it into a ``TokenResponse`` object::

    var tokenResponse = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(httpResponse);

Again these steps are automated using the extension methods. So let's have a look at an example next.

Extension methods
-----------------
For each protocol interaction, an extension method for ``HttpMessageInvoker`` (that's the base class of ``HttpClient``) exists. 
The extension methods expect a request object and return a response object.

It is your responsibility to setup and manage the lifetime of the ``HttpClient``, e.g. manually::

    var client = new HttpClient();

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",
        ClientId = "client",
        ClientSecret = "secret"
    });

You might want to use other techniques to obtain an ``HttpClient``, e.g. via the HTTP client factory::

    var client = HttpClientFactory.CreateClient("my_named_token_client");

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = "https://demo.identityserver.io/connect/token",
        ClientId = "client",
        ClientSecret = "secret"
    });


All other endpoint client follow the same design.

.. note:: Some client libraries also include a stateful client object (e.g. ``TokenClient`` and ``IntrospectionClient``). See the corresponding section to find out more.