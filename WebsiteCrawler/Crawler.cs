using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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
                if (dbStuff.CheckIfExist(_url)) // Er true.
                {
                    Links item = dbStuff.GetNextNotCrawled();

                    if (item.Id != 0)
                    {

                        // && !item.Link.Contains("@") && !item.Link.Contains("javascript:void(") && !item.Link.Contains(":+")
                        if (!item.Link.Contains("//") && !item.Link.Contains("http") && !item.Link.Contains("jpg") && !item.Link.Contains("png") && !item.Link.Contains("jpeg") && !item.Link.Contains("@") && !item.Link.Contains("javascript:void(") && !item.Link.Contains(":+") && item.Crawled == 0)
                        {
                            dbStuff.UpdateLink(item.Id, 1);
                            Console.WriteLine("Crawled Id: " + item.Id);
                            Console.WriteLine("Crawled Link: " + item.Link);
                            StartWebCrawler(_url + item.Link).Wait();
                        }
                        else
                        {
                            dbStuff.UpdateLink(item.Id, 2);
                            LoadCrawler();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Crawler Done");
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
                if (!dbStuff.CheckIfExist(item.Link) && item.Link.Length > 2)
                {
                    dbStuff.InputLink(item, 0);
                }
            }

            LoadCrawler();
        }

    }
}
