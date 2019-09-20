Manual Mode
===========
In manual mode, OidcClient helps you with creating the necessary start URL and state parameters, but you need to coordinate with whatever browser you want to use, e.g.::

    var options = new OidcClientOptions
    {
        Authority = "https://demo.identityserver.io",
        ClientId = "native",
        RedirectUri = redirectUri,
        Scope = "openid profile api"
    };

    var client = new OidcClient(options);

    // generate start URL, state, nonce, code challenge
    var state = await client.PrepareLoginAsync();

When the browser work is done, OidcClient can take over to process the response, get the access/refresh tokens, contact userinfo endpoint etc..::

    var result = await client.ProcessResponseAsync(data, state);

The result will contain the tokens and the claims of the user.
