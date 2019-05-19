using System;
using System.Collections.Generic;

namespace EDI
{
    public class Field : IEquatable<Field>
    {
        public string name {get; set;}
        public string title {get; set;}
        public string type {get; set;}
        public bool mandatory {get; set;}
        public int maxLength {get; set;}
        public int decimals {get; set;}
        public string predefined {get; set;}
        public List<string> values {get; set;}

        public bool Equals(Field match)
        {
            return name == match.name;
        }
    }
}