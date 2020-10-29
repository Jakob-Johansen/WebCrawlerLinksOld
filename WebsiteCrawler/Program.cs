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
            // KIG PÅ SORTLIKS FILTER IGEN!

            // LAV SÅ DEN OGSÅ KAN CRAWLE JAVASCRIPT SIDER.

            Crawler crawler = new Crawler("http://monosoft.dk/"); 
            await crawler.LoadCrawler();
        }
    }
}
