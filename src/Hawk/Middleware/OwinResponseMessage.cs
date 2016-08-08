#if NET452
using Microsoft.Owin;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;


namespace Hawk.Middleware
{
#if NET452
    public class OwinResponseMessage : OwinMessage, IResponseMessage
    {
        private const string OwinResponseHeadersKey = "owin.ResponseHeaders";

        private IOwinResponse response;

        public OwinResponseMessage(IOwinResponse response)
            : base(response.Body, response.ContentType)
        {
            this.response = response;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                var statusCode = Enum.Parse(typeof(HttpStatusCode), this.response.StatusCode.ToString());
                return (HttpStatusCode)statusCode;
            }
        }

        public AuthenticationHeaderValue WwwAuthenticate
        {
            get
            {
                if (this.Headers.ContainsKey(HawkConstants.WwwAuthenticateHeaderName))
                {
                    string[] headerValues = this.Headers[HawkConstants.WwwAuthenticateHeaderName];
                    string hawkHeader = headerValues.First(s => s.StartsWith(HawkConstants.Scheme,
                                                                                StringComparison.OrdinalIgnoreCase));

                    return AuthenticationHeaderValue.Parse(hawkHeader);
                }

                return null;
            }
        }

        public void AddHeader(string name, string value)
        {
            this.response.Headers.AppendValues(name, value);
        }

        public string ContentType
        {
            get
            {
                string contentType = this.response.ContentType;
                if (!String.IsNullOrWhiteSpace(contentType))
                {
                    var header = MediaTypeHeaderValue.Parse(contentType);
                    return header.MediaType;
                }

                return null;
            }
        }

        public IDictionary<string, string[]> Headers
        {
            get
            {
                return this.response.Environment[OwinResponseHeadersKey] as IDictionary<string, string[]>;
            }
        }
    }
#endif
}
