// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityModel.Client
{
    public class IntrospectionRequest
    {
        public string Token { get; set; }
        public string TokenTypeHint { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public IntrospectionRequest()
        {
            Token = "";
            ClientId = "";
            ClientSecret = "";
        }
    }
}