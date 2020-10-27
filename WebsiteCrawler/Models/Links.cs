using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Models
{
    public class Links
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public int Crawled { get; set; }
    }
}
