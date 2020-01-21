using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;

namespace GKeepTransform
{
    class Program
    {
        private static string xmlPath;

        static void ProcessFiles(string path, Action<string> processingFileFunction, string ignorePath = "fd;lkjzhs;diufj;zdsfyh/clsjkdh;lfzhksdl;khsbdzfjchblsvzjghdaksjglhXKJ:x")
        {
            string[] files;
            string[] directories;

            files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (!file.Contains(ignorePath))
                {
                    processingFileFunction(file);
                }
            }

            directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                // Process each directory recursively
                ProcessFiles(directory, ignorePath: ignorePath, processingFileFunction: processingFileFunction);
            }
        }

        static void Main(string[] args)
        {
            StringBuilder str = new StringBuilder("<root>");

            List<string> notProcessed = new List<string>();

            ProcessFiles(path: @"D:\_Downloads\Google_takeout-20191230T235437Z-001\Takeout\Keep",
            processingFileFunction: (path) =>
             {
             string file = "";
             try
             {
                file = File.ReadAllText(path);
                 string body = ("<div>" + HttpUtility.HtmlEncode(Path.GetFileName(path)) + "</div>" +
                file
                .Split(new string[] { @"<body>" }, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(new string[] { @"</body>" }, StringSplitOptions.RemoveEmptyEntries)[0])
                .Replace("<br>", "")
                .Replace("&lrm;", "")
                .Replace("&frac12;", "")
                .Replace("&middot;", "")
                .Replace("&ndash;", "")
                .Replace("&ntilde;", "")
                .Replace("&ccedil;", "")
                .Replace("&ecirc;", "")
                .Replace("&ldquo;", "")
                .Replace("&rdquo;", "")
                .Replace("&rsquo;", "")
                .Replace("&trade;", "")
                ;

                 str.Append(Regex.Replace(body, @"\r\n|\n\r|\n|\r", "\r\n"));

                }
                catch(Exception ex)
                {
                    notProcessed.Add(path);
                    //Console.WriteLine(file);
                }
                
            });
            str.Append("</root>");

            using (StreamWriter sw = new StreamWriter("output.xml"))
            {
                sw.WriteLine(str.ToString());
            }

            XmlDocument doc = new XmlDocument();
            doc.Load("gkeep.xml");
            XmlNode root = doc.DocumentElement;
            doc.Save("gkeep.xml");
        }
    }
}
