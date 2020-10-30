using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebsiteCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // https://www.automobile.tn
            // https://www.apple.com/
            // https://www.proshop.dk/
            // http://monosoft.dk/

            // KIG PÅ DBSTUFF!
            // KIG PÅ FILTERED I LOADCRAWLER IGEN!

            // LAV SÅ DEN OGSÅ KAN CRAWLE JAVASCRIPT SIDER.
            // LAV SÅ DEN TJEKKER OM DER KOMMER SKRÅSTREG EFTER LINKET, OG TJEK OM DER ER SKRÅSTREG FØR CRAWLED UNDERLINK.
            Crawler crawler = new Crawler("https://www.adidas.dk/");
            await crawler.LoadCrawler();
        }
    }
}
