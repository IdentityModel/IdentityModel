// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// A protocol response
    /// </summary>
    public class Response2
    {
        public static async Task<T> FromHttpResponseAsync<T>(HttpResponseMessage httpResponse) where T: Response2, new()
        {
            var response = new T
            {
                HttpResponse = httpResponse
            };

            // try to read content
            string content = null;
            try
            {
                content = await httpResponse.Content.ReadAsStringAsync();
                response.Raw = content;
            }
            catch { }

            // some HTTP error - try to parse body as JSON but allow non-JSON as well
            if (httpResponse.StatusCode != HttpStatusCode.OK &&
                httpResponse.StatusCode != HttpStatusCode.BadRequest)
            {
                response.ErrorType = ResponseErrorType.Http;

                if (content.IsPresent())
                {
                    try
                    {
                        response.Json = JObject.Parse(content);
                    }
                    catch { }
                }

                return response;
            }
            
            if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                response.ErrorType = ResponseErrorType.Protocol;
            }

            // either 200 or 400 - both cases need a JSON response, otherwise error
            try
            {
                response.Json = JObject.Parse(content);
            }
            catch (Exception ex)
            {
                response.ErrorType = ResponseErrorType.Exception;
                response.Exception = ex;
            }
            
            return response;
        }

        public static async Task<T> FromException<T>(Exception ex) where T : Response2, new()
        {
            var response = new T
            {
                Exception = ex,
                ErrorType = ResponseErrorType.Exception
            };

            return response;
        }



        public HttpResponseMessage HttpResponse { get; protected set; }


       

        /// <summary>
        /// Gets the raw protocol response.
        /// </summary>
        /// <value>
        /// The raw.
        /// </value>
        public string Raw { get; protected set; }

        /// <summary>
        /// Gets the protocol response as JSON.
        /// </summary>
        /// <value>
        /// The json.
        /// </value>
        public JObject Json { get; protected set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether an error occurred.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an error occurred; otherwise, <c>false</c>.
        /// </value>
        public bool IsError => Error.IsPresent();

        /// <summary>
        /// Gets the type of the error.
        /// </summary>
        /// <value>
        /// The type of the error.
        /// </value>
        public ResponseErrorType ErrorType { get; protected set; } = ResponseErrorType.None;

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public HttpStatusCode HttpStatusCode => HttpResponse.StatusCode;

        /// <summary>
        /// Gets the HTTP error reason.
        /// </summary>
        /// <value>
        /// The HTTP error reason.
        /// </value>
        public string HttpErrorReason => HttpResponse.ReasonPhrase;

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