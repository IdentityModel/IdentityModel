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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace IdentityModel.Extensions
{
    internal class XmlConverter
    {
        private readonly StringBuilder _xmlTextBuilder;
        private readonly XmlWriter _writer;

        private XmlConverter()
        {
            _xmlTextBuilder = new StringBuilder();

            _writer = new XmlTextWriter(new StringWriter(_xmlTextBuilder))
            {
                Formatting = Formatting.Indented,
                Indentation = 2
            };
        }

        public XmlConverter(XNode e)
            : this()
        {
            Contract.Requires(e != null);


            e.WriteTo(_writer);
        }

        public XmlConverter(XmlNode e)
            : this()
        {
            Contract.Requires(e != null);


            e.WriteTo(_writer);
        }

        public XElement CreateXElement()
        {
            return XElement.Load(new StringReader(_xmlTextBuilder.ToString()));
        }

        public XDocument CreateXDocument()
        {
            return XDocument.Load(new StringReader(_xmlTextBuilder.ToString()));
        }

        public XmlElement CreateXmlElement()
        {
            return CreateXmlDocument().DocumentElement;
        }

        public XmlDocument CreateXmlDocument()
        {
            var doc = new XmlDocument();
            doc.Load(new XmlTextReader(new StringReader(_xmlTextBuilder.ToString())));
            return doc;
        }
    }
}

#endif