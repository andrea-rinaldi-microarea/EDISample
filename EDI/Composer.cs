namespace EDI
{
    public class Composer
    {
        private Message message = null; 
        private Process process = null;

        public Composer(Message msg, Process proc)
        {
            message = msg;
            process = proc;
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

                foreach (var field in segment.fields)
                {
                    //@@ TODO predefined

                    var map = GetMapping($"{segment.name}.{field.name}");

                    if (map == null)
                    {
                        if (field.mandatory)
                            break; // TODO error
                        else
                            continue;
                    }
                    
                    var value = extractor.GetValue(map.rule);
                    if (value != null)
                    {
                        //@@ todo add to buffer
                    }
                    else
                    {
                        // apply default
                    }
                }
            }
        }

        private bool HasMappings(Segment segment)
        {
            return process.mappings.Find(m => m.target.StartsWith($"{segment.name}")) != null;
        }

        private Mapping GetMapping(string path)
        {
            return process.mappings.Find(m => m.target == path);
        }
    }
}