# IdentityModel
A .NET standard helper library for claims-based identity, OAuth 2.0 and OpenID Connect.

### DiscoveryClient
Client library to retrieve OpenID Connect discovery documents and key sets.

```csharp
var discoveryClient = new DiscoveryClient("https://demo.identityserver.io");
var doc = await discoveryClient.GetAsync();

var tokenEndpoint = doc.TokenEndpoint;
var keys = doc.KeySet.Keys;
```

### TokenClient
Client library for OAuth 2.0 and OpenID Connect token endpoints.

Features:

* Support for client credentials & resource owner password credential flow
* Support for exchanging authorization codes with tokens
* Support for refreshing tokens
* Support for extensions grants and assertions
* Support for client secrets via Basic Authentication, POST body and X.509 client certificates
* Extensible for custom parameters
* Parsing of token response messages

Example:
```csharp
var client = new TokenClient(
    doc.TokenEndpoint,
    "client_id",
    "secret");
    
var response = await client.RequestClientCredentialsAsync("scope");
var token = response.AccessToken;
```

### UserInfoClient
Client library for the OpenID Connect user info endpoint

```csharp
var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);

var response = await userInfoClient.GetAsync(token);
var claims = response.Claims;
```

### IntrospectionClient
Client library for the OAuth 2 introspection endpoint

```csharp
var introspectionClient = new IntrospectionClient(
    doc.IntrospectionEndpoint,
    "scope_name",
    "scope_secret");

var response = await introspectionClient.SendAsync(
    new IntrospectionRequest { Token = token });

var isActice = response.IsActive;
var claims = response.Claims;
```

### AuthorizeRequest
Helper class for creating authorize request URLs (e.g. for code and implicit flow).

```csharp
var request = new AuthorizeRequest(doc.AuthorizationEndpoint);
var url = request.CreateAuthorizeUrl(
    clientId:     "client",
    responseType: OidcConstants.ResponseTypes.CodeIdToken,
    responseMode: OidcConstants.ResponseModes.FormPost,
    redirectUri: "https://myapp.com/callback",
    state:       CryptoRandom.CreateUniqueId(),
    nonce:       CryptoRandom.CreateUniqueId());
```

### AuthorizeResponse
Helper class for parsing OpenID Connect/OAuth 2 authorize responses

```csharp
var response = new AuthorizeResponse(url);

var accessToken = response.AccessToken;
var idToken = response.IdentityToken;
var state = response.State;
```

### Fluent API to access the X.509 Certificate store  
e.g. do  
`var cert = X509.LocalMachine.My.SubjectDistinguishedName.Find("CN=sts").First();`

### Base64 URL encoder/decoder
Helper for working with URL safe base64 encodings

### Epoch Time Extensions
Helper for converting `DateTime` and `DateTimeOffset` to/from Epoch Time

### Time Constant Comparer
Helper for comparing strings without leaking timing information

### JWT/OpenID Connect Claim Types
Constants for standard claim types used in JWT, OAuth 2.0 and OpenID Connect

### OpenID Connect Constants
Constants for the OpenID Connect/OAuth 2 protocol