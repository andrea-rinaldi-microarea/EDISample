using System.IO;
using System.Xml.Linq;

namespace EDI
{
    public class XMLExtractor
    {
        string[] files;
        int currFile = 0;
        public XDocument doc = null;

        public XMLExtractor(string fnames)
        {
            files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),"misc"), fnames + ".xml");
        }

        public bool MoreMessages()
        {
            doc = null;
            if (currFile > files.GetUpperBound(0))
                return false;
            doc = XDocument.Load(files[currFile]);
            currFile++;
            return true;
        }

        public string GetValue(Rule rule)
        {
            if (rule.type == "data")
            {
                //@@ todo xpath
                return "a";
            }
            else if (rule.type == "literal")
            {
                return rule.value;
            }
            
            return null;
        }


    }
}