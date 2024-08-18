using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderP.ViewModels
{
    public class HtmlContentFileViewModel : ViewModelBase
    {
        public HtmlContentFileViewModel(string htmlFilePathInEpubManifest, string htmlFilePathInEpubArchive, string htmlContent, Dictionary<string, byte[]> images, Dictionary<string, string> styleSheets, Dictionary<string, byte[]> fonts)
        {
            HtmlFilePathInEpubManifest = htmlFilePathInEpubManifest;
            HtmlFilePathInEpubArchive = htmlFilePathInEpubArchive;
            HtmlContent = htmlContent;
            Images = images;
            StyleSheets = styleSheets;
            Fonts = fonts;
        }

        public string HtmlFilePathInEpubManifest { get; init; }
        public string HtmlFilePathInEpubArchive { get; init; }
        public string HtmlContent {  get; init; }
        public Dictionary<string, byte[]> Images { get; init; }
        public Dictionary<string, string> StyleSheets { get; init; }
        public Dictionary<string, byte[]> Fonts { get; init; }
    }
}
