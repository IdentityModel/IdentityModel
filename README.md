# IdentityModel
A .NET standard helper library for claims-based identity, OAuth 2.0 and OpenID Connect.

The nuget package can be found [here](https://www.nuget.org/packages/IdentityModel/) or use the https://www.myget.org/F/identity/ myget feed for CI builds.

### DiscoveryClient
Client library to retrieve OpenID Connect discovery documents and key sets.

```csharp
var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync("https://demo.identityserver.io");
if (disco.IsError) throw new Exception(disco.Error);

var tokenEndpoint = doc.TokenEndpoint;
var keys = doc.KeySet.Keys;
```

### DiscoveryCache
Simple in-memory cache for discovery documents

```csharp
var cache = new DiscoveryCache(Constants.Authority);

var disco = await cache.GetAsync();
if (disco.IsError) throw new Exception(disco.Error);
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
var client = new HttpClient();

var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});

if (response.IsError) throw new Exception(response.Error);
var token = response.AccessToken;
```

### UserInfoClient
Client library for the OpenID Connect user info endpoint

```csharp
var client = new HttpClient();

var response = await client.GetUserInfoAsync(new UserInfoRequest
{
    Address = disco.UserInfoEndpoint,
    Token = token
});

if (response.IsError) throw new Exception(response.Error);

foreach (var claim in response.Claims)
{
    Console.WriteLine("{0}\n {1}", claim.Type, claim.Value);
}
```

### IntrospectionClient
Client library for the OAuth 2 introspection endpoint

```csharp
var client = new HttpClient();
var result = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
{
    Address = disco.IntrospectionEndpoint,

    ClientId = "api1",
    ClientSecret = "secret",
    Token = accessToken
});

if (result.IsError)
{
    Console.WriteLine(result.Error);
}
else
{
    if (result.IsActive)
    {
        result.Claims.ToList().ForEach(c => Console.WriteLine("{0}: {1}",
            c.Type, c.Value));
    }
    else
    {
        Console.WriteLine("token is not active");
    }
}
```

### RequestUrl
Helper class for creating request URLs (e.g. for authorize and end_session).

```csharp
var request = new RequestUrl(doc.AuthorizationEndpoint);
var url = request.CreateAuthorizeUrl(
    clientId:         "client",
    responseType:     OidcConstants.ResponseTypes.CodeIdToken,
    responseMode:     OidcConstants.ResponseModes.FormPost,
    redirectUri:     "https://myapp.com/callback",
    state:           CryptoRandom.CreateUniqueId(),
    nonce:           CryptoRandom.CreateUniqueId());
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
