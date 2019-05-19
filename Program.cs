using System;
using System.IO;
using EDI;
using Newtonsoft.Json;

namespace EDISample
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = Helpers.LoadMessage("EURITMO-ORDERS");
            var process = Helpers.LoadProcess("SALEORD");

            var kliArt = Helpers.LoadMessage("KLI_Articoli");
            var bressana = Helpers.LoadProcess("BRESSANA");
        }
    }
}
