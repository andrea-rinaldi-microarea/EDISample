using System;
using System.Collections.Generic;

namespace EDI
{
    public class Segment : IEquatable<Segment>
    {
        public string name {get; set;}
        public string title {get; set;}
        public bool mandatory {get; set;}
        public int maxOccurs {get; set;}
        public List<Field> fields {get; set;}

        public bool Equals(Segment match)
        {
            return name == match.name;
        }
    }
}