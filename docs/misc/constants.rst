Protocol and Claim Type Constants
=================================
When working with OAuth 2.0, OpenID Connect and claims, there are a lot of "magic strings" for 
claim types and protocoal values. IdentityModel provides a couple of constant strings classes to help with that.

OAuth 2.0 and OpenID Connect Protocol Values
--------------------------------------------
The ``OidcConstants`` class has all the values for grant types, parameter names, error names etc.

JWT Claim Types
---------------
The ``JwtClaimTypes`` class has all standard claim types found in the OpenID Connect, JWT and OAuth 2.0 specs -
many of them are also aggregated at `IANA <https://www.iana.org/assignments/jwt/jwt.xhtml>`_.