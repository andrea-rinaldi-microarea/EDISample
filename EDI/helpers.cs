using System;
using System.IO;
using EDI.models;
using Newtonsoft.Json;

namespace EDI
{
    public static class Helpers
    {
        private static string MetadataPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(),"EDI","metadata");
        }

        public static Message LoadMessage(string name)
        {
            try
            {
                return JsonConvert.DeserializeObject<Message>(
                    File.ReadAllText(Path.Combine(MetadataPath(), name + ".message.json"))
                ); 
            }
            catch(System.Exception e)
            {
                Console.WriteLine("Exception occurred: " + e.Message);
                return null;
            }
        }

        public static Process LoadProcess(string name)
        {
            try
            {
                return JsonConvert.DeserializeObject<Process>(
                    File.ReadAllText(Path.Combine(MetadataPath(), name + ".process.json"))
                ); 
            }
            catch(System.Exception e)
            {
                Console.WriteLine("Exception occurred: " + e.Message);
                return null;
            }
        }
    }
}