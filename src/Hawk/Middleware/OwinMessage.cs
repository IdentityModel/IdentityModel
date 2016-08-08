using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hawk.Middleware
{
#if NET452

    public abstract class OwinMessage
    {
        private readonly Encoding defaultEncoding = Encoding.UTF8;
        private readonly Lazy<string> body = null;

        public OwinMessage(Stream bodyStream, string contentType)
        {
            this.body = new Lazy<string>(() =>
            {
                MemoryStream buffer = bodyStream as MemoryStream;

                if (buffer != null)
                {
                    byte[] content = buffer.GetBuffer();
                    if (content != null && content.Length > 0)
                    {
                        Encoding encoding = GetEncodingForContentType(contentType);
                        string body = encoding.GetString(content, 0, (int)buffer.Length);

                        buffer.Seek(0, SeekOrigin.Begin);

                        return body;
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Returns the Encoding object for the charset in the Content-Type header value.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private Encoding GetEncodingForContentType(string contentType)
        {
            Encoding encoding = defaultEncoding;

            if (!String.IsNullOrWhiteSpace(contentType))
            {
                var mediaHeader = MediaTypeHeaderValue.Parse(contentType);
                if (mediaHeader != null)
                {
                    string charset = mediaHeader.CharSet;
                    if (!String.IsNullOrWhiteSpace(charset))
                        encoding = Encoding.GetEncoding(charset);
                }
            }

            return encoding;
        }

        public Task<string> ReadBodyAsStringAsync()
        {
            return Task.FromResult<string>(this.body.Value);
        }
    }
#endif
}
