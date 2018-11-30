Welcome to the IdentityModel Documentation!
===========================================

IdentityModel is a .NET standard helper library for claims-based identity, OAuth 2.0 and OpenID Connect.

It has the following high level features:

* client libraries for standard OAuth 2.0 and OpenID Connect endpoints like authorize, token, discovery, introspection, revocation etc.
* helpers for token management
* constants for standard JWT claim types and protocol values
* simplified API to access the X509 certificate store
* misc helpers for base64 URL encoding, time constant string comparison and epoch time

How to get
----------

* github https://github.com/IdentityModel/IdentityModel2
* nuget https://www.nuget.org/packages/IdentityModel/
* CI builds https://www.myget.org/F/identity/


.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Protocol Client Libraries

   client/discovery
   client/authorize
   client/end_session
   client/token
   client/introspection
   client/revocation
   client/userinfo
   client/dynamic_registration
   client/device_authorize

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Misc Helpers

   misc/constants
   misc/request_url
   misc/x509store
   misc/base64
   misc/epoch_time
   misc/time_constant_comparison