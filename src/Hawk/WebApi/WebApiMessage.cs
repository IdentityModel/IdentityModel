using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;

namespace Thinktecture.IdentityModel.Hawk.WebApi
{
    /// <summary>
    /// The base HTTP message class.
    /// </summary>
    public abstract class WebApiMessage : IMessage
    {
        private readonly HttpContent content = null;
        private readonly Lazy<Task<string>> body = null;

        protected readonly IDictionary<string, string[]> messageHeaders = null;

        public WebApiMessage(HttpContent content)
        {
            this.content = content;

            this.body = new Lazy<Task<string>>(async () =>
            {
                if (this.content != null)
                {
                    await this.content.LoadIntoBufferAsync();
                    return await this.content.ReadAsStringAsync();
                }

                return null;
            });

            this.messageHeaders = new Dictionary<string, string[]>();

            if (this.content != null)
                this.content.Headers.ToList()
                    .ForEach(h => this.messageHeaders.Add(h.Key, h.Value.ToArray()));
        }

        public async Task<string> ReadBodyAsStringAsync()
        {
            return await this.body.Value;
        }

        public string ContentType
        {
            get
            {
                if (this.content != null)
                {
                    var contentType = this.content.Headers.ContentType;
                    if (contentType != null)
                        return contentType.MediaType;
                }

                return null;
            }
        }

        public IDictionary<string, string[]> Headers
        {
            get
            {
                return this.messageHeaders;
            }
        }
    }
}
