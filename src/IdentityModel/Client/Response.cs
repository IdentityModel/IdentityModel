// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    /// <summary>
    /// A protocol response
    /// </summary>
    public abstract class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
        protected Response(string raw)
        {
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
            }
            catch (Exception ex)
            {
                ErrorType = ResponseErrorType.Exception;
                Exception = ex;

                return;
            }

            if (string.IsNullOrWhiteSpace(Error))
            {
                HttpStatusCode = HttpStatusCode.OK;
            }
            else
            {
                HttpStatusCode = HttpStatusCode.BadRequest;
                ErrorType = ResponseErrorType.Protocol;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class with an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected Response(Exception exception)
        {
            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class with an HTTP status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        protected Response(HttpStatusCode statusCode, string reason)
        {
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;

            if (statusCode != HttpStatusCode.OK) ErrorType = ResponseErrorType.Http;
        }

        /// <summary>
        /// Gets the raw protocol response.
        /// </summary>
        /// <value>
        /// The raw.
        /// </value>
        public string Raw { get; }

        /// <summary>
        /// Gets the protocol response as JSON.
        /// </summary>
        /// <value>
        /// The json.
        /// </value>
        public JObject Json { get; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets a value indicating whether an error occurred.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an error occurred; otherwise, <c>false</c>.
        /// </value>
        public bool IsError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// Gets the type of the error.
        /// </summary>
        /// <value>
        /// The type of the error.
        /// </value>
        public ResponseErrorType ErrorType { get; } = ResponseErrorType.None;

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// Gets the HTTP error reason.
        /// </summary>
        /// <value>
        /// The HTTP error reason.
        /// </value>
        public string HttpErrorReason { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }
                else if (ErrorType == ResponseErrorType.Exception)
                {
                    return Exception.Message;
                }

                return TryGet(OidcConstants.TokenResponse.Error);
            }
        }

        /// <summary>
        /// Tries to get a specific value from the JSON response.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string TryGet(string name) => Json.TryGetString(name);
    }
}