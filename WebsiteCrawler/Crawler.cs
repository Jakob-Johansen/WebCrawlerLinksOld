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

        public  async Task LoadCrawler()
        {
            if (_url == null || _url.Length == 0)
            {
                Console.WriteLine("Dit link er ugyldit");
                return;
            }

            if (dbStuff.CheckIfExist(_url))
            {
                var nextLink = dbStuff.GetNextNotCrawled();

                if (nextLink.Id == 0)
                {
                    Console.WriteLine("\nCrawler Done");
                    return;
                }

                //ValidateFilter(nextLink.Link) && !nextLink.Link.Contains("//") && !nextLink.Link.Contains("http") && !nextLink.Link.Contains("jpg") && !nextLink.Link.Contains("jpeg") && !nextLink.Link.Contains("png") && nextLink.Crawled == 0
                if (ValidateFilter(nextLink.Link) && !nextLink.Link.Contains("//") && !nextLink.Link.Contains("http") && !nextLink.Link.Contains(".jpg") && !nextLink.Link.Contains(".jpeg") && !nextLink.Link.Contains(".png") && !nextLink.Link.Contains(".mp3") && !nextLink.Link.Contains(".mp4") && !nextLink.Link.Contains(".exe") && nextLink.Crawled == 0)
                {
                    Console.WriteLine("Crawled Id: " + nextLink.Id);
                    Console.WriteLine("Crawled Link: " + nextLink.Link);
                    dbStuff.UpdateLink(nextLink.Id, 1);
                    await StartWebCrawler(nextLink);
                }
                else
                {
                    dbStuff.UpdateLink(nextLink.Id, 2);
                    await LoadCrawler();
                }
            }
            else
            {
                await SortLinks(_url);
            }
        }

        public async Task StartWebCrawler(Links thisModel)
        {
            string url;

            // ---
            if (_url == thisModel.Link)
                url = _url;
            else
                url = _url + thisModel.Link.Trim();
            // ---

            HttpClient httpClient = new HttpClient();

            try
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);

                if (httpResponseMessage.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    dbStuff.UpdateLink(thisModel.Id, 2);
                    await LoadCrawler();
                }
                else
                {
                    var html = await httpClient.GetStringAsync(url);

                    HtmlDocument htmlDocument = new HtmlDocument();

                    htmlDocument.LoadHtml(html);

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
                        await LoadCrawler();
                    }

                    await SortLinks(linkList);
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Der skete en fejl");
                Console.WriteLine(url);
                return;
            }
        }

        public async Task SortLinks(string link)
        {
            if (link.Contains("http"))
            {
                Links linkModel = new Links() {
                    Link = link,
                    Crawled = 1
                };

                dbStuff.InputLink(linkModel, 1);
                await StartWebCrawler(linkModel);
            }
        }

        public async Task SortLinks(List<Links> links)
        {
            foreach (var item in links)
            {

                if (!dbStuff.CheckIfExist(item.Link) && item.Link.Length > 1)
                {
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
            
           await LoadCrawler();
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

        public async Task CheckIfMovedPerm(Links item)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage test = await httpClient.GetAsync(_url + item.Link);
                if (test.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    dbStuff.InputLink(item, 2);
                }
                else
                {
                    dbStuff.InputLink(item, 0);
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Der skete en fejl");
                Console.WriteLine(item.Link);
            }
        }
    }
}
