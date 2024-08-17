using EpubReaderP.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;

namespace EpubReaderP.ViewModels
{
    public class ReadingPageViewModel : ViewModelBase
    {
        public ReadingPageViewModel(Book book)
        {
            Book = book;
            EpubBook = EpubReader.ReadBook(book.FilePath);
            Navigation = EpubBook.Navigation;

            if (Navigation is not null)
            {
                AddNavigationItem(EntireNavigationList, Navigation);

                _currentChapter0 = EntireNavigationList[Book.CurrentChapter0];
                UpdateCurrentContent(0);

                _currentChapter1 = EntireNavigationList[Book.CurrentChapter1];
                UpdateCurrentContent(1);
            }
        }

        public Book Book { get; set; }
        public EpubBook EpubBook { get; set; }

        public List<EpubNavigationItem>? Navigation { get; set; }

        public List<EpubNavigationItem> EntireNavigationList { get; set; } = new List<EpubNavigationItem>();

        public List<string> Chapters
        {
            get => Navigation?[0].NestedItems.Select(item => item.Title).ToList() ?? new List<string>();
        }

        public int _currentPane = 0;
        public int CurrentPane
        {
            get => _currentPane;
            set => this.RaiseAndSetIfChanged(ref _currentPane, value);
        }

        public EpubNavigationItem? CurrentChapterOnSelectedPane
        {
            get
            {
                if (CurrentPane == 0)
                {
                    return CurrentChapter0;
                } 
                else
                {
                    return CurrentChapter1;
                }
            }
            set
            {
                if (CurrentPane == 0)
                {
                    CurrentChapter0 = value;
                }
                else
                {
                    CurrentChapter1 = value;
                }
            }
        }

        private EpubNavigationItem? _currentChapter0;
        public EpubNavigationItem? CurrentChapter0 { 
            get
            {
                return _currentChapter0;
            }

            set
            {
                if (value is null) return;
                if (value.Link is null) return;
                this.RaiseAndSetIfChanged(ref _currentChapter0, value);

                int chapter = EntireNavigationList.FindIndex(navItem => navItem == value);
                if (Book.CurrentChapter0 != chapter)
                {
                    Book.CurrentChapter0 = chapter;
                    UpdateCurrentContent(0);
                    SaveBookAsync(Book);
                }
            }
        }

        private string _currentChapter0Content = string.Empty;
        public string CurrentChapter0Content
        {
            get
            {
                return _currentChapter0Content;
            }
            private set
            {
                EpubNavigationItemLink? NavigationItemLink = CurrentChapter0?.Link;
                if (NavigationItemLink is null) return;

                int chapter = EntireNavigationList.FindIndex(navItem => navItem == CurrentChapter0);
                this.RaiseAndSetIfChanged(ref _currentChapter0Content, EpubBook.Content.Html.Local[chapter].Content);
            }
        }

        private EpubNavigationItem? _currentChapter1;
        public EpubNavigationItem? CurrentChapter1
        {
            get
            {
                return _currentChapter1;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _currentChapter1, value);

                if (value is null) return;
                int chapter = EntireNavigationList.FindIndex(navItem => navItem == value);
                if (Book.CurrentChapter1 != chapter)
                {
                    Book.CurrentChapter1 = chapter;
                    UpdateCurrentContent(1);
                    SaveBookAsync(Book);
                }
            }
        }

        private string _currentChapter1Content = string.Empty;
        public string CurrentChapter1Content
        {
            get
            {
                return _currentChapter1Content;
            }
            private set
            {
                EpubNavigationItemLink? NavigationItemLink = CurrentChapter1?.Link;
                if (NavigationItemLink is null) return;

                int chapter = EntireNavigationList.FindIndex(navItem => navItem == CurrentChapter1);
                this.RaiseAndSetIfChanged(ref _currentChapter1Content, EpubBook.Content.Html.Local[chapter].Content);
            }
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

        internal void UpdateCurrentContent(int paneNumber)
        {
            if (paneNumber == 0)
            {
                CurrentChapter0Content = string.Empty;
            } else if (paneNumber == 1)
            {
                CurrentChapter1Content = string.Empty;
            }
        }

        public async void SaveBookAsync(Book book)
        {
            string SaveLocation = Path.Combine(Constants.BOOKS_DATA_FOLDER, book.Title + book.Id + ".json");

            await ItemLoader.SaveItemAsync(book, SaveLocation);
        }
    }
}
