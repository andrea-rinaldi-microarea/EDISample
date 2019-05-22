using System;
using System.Collections.Generic;
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
        private XMLExtractor extractor = null;

        public Composer(Message aMessage, Process aProcess, string fname, XMLExtractor aExtractor)
        {
            message = aMessage;
            process = aProcess;
            extractor = aExtractor;
            stream = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "misc", fname)); 
            writer = new NLight.IO.Text.FixedWidthRecordWriter(stream);
        }

        private string Default(Field field)
        {
            if (field.type == "text")
                return "";
            if (field.type == "numeric")
                return new string('0', field.maxLength);
            if (field.type == "boolean")
                return "N";
            return "";
        }

        private string Format(Field field, string value)
        {
            if (value == null)
                return Default(field);

            if (field.type == "text")
                return value.PadLeft(field.maxLength);
            if (field.type == "numeric")
            {
                if (field.decimals == 0)
                {
                    return value.Split('.')[0].PadLeft(field.maxLength).Replace(' ', '0');
                }
                else
                {
                    var parts = value.Split('.');
                    var padded = parts[0].PadLeft(field.maxLength - field.decimals) + 
                                 (parts.Length > 1 ? parts[1].PadRight(field.decimals) : new string('0', field.decimals));

                    return padded.Replace(' ','0');
                }
            }
            if (field.type == "boolean")
                return value == "0" ? "S" : "N";
            return "";
        }

        public void Compose()
        {
            ComposeSection(message.layout.heading);
            ComposeSection(message.layout.detail);
            ComposeSection(message.layout.summary);
        }

        private void ComposeSection(List<Segment> section)
        {
            if (section == null || section.Count == 0)
                return;

            while (extractor.MoreMessages())
            {
                foreach (var segment in section)
                {
                    if (!HasMappings(segment))
                    {
                        if (segment.mandatory)
                            break; // TODO error
                        else
                            continue; 
                    }
                    ComposeSegment(segment);
                }
                writer.BaseWriter.Flush();
            }
        }

        private void ComposeSegment(Segment segment)
        {
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

                string value = null;
                if (map != null)
                    value = extractor.GetValue(process.roots.detail, map);
                else 
                {
                    if (field.mandatory)
                        ; // TODO error
                }

                writer.WriteField(Format(field, value)); 
            }
            writer.WriteRecordEnd();
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