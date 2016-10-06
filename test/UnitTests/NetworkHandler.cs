using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.UnitTests
{
    public class NetworkHandler : HttpMessageHandler
    {
        enum Behavior
        {
            Throw,
            ReturnError,
            ReturnDocument
        }

        private readonly Exception _exception;
        private Behavior _behavior;
        private readonly HttpStatusCode _statusCode;
        private readonly string _reason;
        private readonly string _document;
        private readonly Func<HttpRequestMessage, string> _selector;

        public NetworkHandler(Exception exception)
        {
            _exception = exception;
            _behavior = Behavior.Throw;
        }

        public NetworkHandler(HttpStatusCode statusCode, string reason)
        {
            _statusCode = statusCode;
            _reason = reason;
            _behavior = Behavior.ReturnError;
        }

        public NetworkHandler(string document, HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            _document = document;
            _behavior = Behavior.ReturnDocument;
        }

        public NetworkHandler(Func<HttpRequestMessage, string> documentSelector, HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            _selector = documentSelector;
            _behavior = Behavior.ReturnDocument;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_behavior == Behavior.Throw) throw _exception;

            var response = new HttpResponseMessage(_statusCode);

            if (_behavior == Behavior.ReturnError)
            {
                response.ReasonPhrase = _reason;
            }

            if (_behavior == Behavior.ReturnDocument)
            {
                if (_selector != null)
                {
                    response.Content = new StringContent(_selector(request));
                }
                else
                {
                    response.Content = new StringContent(_document);
                }
            }

            return Task.FromResult(response);
        }
    }
}