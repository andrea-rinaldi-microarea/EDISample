using System;
using System.IO;
using System.Text;
using CsvHelper;

namespace EDI
{
    public class Interpreter : IDisposable
    {
        private CsvReader reader = null;
        private TextReader stream = null;
        private Message message = null; 

        public Interpreter(Message msg, string fname)
        {
            message = msg; 
            stream = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(),"misc", fname)); 
            reader = new CsvReader(stream);

            reader.Configuration.Encoding = Encoding.UTF8;
            reader.Configuration.Delimiter = ";";
            reader.Read(); // skip the header
        }

        private int ValueIndex(string[] nspace)
        {
            var seg = message.layout.detail.IndexOf(new Segment { name = nspace[0] });
            if (seg == -1)
                return seg; // segment not found
            
            return message.layout.detail[seg].fields.IndexOf(new Field{ name = nspace[1]});
        }

        public string GetValue(Rule rule)
        {
            if (rule.type == "data")
            {
                var nspace = rule.value.Split('.');
                var valIdx = ValueIndex(nspace);
                if (valIdx != -1)
                    return reader[valIdx];
                else
                    return null;
            }
            else if (rule.type == "literal")
            {
                return rule.value;
            }
            
            return null;
        }

        public bool MoreMessages()
        {
            return reader.Read();
        }

#region IDisposable   
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if(disposing)
            {
                stream.Dispose();
                reader.Dispose();
                disposed = true;
            }
        }
#endregion
    }
}