using System.Collections.Generic;

namespace EDI.models
{
    public class Segment
    {
        public string name {get; set;}
        public string title {get; set;}
        public bool mandatory {get; set;}
        public int maxOccurs {get; set;}
        public List<Field> fields {get; set;}
    }
}