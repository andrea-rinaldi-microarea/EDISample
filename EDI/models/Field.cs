using System.Collections.Generic;

namespace EDI.models
{
    public class Field 
    {
        public string name {get; set;}
        public string title {get; set;}
        public string type {get; set;}
        public bool mandatory {get; set;}
        public int maxLength {get; set;}
        public int decimals {get; set;}
        public string predefined {get; set;}
        public List<string> values {get; set;}
    }
}