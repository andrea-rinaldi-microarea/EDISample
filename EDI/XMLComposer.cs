using System.Xml.Linq;

namespace EDI
{
    public class XMLComposer
    {
        private Process process = null;
        private Interpreter interpreter = null;
        private XNamespace maxs;
        private XElement root;
        private XElement data;

        public XMLComposer(string rootTag, Process p, Interpreter i)
        {
            process = p;
            interpreter = i;

            maxs = $"http://www.microarea.it/Schema/2004/Smart/ERP/Items/Items/AllUsers/{process.profile}.xsd";             

            data = new XElement("Data");
            root = new XElement(maxs + rootTag,
                new XAttribute(XNamespace.Xmlns + "maxs", maxs.ToString()),
                new XAttribute("tbNamespace", process.document),
                new XAttribute("xTechProfile", process.profile),
                data
            );
        }

        private Mapping GetMapping(string path)
        {
            return process.mappings.Find(m => m.target == path);
        }

        public void AddField(XElement node, string tag)
        {
            var map = GetMapping($"{node.Name}/{tag}");
            if (map != null)
            {
                var value = interpreter.GetValue(map.rule);
                if (value != null)
                {
                    node.Add(new XElement(maxs + tag, value));
                }
            }
        }

        public void AddNode(XElement node)
        {
            data.Add(node);
        }

        public XDocument GetDocument()
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }
    }
}