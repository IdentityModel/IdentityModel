// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    public abstract class Response
    {
        public Response(string raw)
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

        public Response(Exception exception)
        {
            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        public Response(HttpStatusCode statusCode, string reason)
        {
            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;
        }

        public string Raw { get; }
        public JObject Json { get; }
        public Exception Exception { get; set; }

        public bool IsError => !string.IsNullOrEmpty(Error);
        public ResponseErrorType ErrorType { get; } = ResponseErrorType.None;
        public HttpStatusCode HttpStatusCode { get; }
        public string HttpErrorReason { get; }

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

        public string TryGet(string name) => Json.TryGetString(name);
    }
}