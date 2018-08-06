// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client
{
    /// <summary>
    /// Request for device authorization
    /// </summary>
    public class DeviceAuthorizationRequest : Request
    {
        /// <summary>
        /// Gets or sets the scope (optional).
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }
    }
}