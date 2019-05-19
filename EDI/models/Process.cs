using System.Collections.Generic;

namespace EDI
{
    public class Process
    {
        public string title {get; set; }
        public string type {get; set; }
        public string message {get; set; }
        public string document {get; set; }
        public string profile {get; set; }
        public Roots roots {get; set;}
        public List<Mapping> mappings {get; set;}
    }
}