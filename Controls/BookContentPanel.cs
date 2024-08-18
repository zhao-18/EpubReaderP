using Avalonia;
using Avalonia.Media.Imaging;
using DynamicData;
using EpubReaderP.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Avalonia;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace EpubReaderP.Controls
{
    public class BookContentPanel : HtmlPanel
    {
        public static readonly StyledProperty<ZipArchive> EpubArchiveProperty = AvaloniaProperty.Register<BookContentPanel, ZipArchive>(nameof(EpubArchive));

        public ZipArchive EpubArchive
        {
            get => GetValue(EpubArchiveProperty);
            set => SetValue(EpubArchiveProperty, value);
        }

        public static readonly StyledProperty<HtmlContentFileViewModel> HtmlContentFileProperty = AvaloniaProperty.Register<BookContentPanel, HtmlContentFileViewModel>(nameof(HtmlContentFile));

        public HtmlContentFileViewModel HtmlContentFile
        {
            get => GetValue(HtmlContentFileProperty);
            set => SetValue(HtmlContentFileProperty, value);
        }

        public static readonly StyledProperty<string> AnchorProperty = AvaloniaProperty.Register<BookContentPanel, string>(nameof(Anchor));

        public string Anchor
        {
            get => GetValue(AnchorProperty);
            set => SetValue(AnchorProperty, value);
        }

        public static readonly StyledProperty<bool?> LoadStyleSheetsProperty = AvaloniaProperty.Register<BookContentPanel, bool?>(nameof(LoadStyleSheets));

        public bool? LoadStyleSheets
        {
            get => GetValue(LoadStyleSheetsProperty);
            set => SetValue(LoadStyleSheetsProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == AnchorProperty)
            {
                BookContentPanel Sender = (BookContentPanel)e.Sender;
                if (Sender.Anchor != string.Empty)
                {
                    Sender.ScrollToElement(Sender.Anchor);
                }
            } 
            else if (e.Property == HtmlContentFileProperty)
            {
                BookContentPanel Sender = (BookContentPanel)e.Sender;
                Sender.Text = Sender.HtmlContentFile.HtmlContent;
            }
        }

        protected override void OnImageLoad(HtmlImageLoadEventArgs e)
        {
            byte[]? imageContent = GetImageContent(e.Src);

            if (imageContent != null)
            {
                using (MemoryStream imageStream = new MemoryStream(imageContent))
                {
                    Bitmap bitmapImage = new Bitmap(imageStream);
                    e.Callback(bitmapImage);
                }
                e.Handled = true;
            }

            base.OnImageLoad(e);
        }

        protected override void OnStylesheetLoad(HtmlStylesheetLoadEventArgs e)
        {
            if (LoadStyleSheets ?? true)
            {
                string? styleSheetContent = GetStyleSheetContent(e.Src);

                if (styleSheetContent != null)
                {
                    e.SetStyleSheet = styleSheetContent;
                }
            }
            base.OnStylesheetLoad(e);
        }

        
        private byte[]? GetImageContent(string filePath)
        {
            if (HtmlContentFile.Images.TryGetValue(GetFullPath(HtmlContentFile.HtmlFilePathInEpubManifest, filePath), out byte[]? imageContent))
            {
                return imageContent;
            }
            ZipArchiveEntry? zipArchiveEntry = EpubArchive.GetEntry(GetFullPath(HtmlContentFile.HtmlFilePathInEpubArchive, filePath));
            if (zipArchiveEntry != null)
            {
                imageContent = new byte[(int)zipArchiveEntry.Length];
                using (Stream zipArchiveEntryStream = zipArchiveEntry.Open())
                using (MemoryStream memoryStream = new MemoryStream(imageContent))
                {
                    zipArchiveEntryStream.CopyTo(memoryStream);
                }
                return imageContent;
            }

            return null;
        }

        private string? GetStyleSheetContent(string filePath)
        {
            if (HtmlContentFile.StyleSheets.TryGetValue(GetFullPath(HtmlContentFile.HtmlFilePathInEpubManifest, filePath), out string? fileContent))
            {
                return fileContent;
            }
            ZipArchiveEntry? zipArchiveEntry = EpubArchive.GetEntry(GetFullPath(HtmlContentFile.HtmlFilePathInEpubArchive, filePath));
            if (zipArchiveEntry != null)
            {
                using (Stream zipArchiveEntryStream = zipArchiveEntry.Open())
                using (StreamReader streamReader = new StreamReader(zipArchiveEntryStream))
                {
                    fileContent = streamReader.ReadToEnd();
                }
                return fileContent;
            }

            return null;
        }

        private string GetFullPath(string htmlFilePath, string relativePath)
        {
            if (relativePath.StartsWith("/"))
            {
                return relativePath.Length > 1 ? relativePath.Substring(1) : string.Empty;
            }

            string BasePath = Path.GetDirectoryName(htmlFilePath) ?? string.Empty;
            if (string.IsNullOrEmpty(BasePath)) return string.Empty;

            while(relativePath.StartsWith("../"))
            {
                relativePath = relativePath.Length > 3 ? relativePath.Substring(3) : string.Empty;
                BasePath = Path.GetDirectoryName(BasePath) ?? string.Empty;
                if (string.IsNullOrEmpty(BasePath)) return string.Empty;
            }

            string fullPath = string.Concat(BasePath.Replace('\\', '/'), '/', relativePath).TrimStart('/');
            return fullPath;
        }
    }
}
