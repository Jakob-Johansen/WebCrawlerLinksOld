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

            // Javascript side
            // https://madfaerd.org/

            // KIG PÅ DBSTUFF!
            // KIG PÅ FILTERED I LOADCRAWLER IGEN!

            // LÆS BEST PRATCIS Puppeteer. -DONE
            // LAV SÅ DEN IKKE ÅBNER OG LUKKER NYE PUPPETEER BROWSERE MEN KUN NYE PAGES HVOR DEN LUKKER DE GAMLE -DONE

            // LÆR OM EVENTS I C#

            // LAV SÅ DEN TJEKKER OM DER ER ET SITEMAP FØR DEN GÅR IGANG MED AT HENTE LINKS, ELLERS SKAL DEN BRUGE SITEMAPPET.
            // LAV SÅ DEN TJEKKER OM DER KOMMER SKRÅSTREG EFTER LINKET, OG TJEK OM DER ER SKRÅSTREG FØR CRAWLED UNDERLINK.

            // MÅSKE, GØR SÅ DEN CRAWLER FLERE SIDER PÅ EN GANG.

            //TILFØJ ROBOTS:TXT READER OG XML READER TIL PROJEKTET.


            //Crawler crawler = new Crawler("http://monosoft.dk/");
            //await crawler.LoadCrawler();

            Test test = new Test("https://www.automobile.tn");
            await test.LoadCrawler();
        }
    }
}
