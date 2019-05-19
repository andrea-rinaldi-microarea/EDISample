using System.Collections.Generic;

namespace EDI
{
    public class Layout 
    {
        public string name {get; set; }
        public List<Segment> heading { get; set;}
        public List<Segment> detail { get; set;}
        public List<Segment> summary { get; set;}
    }
}