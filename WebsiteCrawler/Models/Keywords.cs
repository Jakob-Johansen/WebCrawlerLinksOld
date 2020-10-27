using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Models
{
    public class Keywords
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public string Keyword { get; set; }
    }
}
