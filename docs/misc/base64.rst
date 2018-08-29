Base64 URL Encoding
===================
JWT tokens are serialized using `Base64 URL encoding <https://tools.ietf.org/html/rfc4648#section-5>`_.

IdentityModel includes the ``Base64Url`` class to help with encoding/decoding::

    var text = "hello";
    var b64url = Base64Url.Encode(text);

    text = Base64Url.Decode(b64url);

.. note:: ASP.NET Core has built-in support via `WebEncoders.Base64UrlEncode <https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.webutilities.webencoders.base64urlencode>`_ and `WebEncoders.Base64UrlDecode <https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.webutilities.webencoders.base64urldecode>`_.
