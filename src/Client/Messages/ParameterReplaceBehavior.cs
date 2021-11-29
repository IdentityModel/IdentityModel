namespace IdentityModel.Client;

/// <summary>
/// Specifies how parameter in the collection get replaced (or not).
/// </summary>
public enum ParameterReplaceBehavior
{
    /// <summary>
    /// Allow multiple
    /// </summary>
    None, 
        
    /// <summary>
    /// Replace a single parameter with the same key
    /// </summary>
    Single, 
        
    /// <summary>
    /// Replace all parameters with same key
    /// </summary>
    All
}