using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EDI
{
    public class XMLExtractor
    {
        string[] files;
        int currFile = 0;
        private XDocument document = null;
        private Process process = null;

        public XMLExtractor(Process proc)
        {
            process = proc;
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

        public string GetValue(string root, Rule rule)
        {
            if (rule.type == "data")
            {
                IEnumerable<XElement> elem = document.XPathSelectElements($"{root}/{rule.value}");
                try
                {
                    return elem.Single().Value;
                }
                catch (System.Exception)
                {
                    // @@todo report an error if multiple results
                    return null;
                }
            }
            else if (rule.type == "literal")
            {
                return rule.value;
            }
            
            return null;
        }


    }
}