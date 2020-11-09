using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.ReadSiteMapFolder
{
    public class RobotsPathCheck
    {
        private readonly string _folderPath;

        public RobotsPathCheck()
        {
            _folderPath = GetFolderPath();
        }

        public string CheckFolderPath()
        {
            return _folderPath;
        }

        private string GetFolderPath()
        {
            string[] mainFolderPath = Directory.GetCurrentDirectory().Split('\\');

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in mainFolderPath)
            {
                if (item.ToLower() == "bin")
                {
                    break;
                }

                stringBuilder.Append(item + "\\");
            }

            stringBuilder.Append("Robots\\");

            string path = CreatePathAndFile(stringBuilder.ToString());

            return path;
        }

        private string CreatePathAndFile(string fullPath)
        {
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath + "Robots.txt";
        }
    }
}
