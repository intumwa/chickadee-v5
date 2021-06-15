using NLog;
using System.Text.RegularExpressions;

namespace Chickadee
{
    public class FileHelper
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static string BuildFilePath(WebPageVisit visit)
        {
            Regex regex = new Regex("[^\\w]");
            string[] filePaths = { @"/home/intumwa/workspace/chickadee-pages", regex.Replace(visit.Url, "_"), visit.FilePath };
            var fullFilePath = System.IO.Path.Combine(filePaths);

            return fullFilePath;
        }
    }
}
