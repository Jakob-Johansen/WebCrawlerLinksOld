using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            Console.WriteLine("Crawler Starting\n");
        }

        public void LoadCrawler()
        {
            if (_url == null || _url.Length == 0)
            {
                Console.WriteLine("Dit link er ugyldit");
                return;
            }

            // FIX!
            // && !item.Link.Contains("@") && !item.Link.Contains("javascript:void(") && !item.Link.Contains(":+")
            //!item.Link.Contains("//") && !item.Link.Contains("http") && !item.Link.Contains("jpg") && !item.Link.Contains("png") && !item.Link.Contains("jpeg") && item.Crawled == 0

            if (dbStuff.CheckIfExist(_url))
            {
                var nextLink = dbStuff.GetNextNotCrawled();

                if (nextLink.Id == 0)
                {
                    Console.WriteLine("\nCrawler Done");
                    return;
                }

                if (ValidateFilter(nextLink.Link) && !nextLink.Link.Contains("//") && !nextLink.Link.Contains("http") && !nextLink.Link.Contains("jpg") && !nextLink.Link.Contains("jpeg") && !nextLink.Link.Contains("png") && nextLink.Crawled == 0)
                {
                    Console.WriteLine("Crawled Id: " + nextLink.Id);
                    Console.WriteLine("Crawled Link: " + nextLink.Link);
                    dbStuff.UpdateLink(nextLink.Id, 1);
                    StartWebCrawler(_url + nextLink.Link).Wait();
                }
                else
                {
                    dbStuff.UpdateLink(nextLink.Id, 2);
                    LoadCrawler();
                }
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

            //var keyWordTags = htmlDocument.DocumentNode.Descendants("meta").ToList().Where(node => node.Attributes.Any(x => x.Name.Equals("name") && x.Value.Equals("keywords")));
            //string[] keyWordTagsArray = keyWordTags.FirstOrDefault().ChildAttributes("content").FirstOrDefault().Value.Split(',');
            //List<string> keyWordList = new List<string>();

            var filteredLinks = htmlDocument.DocumentNode.Descendants().Where(node => node.Name.Equals("a")).ToList().Where(x => x.Attributes.Any(t => t.Name.Equals("href")));
            List<Links> linkList = new List<Links>();

            foreach (var item in filteredLinks)
            {
                var itemLink = item.ChildAttributes("href").FirstOrDefault().Value;
                if (!dbStuff.CheckIfExist(itemLink))
                {
                    Links linkModel = new Links()
                    {
                        Link = itemLink,
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
                if (!dbStuff.CheckIfExist(item.Link) && item.Link.Length > 1)
                {
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.allowautoredirect?redirectedfrom=MSDN&view=netcore-3.1#System_Net_HttpWebRequest_AllowAutoRedirect

                    if (ValidateFilter(item.Link))
                    {
                        dbStuff.InputLink(item, 0);
                    }
                    else
                    {
                        dbStuff.InputLink(item, 2);
                    }
                }
            }

            LoadCrawler();
        }

        public bool ValidateFilter(string link)
        {
            string extension = Path.GetExtension(link);

            if (link.Contains(":") && !link.Contains("/"))
            {
                return false;
            }

            if (link.Contains("/") || extension.Length > 2)
            {
                return true;
            }

            return false;
        }

        //public void SortLinks(List<Links> links)
        //{
        //    foreach (var item in links)
        //    {
        //        if (!dbStuff.CheckIfExist(item.Link) && item.Link.Length > 2)
        //        {
        //            dbStuff.InputLink(item, 0);
        //        }
        //    }

        //    LoadCrawler();
        //}

    }
}
