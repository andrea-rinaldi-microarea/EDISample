using System;
using System.IO;
using System.Text;
using CsvHelper;
using EDI;
using Newtonsoft.Json;

namespace EDISample
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = Helpers.LoadMessage("KLI_Articoli");
            var process = Helpers.LoadProcess("BRESSANA");

            using (Interpreter interpreter = new Interpreter(message, "item-data.csv")) 
            {
                while (interpreter.MoreMessages())
                {
                    foreach (EDI.Mapping map in process.mappings)
                    {
                        var value = interpreter.GetValue(map.rule);
                        if (value != null)
                        {
                            Console.WriteLine(map.target + " = " + value);
                        }
                        else
                        {
                            Console.WriteLine(map.target + " = [empty]");
                        }
                    }
                }
            }
        }
    }
}
