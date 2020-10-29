using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler
{
    public class Test
    {
        private readonly string _url;

        public Test(string url)
        {
            _url = url.Trim();
            Console.WriteLine("Crawler Starting.");
        }

        public void LoadCrawler()
        {
            if (_url == null || _url.Length == 0)
            {
                Console.WriteLine("Dit link er ikke gyldigt");
            }
            else
            {
                SortLinks(_url);
            }
        }

        public async Task StartWebCrawler(string url)
        {
            url = url.Trim();

            HttpClient httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html);

            var filteredLinks = htmlDocument.DocumentNode.Descendants().Where(node => node.Name.Equals("a")).ToList().Where(x => x.Attributes.Any(t => t.Name.Equals("href")));
            List<Links> linkList = new List<Links>();

            foreach (var item in filteredLinks)
            {
                var test = item.ChildAttributes("href").FirstOrDefault().Value;

                Links linkModel = new Links()
                {
                    Link = test,
                    Crawled = 0
                };

                linkList.Add(linkModel);
            }

            if (linkList == null)
            {
                LoadCrawler();
            }

            SortLinks(linkList);
        }

        public void SortLinks(string link)
        {
            if (link.Contains("http"))
            {
                Links linkModel = new Links()
                {
                    Link = link,
                    Crawled = 1
                };

                StartWebCrawler(link).Wait();
            }
        }

        public void SortLinks(List<Links> links)
        {
            foreach (var item in links)
            {
                string extension = Path.GetExtension(item.Link);

                if (item.Link.Length > 1)
                {
                    if (item.Link.Contains("/") || extension.Length > 2)
                    {
                        Console.WriteLine(item.Link);
                    }
                    else
                    {
                        Console.WriteLine("***********");
                    }
                }
                else
                {
                    Console.WriteLine("!!!!!!!!!!!!!");
                }
            }
        }
    }
}
