using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.ReadSiteMapFolder
{
    public class ReadXml
    {
        private readonly DbStuff _dbStuff;

        public ReadXml()
        {
            _dbStuff = new DbStuff();
        }

        public void StartReadingXml(string url)
        {

            // .gz ER EN FIL TYPE. https://www.microsoft.com/ar-ly/store/ar-ly.apps.v7.1.xml.gz
            // HVAD JEG FORSTÅR ER xml.gz BARE COMPRESSED XML SÅ DET GÅR HURTIGERE FOR CRAWLEREN AT LÆSE. 

            using XmlTextReader reader = new XmlTextReader(url);
            try
            {
                //List<string> linkList = new List<string>();

                while (reader.Read())
                {
                    if (reader.Value.Contains("http") && !reader.Value.Contains(".gz"))
                    {
                        if (reader.Value.ToLower().Contains(".xml"))
                        {
                            StartReadingXml(reader.Value);
                        }
                        else
                        {
                            Links link = new Links()
                            {
                                Link = reader.Value,
                                Crawled = 3
                            };

                            Console.ForegroundColor = ConsoleColor.Blue;
                            if (!_dbStuff.CheckIfExist(link.Link))
                            {
                                Console.WriteLine(link.Link);
                                _dbStuff.InputLink(link, 3);
                            }
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("!------------------------!");
                Console.WriteLine("Kunne ikke køre den valgte url");
                Console.WriteLine(e.SourceUri);
                Console.WriteLine("!------------------------!");
                Console.BackgroundColor = ConsoleColor.Black;
            }

            reader.Close();
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
    }
}
