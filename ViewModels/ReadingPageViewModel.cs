using EpubReaderP.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VersOne.Epub;

namespace EpubReaderP.ViewModels
{
    public class ReadingPageViewModel : ViewModelBase
    {
        public ReadingPageViewModel(Book book)
        {
            PrevPageCommand = ReactiveCommand.Create(PrevPage);
            NextPageCommand = ReactiveCommand.Create(NextPage);

            Book = book;
            EpubBook = EpubReader.ReadBook(book.FilePath);
            ReadingOrder = GetReadingOrder();
            Navigation = EpubBook.Navigation;

            if (Navigation is not null)
            {
                AddNavigationItem(EntireNavigationList, Navigation);
            }

            if (ReadingOrder.Count > Book.CurrentChapter0)
            {
                _currentChapter0 = ReadingOrder[Book.CurrentChapter0];
                _currentAnchor0 = EntireNavigationList.Find(item => item.HtmlContentFile?.Content == CurrentChapter0?.HtmlContent)?.Link?.Anchor ?? string.Empty;
            }

            if (ReadingOrder.Count > Book.CurrentChapter1)
            {
                _currentChapter1 = ReadingOrder[Book.CurrentChapter1];
                _currentAnchor1 = EntireNavigationList.Find(item => item.HtmlContentFile?.Content == CurrentChapter1?.HtmlContent)?.Link?.Anchor ?? string.Empty;
            }

            if (EpubBook.FilePath is not null)
            {
                EpubArchive = ZipFile.OpenRead(EpubBook.FilePath);
            }
        }

        public Book Book { get; set; }
        public EpubBook EpubBook { get; set; }

        public List<EpubNavigationItem>? Navigation { get; set; }

        public List<EpubNavigationItem> EntireNavigationList { get; set; } = new List<EpubNavigationItem>();

        public List<HtmlContentFileViewModel> ReadingOrder { get; set; }

        public ZipArchive? EpubArchive { get; set; }

