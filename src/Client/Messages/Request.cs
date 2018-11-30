// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models a base OAuth/OIDC request with client credentials
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the client assertion.
        /// </summary>
        /// <value>
        /// The assertion.
        /// </value>
        public ClientAssertion ClientAssertion { get; set; } = new ClientAssertion();

        /// <summary>
        /// Gets or sets the client credential style.
        /// </summary>
        /// <value>
        /// The client credential style.
        /// </value>
        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

        /// <summary>
        /// Gets or sets the basic authentication header style.
        /// </summary>
        /// <value>
        /// The basic authentication header style.
        /// </value>
        public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

        /// <summary>
        /// Gets or sets optional parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Request Clone()
        {
            var clone = new Request
            {
                Address = Address,
                AuthorizationHeaderStyle = AuthorizationHeaderStyle,
                ClientAssertion = ClientAssertion,
                ClientCredentialStyle = ClientCredentialStyle,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Parameters = new Dictionary<string, string>()
            };

            if (Parameters != null)
            {
                foreach (var item in Parameters) clone.Parameters.Add(item);
            }

            return clone;
        }
    }

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
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the assertion value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }
}