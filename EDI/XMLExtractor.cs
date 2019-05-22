using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EDI
{
    public delegate string CustomExtractor(string target, string value);

    public class XMLExtractor
    {
        string[] files;
        int currFile = 0;
        private XDocument document = null;
        private Process process = null;
        CustomExtractor custom = null;

        public XMLExtractor(Process proc, CustomExtractor cust)
        {
            process = proc;
            custom = cust;
            //@@todo invoke MagicLink web services
            files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),"misc"), process.profile + "*.xml");
        }

        // clear namespace declaration to simplify XPath searches
        private void ClearNamespace()
        {
            document.Descendants()
                .Attributes()
                .Where( x => x.IsNamespaceDeclaration )
                .Remove();

            foreach (var elem in document.Descendants())
                elem.Name = elem.Name.LocalName;
        }

        public bool MoreMessages()
        {
            document = null;
            if (currFile > files.GetUpperBound(0))
                return false;

            document = XDocument.Load(files[currFile]);
            ClearNamespace();
            currFile++;
            return true;
        }

        public string GetValue(string root, Mapping map)
        {
            if (map.rule.type == "data" || map.rule.type == "custom")
            {
                IEnumerable<XElement> elem = document.XPathSelectElements($"{root}/{map.rule.value}");
                try
                {
                    var value = elem.Single().Value;
                    if (map.rule.type == "data")
                        return value;
                    else // "custom"
                        return custom(map.target, value);
                }
                catch (System.Exception ex)
                {
                    // @@todo report an error if multiple results
                    return null;
                }
            }
            else if (map.rule.type == "literal")
            {
                return map.rule.value;
            }
            
            return null;
        }


    }
}