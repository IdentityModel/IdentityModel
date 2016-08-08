#if NET452
using Microsoft.Owin;
#endif
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;

namespace Hawk.Middleware
{
#if NET452
    public class OwinRequestMessage : OwinMessage, IRequestMessage
    {
        private const string PARAMETER_KEY = "HK_Challenge";
        private const string OwinRequestHeadersKey = "owin.RequestHeaders";

        private IOwinRequest request;

        public OwinRequestMessage(IOwinRequest request)
            : base(request.Body, request.Headers.Get(HawkConstants.ContentTypeHeaderName))
        {
            this.request = request;
        }

        public string ChallengeParameter
        {
            get
            {
                return this.request.Get<string>(PARAMETER_KEY);
            }
            set
            {
                this.request.Set<string>(PARAMETER_KEY, value);
            }
        }

        public string Host
        {
            get
            {
                return this.request.Host.Value;
            }
        }

        public AuthenticationHeaderValue Authorization
        {
            get
            {
                string authorization = this.request.Headers.Get(HawkConstants.AuthorizationHeaderName);

                return authorization != null ?
                    AuthenticationHeaderValue.Parse(authorization) :
                        null;
            }
            set
            {
                this.Headers[HawkConstants.AuthorizationHeaderName] = new[] { value.ToString() };
            }
        }

        public Uri Uri
        {
            get
            {
                return this.request.Uri;
            }
        }

        public HttpMethod Method
        {
            get
            {
                return new HttpMethod(this.request.Method);
            }
        }

        public string ContentType
        {
            get
            {
                string contentType = this.request.Headers.Get(HawkConstants.ContentTypeHeaderName);

                if (!String.IsNullOrWhiteSpace(contentType))
                {
                    var header = MediaTypeHeaderValue.Parse(contentType);
                    if (header != null)
                        return header.MediaType;
                }

                return null;
            }
        }

        public IDictionary<string, string[]> Headers
        {
            get
            {
                return this.request.Environment[OwinRequestHeadersKey] as IDictionary<string, string[]>;
            }
        }

        public string QueryString
        {
            set
            {
                this.request.QueryString = value == null ?
                                                Microsoft.Owin.QueryString.Empty :
                                                    new Microsoft.Owin.QueryString(value);
            }
        }

        public string Scheme
        {
            get
            {
                return this.request.Scheme;
            }
        }
    }
#endif
}
