using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using tree_matching_csharp;
using NLog;
using AngleSharp.Dom;
using Node = tree_matching_csharp.Node;

namespace Chickadee
{
    public static class WebSurfer
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        public static async Task<IDocument> GetWebPageDocument(string filePath)
        {
            var htmlContent = "";
            IDocument document;

            try
            {
                htmlContent = File.ReadAllText(filePath);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Logger.Error("The file was not found: " + fileNotFoundException.Message);
                htmlContent = "<p id=\"FileNotFoundException\">File Not Found</p>";
            }
            catch (FileLoadException fileLoadException)
            {
                Logger.Error("There was a problem loading the file: " + fileLoadException.Message);
                htmlContent = "<p id=\"FileLoadException\">Problem Loading File</p>";
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Logger.Error("The directory was not found: " + directoryNotFoundException.Message);
                htmlContent = "<p id=\"DirectoryNotFoundException\">Directory Not Found</p>";
            }
            finally
            {
                document = await DOM.WebpageToDocument(htmlContent);
            }

            return document;
        }

        public static IEnumerable<Node> GetWebPageDomTree(IDocument webPageDocument)
        {
            var domTree = DOM.DomToTree(webPageDocument);
            return domTree;
        }

    }
}
