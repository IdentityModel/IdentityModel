// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client
{
    public class RegistrationRequest
    {
        public ICollection<string> redirect_uris { get; set; } = new HashSet<string>();
        public ICollection<string> response_types { get; set; } = new HashSet<string>();
        public ICollection<string> grant_types { get; set; } = new HashSet<string>();
        public string application_type { get; set; }
        public ICollection<string> contacts { get; set; } = new HashSet<string>();
        public string client_name { get; set; }
        public string logo_uri { get; set; }
        public string client_uri { get; set; }
        public string policy_uri { get; set; }
        public string tos_uri { get; set; }
        public string jwks_uri { get; set; }
        public string jwks { get; set; } // correct type?
        public string sector_identifier_uri { get; set; }
        public string subject_type { get; set; } // pairwise or public
        public string id_token_signed_response_alg { get; set; }
        public string id_token_encrypted_response_alg { get; set; }
        public string id_token_encrypted_response_enc { get; set; }
        public string userinfo_signed_response_alg { get; set; }
        public string userinfo_encrypted_response_alg { get; set; }
        public string userinfo_encrypted_response_enc { get; set; }
        public string request_object_signing_alg { get; set; }
        public string request_object_encryption_alg { get; set; }
        public string request_object_encryption_enc { get; set; }
        public string token_endpoint_auth_method { get; set; }
        public string token_endpoint_auth_signing_alg { get; set; }
        public int default_max_age { get; set; }
        public bool require_auth_time { get; set; }
        public ICollection<string> default_acr_values { get; set; } = new HashSet<string>();
        public string initiate_login_uri { get; set; }
        public ICollection<string> request_uris { get; set; } = new HashSet<string>();
    }
}