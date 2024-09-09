// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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