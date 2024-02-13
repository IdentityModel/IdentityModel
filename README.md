## About IdentityModel

IdentityModel is a .NET library for claims-based identity, OAuth 2.0 and OpenID Connect. 

It provides an object model to interact with the endpoints defined in the various OAuth
and OpenId Connect specifications in the form of:
- types to represent the requests and responses
- extension methods to invoke requests
- constants defined in the specifications, such as standard scope, claim, and parameter names
- other convenience methods for performing common identity related operations

IdentityModel targets [.NET Standard 2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0), 
making it suitable for .NET and .NET Framework.

For more documentation, please visit [readthedocs](https://identitymodel.readthedocs.io).

## Related Packages

- Certified OIDC client library for native apps: [IdentityModel.OidcClient](https://www.nuget.org/packages/IdentityModel.OidcClient)
- Id token validator for IdentityModel.OidcClient based on the Microsoft JWT handler: [IdentityModel.OidcClient.IdentityTokenValidator](https://www.nuget.org/packages/IdentityModel.OidcClient.IdentityTokenValidator)
- [DPoP](https://datatracker.ietf.org/doc/html/rfc9449) extensions for IdentityModel.OidcClient: [IdentityModel.OidcClient.DPoP ](https://www.nuget.org/packages/IdentityModel.OidcClient.DPoP)
- Authentication handler for introspection tokens: [IdentityModel.AspNetCore.OAuth2Introspection](https://www.nuget.org/packages/IdentityModel.AspNetCore.OAuth2Introspection)

## Feedback

IdentityModel is released as open source under the 
[Apache 2.0 license](https://github.com/identitymodel/identitymodel/blob/main/LICENSE). 
Bug reports and contributions are welcome at 
[the GitHub repository](https://github.com/identitymodel/identitymodel).