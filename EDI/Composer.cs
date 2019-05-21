using System;
using System.IO;
using NLight.IO.Text;

namespace EDI
{
    public class Composer : IDisposable
    {
        private Message message = null; 
        private Process process = null;
        private TextWriter writer = null;
        private FixedWidthRecordWriter recWriter = null;

        public Composer(Message msg, Process proc)
        {
            message = msg;
            process = proc;
            writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(),"out.txt")); //Path.GetTempFileName());
            recWriter = new NLight.IO.Text.FixedWidthRecordWriter(writer);
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

                recWriter.Columns.Clear();
                int pos = 0;
                foreach (var field in segment.fields)
                {
                    recWriter.Columns.Add(new FixedWidthRecordColumn(field.name, pos, field.maxLength));
                    pos += field.maxLength;
                }

                recWriter.WriteRecordStart();
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
                            recWriter.WriteField("default"); 
                            continue;
                        }
                    }
                    
                    var value = extractor.GetValue(map.rule);
                    if (value != null)
                    {
                        //@@ todo add to buffer
                        recWriter.WriteField(value); 
                    }
                    else
                    {
                        // apply default
                        recWriter.WriteField("default"); 
                    }
                }
                recWriter.WriteRecordEnd();
            }
            recWriter.BaseWriter.Flush();
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
                writer.Dispose();
                recWriter.Dispose();
                disposed = true;
            }
        }
#endregion
 }
}