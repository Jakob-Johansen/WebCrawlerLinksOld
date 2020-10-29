using System;
using System.Linq;

namespace WebsiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler crawler = new Crawler("https://www.adidas.dk/"); //https://www.automobile.tn
            crawler.LoadCrawler();

            //Test test = new Test("http://www.monosoft.dk/");
            //test.LoadCrawler();

        }
    }
}
