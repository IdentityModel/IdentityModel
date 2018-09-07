// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System.Linq;
using System.Text.Encodings.Web;

namespace IdentityModel.Client
{
    /// <summary>
    /// Helper class for creating request URLs
    /// </summary>
    public class RequestUrl
    {
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestUrl"/> class.
        /// </summary>
        /// <param name="baseUrl">The authorize endpoint.</param>
        public RequestUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Creates URL based on key/value input pairs.
        /// </summary>
        /// <param name="values">The values (either as a Dictionary of string/string or as a type with properties).</param>
        /// <returns></returns>
        public string Create(object values)
        {
            var dictionary = ValuesHelper.ObjectToDictionary(values);
            if (dictionary == null || !dictionary.Any())
            {
                return _baseUrl;
            }

            return QueryHelpers.AddQueryString(_baseUrl, dictionary);
        }
    }
}