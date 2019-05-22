using System;
using System.IO;
using NLight.IO.Text;

namespace EDI
{
    public class Composer : IDisposable
    {
        private Message message = null; 
        private Process process = null;
        private TextWriter stream = null;
        private FixedWidthRecordWriter writer = null;

        public Composer(Message msg, Process proc, string fname)
        {
            message = msg;
            process = proc;
            stream = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "misc", fname)); 
            writer = new NLight.IO.Text.FixedWidthRecordWriter(stream);
        }

        public void AddDetail(XMLExtractor extractor)
        {
            foreach (var segment in message.layout.detail)
            {
                if (!HasMappings(segment))
                {
                    if (segment.mandatory)
                        break; // TODO error
                    else
                        continue; 
                }

                writer.Columns.Clear();
                int pos = 0;
                foreach (var field in segment.fields)
                {
                    writer.Columns.Add(new FixedWidthRecordColumn(field.name, pos, field.maxLength));
                    pos += field.maxLength;
                }

                writer.WriteRecordStart();
                foreach (var field in segment.fields)
                {
                    //@@ TODO predefined

                    var map = GetMapping($"{segment.name}.{field.name}");

                    if (map == null)
                    {
                        if (field.mandatory)
                            break; // TODO error
                        else
                        {
                            writer.WriteField("default"); 
                            continue;
                        }
                    }
                    
                    var value = extractor.GetValue(process.roots.detail, map.rule);
                    if (value != null)
                    {
                        writer.WriteField(value); 
                    }
                    else
                    {
                        // apply default
                        writer.WriteField("default"); 
                    }
                }
                writer.WriteRecordEnd();
            }
            writer.BaseWriter.Flush();
        }

        private bool HasMappings(Segment segment)
        {
            return process.mappings.Find(m => m.target.StartsWith($"{segment.name}")) != null;
        }

        private Mapping GetMapping(string path)
        {
            return process.mappings.Find(m => m.target == path);
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
                writer.Dispose();
                disposed = true;
            }
        }
#endregion
 }
}