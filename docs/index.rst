Overview
========
IdentityModel is a family of libraries for building OAuth 2.0 and OpenID Connect clients.

It has the following high level features:

* OpenID certified library for building .NET-based native clients (e.g. console, Xamarin iOS & Android, WPF and WinForms). Follows `RFC 8252 <https://tools.ietf.org/html/rfc8252>`_.
* OpenID certified library for building JavaScript-based clients. Follows the `OAuth 2.0 for Browser-Based Apps BCP <https://tools.ietf.org/html/draft-ietf-oauth-browser-based-apps-03>`_.
* .NET standard client library for OAuth 2.0 and OpenID Connect endpoints like authorize, token, discovery, introspection, revocation etc.
* Helpers for ASP.NET Core token management
* OAuth 2.0 Token Introspection authentication handler for ASP.NET Core
* Constants for standard JWT claim types and protocol values
* Misc helpers for base64 URL encoding, X509 certificate store access, time constant string comparison and epoch time

IdentityModel
-------------
* github https://github.com/IdentityModel/IdentityModel
* nuget https://www.nuget.org/packages/IdentityModel/
* CI builds https://www.myget.org/F/identity/

IdentityModel.AspNetCore
------------------------
* github https://github.com/IdentityModel/IdentityModel.AspNetCore
* nuget https://www.nuget.org/packages/IdentityModel.AspNetCore/
* CI builds https://www.myget.org/F/identity/

IdentityModel.AspNetCore.OAuthIntrospection
-------------------------------------------
* github https://github.com/IdentityModel/IdentityModel.AspNetCore.OAuthIntrospection
* nuget https://www.nuget.org/packages/IdentityModel.AspNetCore.OAuthIntrospection/
* CI builds https://www.myget.org/F/identity/

IdentityModel.OidcClient
------------------------
* github https://github.com/IdentityModel/IdentityModel.OidcClient
* nuget https://www.nuget.org/packages/IdentityModel.OidClient
* CI builds https://www.myget.org/F/identity/

oidc-client.js
--------------
* github https://github.com/IdentityModel/oidc-client-js
* npm https://www.npmjs.com/package/oidc-client


.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: IdentityModel

   client/overview
   client/discovery
   client/token
   client/introspection
   client/revocation
   client/userinfo
   client/dynamic_registration
   client/device_authorize

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: IdentityModel - Misc Helpers

   misc/constants
   misc/request_url
   misc/x509store
   misc/base64
   misc/epoch_time
   misc/time_constant_comparison

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Building mobile/native Clients

   native/overview
   native/manual
   native/automatic
   native/logging
   native/samples

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Building JavaScript Clients

   js/overview