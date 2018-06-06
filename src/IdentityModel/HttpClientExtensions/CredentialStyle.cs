using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public enum CredentialStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        AuthorizationHeader,

        /// <summary>
        /// post values in body
        /// </summary>
        PostBody,
    };
}
