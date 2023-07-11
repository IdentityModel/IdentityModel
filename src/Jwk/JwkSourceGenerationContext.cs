using System.Text.Json.Serialization;

namespace IdentityModel.Jwk;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(JsonWebKey))]
[JsonSerializable(typeof(JsonWebKeySet))]
internal partial class JwkSourceGenerationContext : JsonSerializerContext
{
}
