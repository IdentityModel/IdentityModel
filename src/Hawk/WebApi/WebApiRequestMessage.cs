using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;

namespace Thinktecture.IdentityModel.Hawk.WebApi
{
    public class WebApiRequestMessage : WebApiMessage, IRequestMessage
    {
        private const string PARAMETER_KEY = "HK_Challenge";

        private readonly HttpRequestMessage request = null;

        public WebApiRequestMessage(HttpRequestMessage request) : base(request.Content)
        {
            this.request = request;

            this.request.Headers.ToList()
                .ForEach(h => this.messageHeaders.Add(h.Key, h.Value.ToArray()));
        }

        public string ChallengeParameter
        {
            get
            {
                if (this.request.Properties.ContainsKey(PARAMETER_KEY))
                {
                    return (string)this.request.Properties[PARAMETER_KEY];
                }

                return null;
            }
            set
            {
                this.request.Properties[PARAMETER_KEY] = value;
            }
        }

        public string Host
        {
            get
            {
                return this.request.Headers.Host;
            }
        }

		public AuthenticationHeaderValue Authorization
        {
            get
            {
                return this.request.Headers.Authorization;
            }
            set
            {
                this.request.Headers.Authorization = value;
            }
        }

        public Uri Uri
        {
            get
            {
                return this.request.RequestUri;
            }
        }

        public HttpMethod Method
        {
            get
            {
                return this.request.Method;
            }
        }

        public string QueryString
        {
            set
            {
                UriBuilder builder = new UriBuilder(this.request.RequestUri);
                builder.Query = value ?? String.Empty;

                this.request.RequestUri = builder.Uri;
            }
        }

        public string Scheme
        {
            get
            {
                return this.request.RequestUri.Scheme;
            }
        }
    }
}
