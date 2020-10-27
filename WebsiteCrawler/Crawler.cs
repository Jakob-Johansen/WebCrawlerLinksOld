using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Models;

namespace WebsiteCrawler
{
    public class Crawler
    {
        private DbStuff dbStuff = new DbStuff();

        private readonly string _url;

        public Crawler(string url)
        {
            _url = url.Trim();
        }

        public void LoadCrawler()
        {
            if (_url == null || _url.Length == 0)
            {
                Console.WriteLine("Dit link er ikke gyldigt");
            }
            else
            {
                if (dbStuff.CheckIfExist(_url)) // Er true.
                {
                    Links item = dbStuff.GetNextNotCrawled();

                    if (!item.Link.Contains("//") && !item.Link.Contains("http") && !item.Link.Contains("jpg") && !item.Link.Contains("png") && !item.Link.Contains("jpeg") && item.Crawled != 1)
                    {
                        dbStuff.UpdateLink(item.Id);
                        Console.WriteLine("Crawled Id: " + item.Id);
                        StartWebCrawler(_url + item.Link).Wait();
                    }
                    else
                    {
                        dbStuff.UpdateLink(item.Id);
                        LoadCrawler();
                    }
                }
                else
                {
                    Console.WriteLine("De er ikke ens");
                    SortLinks(_url);
                }
            }
        }

        public async Task StartWebCrawler(string url)
        {
            url = url.Trim();

            HttpClient httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html);

            //var keyWordTags = htmlDocument.DocumentNode.Descendants("meta").ToList().Where(node => node.Attributes.Any(x => x.Name.Equals("name") && x.Value.Equals("keywords")));
            //string[] keyWordTagsArray = keyWordTags.FirstOrDefault().ChildAttributes("content").FirstOrDefault().Value.Split(',');
            //List<string> keyWordList = new List<string>();

            var filteredLinks = htmlDocument.DocumentNode.Descendants().Where(node => node.Name.Equals("a")).ToList().Where(x => x.Attributes.Any(t => t.Name.Equals("href")));
            List<Links> linkList = new List<Links>();

            foreach (var item in filteredLinks)
            {
                var test = item.ChildAttributes("href").FirstOrDefault().Value;
                if (!dbStuff.CheckIfExist(test))
                {
                    Links linkModel = new Links()
                    {
                        Link = test,
                        Crawled = 0
                    };

                    linkList.Add(linkModel);
                }
            }

            SortLinks(linkList);
        }

        public void SortLinks(string link)
        {
            if (link.Contains("http"))
            {
                Links linkModel = new Links() {
                    Link = link,
                    Crawled = 1
                };

                dbStuff.InputLink(linkModel, 1);
                StartWebCrawler(link).Wait();
            }
        }

        public void SortLinks(List<Links> links)
        {
            foreach (var item in links)
            {
                if (!dbStuff.CheckIfExist(item.Link) && item.Link.Length > 2 && item.Link.Contains("/"))
                {
                    dbStuff.InputLink(item, 0);
                }
            }

            LoadCrawler();
        }

    }
}
