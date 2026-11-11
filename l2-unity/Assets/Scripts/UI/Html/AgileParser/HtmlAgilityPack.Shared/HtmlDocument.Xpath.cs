// Description: Html Agility Pack - HTML Parsers, selectors, traversors, manupulators.
// Website & Documentation: https://html-agility-pack.net
// Forum & Issues: https://github.com/zzzprojects/html-agility-pack
// License: https://github.com/zzzprojects/html-agility-pack/blob/master/LICENSE
// More projects: https://zzzprojects.com/
// Copyright © ZZZ Projects Inc. All rights reserved.

#if !METRO

using System.Xml.XPath;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlAgilityPack
{
    public partial class HtmlDocument : IXPathNavigable
    {
        /// <summary>
        /// Creates a new XPathNavigator object for navigating this HTML document.
        /// </summary>
        /// <returns>An XPathNavigator object. The XPathNavigator is positioned on the root of the document.</returns>
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(this, _documentnode);
        }
    }
}

#endif