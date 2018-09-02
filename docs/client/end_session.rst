.. _refEndSession:
EndSession Endpoint
===================

The :ref:`RequestUrl <refRequestUrl>` class can be used to construct URLs to the `OpenID Connect EndSession endpoint <https://openid.net/specs/openid-connect-session-1_0.html#RPLogout>`_.

The ``CreateEndSessionUrl`` extensions methods supports the most common parameters::

    /// <summary>
    /// Creates a end_session URL.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="idTokenHint">The id_token hint.</param>
    /// <param name="postLogoutRedirectUri">The post logout redirect URI.</param>
    /// <param name="state">The state.</param>
    /// <param name="extra">The extra parameters.</param>
    /// <returns></returns>
    public static string CreateEndSessionUrl(this RequestUrl request,
        string idTokenHint = null,
        string postLogoutRedirectUri = null,
        string state = null,
        object extra = null)
    { ... }

.. note:: The ``extra`` parameter can either be a string dictionary or an arbitrary other type with properties. In both cases the values will be serialized as keys/values.