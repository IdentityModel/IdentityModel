// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Models a client assertion
/// </summary>
public class ClientAssertion
{
    /// <summary>
    /// Gets or sets the assertion type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public string Type { get; set; } = default!;

    /// <summary>
    /// Gets or sets the assertion value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; set; } = default!;
}