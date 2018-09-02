.. _refAuthorize:
Authorization Endpoint
======================
For most cases, the `OAuth 2.0 <https://tools.ietf.org/html/rfc6749#section-3.1>`_ and `OpenID Connect <https://openid.net/specs/openid-connect-core-1_0.html#AuthorizationEndpoint>`_ authorization endpoint expects a GET request with a number of query string parameters.

While you can use any means of creating URLs with parameters to create the right strings, 
the :ref:`RequestUrl <refRequestUrl>` class is a simple helper to accomplish this task.

In particular, you can use the ``CreateAuthorizeUrl`` extension method to create URLs for the authorize endpoint - it has support the most common parameters::

    /// <summary>
    /// Creates an authorize URL.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="responseType">The response type.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="state">The state.</param>
    /// <param name="nonce">The nonce.</param>
    /// <param name="loginHint">The login hint.</param>
    /// <param name="acrValues">The acr values.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="responseMode">The response mode.</param>
    /// <param name="codeChallenge">The code challenge.</param>
    /// <param name="codeChallengeMethod">The code challenge method.</param>
    /// <param name="display">The display option.</param>
    /// <param name="maxAge">The max age.</param>
    /// <param name="uiLocales">The ui locales.</param>
    /// <param name="idTokenHint">The id_token hint.</param>
    /// <param name="extra">Extra parameters.</param>
    /// <returns></returns>
    public static string CreateAuthorizeUrl(this RequestUrl request,
        string clientId,
        string responseType,
        string scope = null,
        string redirectUri = null,
        string state = null,
        string nonce = null,
        string loginHint = null,
        string acrValues = null,
        string prompt = null,
        string responseMode = null,
        string codeChallenge = null,
        string codeChallengeMethod = null,
        string display = null,
        int? maxAge = null,
        string uiLocales = null,
        string idTokenHint = null,
        object extra = null)
    { ... }

Example::

    var ru = new RequestUrl("https://demo.identityserver.io/connect/authorize");

    var url = ru.CreateAuthorizeUrl(
        clientId: "client",
        responseType: "implicit",
        redirectUri: "https://app.com/callback",
        nonce: "xyz",
        scope: "openid");

.. note:: The ``extra`` parameter can either be a string dictionary or an arbitrary other type with properties. In both cases the values will be serialized as keys/values.