        public int _currentPane = 0;
        public int CurrentPane
        {
            get => _currentPane;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPane, value);
                this.RaisePropertyChanged(nameof(CurrentChapterOnSelectedPane));
            }
        }

        private bool _loadStyleSheets = true;
        public bool LoadStyleSheets
        {
            get => _loadStyleSheets;
            set
            {
                if (value != _loadStyleSheets)
                {
                    this.RaisePropertyChanged(nameof(CurrentChapter0));
                    this.RaisePropertyChanged(nameof(CurrentChapter1));
                }
                this.RaiseAndSetIfChanged(ref _loadStyleSheets, value);
            }
        }

        public EpubNavigationItem? CurrentChapterOnSelectedPane
        {
            get
            {
                if (CurrentPane == 0)
                {
                    return EntireNavigationList.Find(item => item.HtmlContentFile?.Content == CurrentChapter0?.HtmlContent);
                } 
                else
                {
                    return EntireNavigationList.Find(item => item.HtmlContentFile?.Content == CurrentChapter1?.HtmlContent);
                }
            }
            set
            {
                if (CurrentPane == 0)
                {
                    CurrentChapter0 = ReadingOrder.Find(item => item.HtmlContent == value?.HtmlContentFile?.Content);
                    CurrentAnchor0 = value?.Link?.Anchor ?? string.Empty;
                }
                else
                {
                    CurrentChapter1 = ReadingOrder.Find(item => item.HtmlContent == value?.HtmlContentFile?.Content);
                    CurrentAnchor1 = value?.Link?.Anchor ?? string.Empty;
                }
            }
        }

        public ICommand NextPageCommand { get; }
        private void NextPage()
        {
            if (CurrentPane == 0)
            {
                if (Book.CurrentChapter0 < ReadingOrder.Count - 1)
                {
                    CurrentChapter0 = ReadingOrder[Book.CurrentChapter0 + 1];
                }
            }
            else
            {
                if (Book.CurrentChapter1 < ReadingOrder.Count - 1)
                {
                    CurrentChapter1 = ReadingOrder[Book.CurrentChapter1 + 1];
                }
            }
            this.RaisePropertyChanged(nameof(CurrentChapterOnSelectedPane));
        }

        public ICommand PrevPageCommand { get; }
        private void PrevPage()
        {
            if (CurrentPane == 0)
            {
                if (Book.CurrentChapter0 > 1)
                {
                    CurrentChapter0 = ReadingOrder[Book.CurrentChapter0 - 1];
                }
            }
            else
            {
                if (Book.CurrentChapter1 > 1)
                {
                    CurrentChapter1 = ReadingOrder[Book.CurrentChapter1 - 1];
                }
            }
            this.RaisePropertyChanged(nameof(CurrentChapterOnSelectedPane));
        }

        private HtmlContentFileViewModel? _currentChapter0;
        public HtmlContentFileViewModel? CurrentChapter0 { 
            get
            {
                return _currentChapter0;
            }

            set
            {
                if (value is null) return;
                this.RaiseAndSetIfChanged(ref _currentChapter0, value);
                
                int chapter = ReadingOrder.FindIndex(navItem => navItem == value);
                if (Book.CurrentChapter0 != chapter)
                {
                    Book.CurrentChapter0 = chapter;
                    SaveBookAsync(Book);
                }
            }
        }

        private string _currentAnchor0 = string.Empty;
        public string CurrentAnchor0
        {
            get => _currentAnchor0;
            set => this.RaiseAndSetIfChanged(ref _currentAnchor0, value);
        }

        private HtmlContentFileViewModel? _currentChapter1;
        public HtmlContentFileViewModel? CurrentChapter1
        {
            get
            {
                return _currentChapter1;
            }

            set
            {
                if (value is null) return;
                this.RaiseAndSetIfChanged(ref _currentChapter1, value);
                
                if (value is null) return;
                int chapter = ReadingOrder.FindIndex(navItem => navItem == value);
                if (Book.CurrentChapter1 != chapter)
                {
                    Book.CurrentChapter1 = chapter;
                    SaveBookAsync(Book);
                }
            }
        }

        private string _currentAnchor1 = string.Empty;
        public string CurrentAnchor1
        {
            get => _currentAnchor1;
            set => this.RaiseAndSetIfChanged(ref _currentAnchor1, value);
        }

        internal List<HtmlContentFileViewModel> GetReadingOrder()
        {
            Dictionary<string, byte[]> images = EpubBook.Content.Images.Local.ToDictionary(imageFile => imageFile.Key, imageFile => imageFile.Content);
            Dictionary<string, string> styleSheets = EpubBook.Content.Css.Local.ToDictionary(cssFile => cssFile.Key, cssFile => cssFile.Content);
            Dictionary<string, byte[]> fonts = EpubBook.Content.Fonts.Local.ToDictionary(fontFile => fontFile.Key, fontFile => fontFile.Content);

            List<HtmlContentFileViewModel> ret = new();
            foreach(EpubLocalTextContentFile htmlFile in EpubBook.ReadingOrder)
            {
                ret.Add(new HtmlContentFileViewModel(htmlFile.Key, htmlFile.FilePath, htmlFile.Content, images, styleSheets, fonts));
            }

            return ret;
        }

        
        internal void AddNavigationItem(List<EpubNavigationItem> Target, List<EpubNavigationItem> Source)
        {
            foreach (EpubNavigationItem nestedNavigationItem in Source)
            {
                if (nestedNavigationItem.Link is not null)
                {
                    Target.Add(nestedNavigationItem);
                }
                AddNavigationItem(Target, nestedNavigationItem.NestedItems);
            }
        }

        public async void SaveBookAsync(Book book)
        {
            string SaveLocation = Path.Combine(Constants.BOOKS_DATA_FOLDER, book.Title + book.Id + ".json");

            await ItemLoader.SaveItemAsync(book, SaveLocation);
        }
    }
}
