/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if NET451

using System.Diagnostics.Contracts;
using System.Xml;
using System.Xml.Linq;

namespace IdentityModel.Extensions
{
    public static partial class XmlExtensions
    {
        /// <summary>
        /// Converts a XmlDocument to a XDocument.
        /// </summary>
        /// <param name="document">The XmlDocument.</param>
        /// <returns>A XDocument</returns>
        public static XDocument ToXDocument(this XmlDocument document)
        {
            Contract.Requires(document != null);
            Contract.Ensures(Contract.Result<XDocument>() != null);


            return new XmlConverter(document).CreateXDocument();
        }   
    }
}

#endif