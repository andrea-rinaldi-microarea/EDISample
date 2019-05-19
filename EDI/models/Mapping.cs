using System;

namespace EDI
{
    public class Mapping : IEquatable<Mapping>
    {
        public string target {get; set;}
        public Rule rule {get; set;}
       
        public bool Equals(Mapping match)
        {
            return target == match.target;
        }
    }
}