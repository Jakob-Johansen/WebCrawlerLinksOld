using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.ReadSiteMapFolder
{
    public class ReadRobots
    {
        private string _robottxtUrl;
        private readonly RobotsPathCheck _robotsPathCheck;
        private readonly ReadXml _readXml;
        private readonly DbStuff _dbStuff;

        public ReadRobots()
        {
            _robotsPathCheck = new RobotsPathCheck();
            _readXml = new ReadXml();
            _dbStuff = new DbStuff();
        }

        public async Task<bool> StartReadRobots(string url)
        {
            _robottxtUrl = url + "robots.txt"; 
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Reading...\n");

            using HttpClient httpClient = new HttpClient();

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_robottxtUrl);

            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                using WebClient webClient = new WebClient();

                webClient.DownloadFile(_robottxtUrl, _robotsPathCheck.CheckFolderPath());

                if (ReadTextFile(url) == true)
                {
                    return true;
                }

                return false;
            }
            else
            {
                Console.WriteLine("No Robots.txt\n");
                return false;
            }
        }

        private bool ReadTextFile(string url)
        {
            string[] robotsText = System.IO.File.ReadAllLines(_robotsPathCheck.CheckFolderPath());

            List<string> sitemapList = new List<string>();

            foreach (var item in robotsText)
            {
                if (item.ToLower().Contains("sitemap:"))
                {
                    sitemapList.Add(item.Split(' ').Last());
                }
            }

            if (sitemapList.Count == 0 || sitemapList == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("NO SITEMAPS!\n");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Magenta;

                return false;
            }

            Links link = new Links()
            {
                Link = url,
                Crawled = 3
            };

            _dbStuff.InputLink(link, 3);

            foreach (var item in sitemapList)
            {
                _readXml.StartReadingXml(item);
            }
            return true;
        }
    }
}
