using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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
                    var composer = new XMLComposer("Items", process, interpreter);
                    var items = new XElement("Items");
                    composer.AddField(items,"Item");
                    composer.AddField(items,"SaleBarCode");
                    composer.AddField(items,"Description");
                    composer.AddField(items,"UseSerialNo");
                    composer.AddNode(items);

                    var goods = new XElement("GoodsData");
                    composer.AddField(goods,"NetWeight");
                    composer.AddField(goods,"GrossWeight");
                    composer.AddField(goods,"GrossVolume");
                    composer.AddNode(goods);

                    var doc = composer.GetDocument();

                    string fname = Path.GetTempFileName();
                    doc.Save(fname);
                    Console.WriteLine(File.ReadAllText(fname)); 
                }
            }
        }
    }
}
