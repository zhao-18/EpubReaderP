using System.Collections.ObjectModel;
using System.IO;
using EpubReaderP.Models;
using ReactiveUI;
using System.Reactive.Concurrency;
using VersOne.Epub;
using EpubReaderP.Views;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;
using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace EpubReaderP.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        RxApp.MainThreadScheduler.Schedule(LoadLibraryAsync);
        FilePicker.FilePickedEvent += AddNewBook;

        IObservable<bool> canRemoveBook = this.WhenAnyValue(x => x.CanRemove);
        RemoveSelectedBookCommand = ReactiveCommand.Create(RemoveSelectedBook, canRemoveBook);
    }

    public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();

    private Book? _selectedBook;
    public Book? SelectedBook
    {
        get => _selectedBook;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBook, value);
            UpdateCanRemove();
        }
    }

    public ICommand RemoveSelectedBookCommand { get; }
    public void RemoveSelectedBook()
    {
        if (SelectedBook is null) return;
        RemoveBook(SelectedBook);
    }

    private bool _canRemove = false;
    public bool CanRemove { 
        get => _canRemove;
        set => this.RaiseAndSetIfChanged(ref _canRemove, value);
    }
    public void UpdateCanRemove() => CanRemove = SelectedBook != null;

    public void RemoveBook(Book book)
    {
        Books.Remove(book);

        string BookCacheFile = Path.Combine(Constants.BOOKS_DATA_FOLDER, book.Title + book.Id + ".json");
        if (File.Exists(BookCacheFile))
        {
            File.Delete(BookCacheFile);
        }

        string BookCoverCacheFile = Path.Combine(Constants.COVER_IMAGE_FOLDER, book.Title + book.Id + ".bmp");
        if (book.HasCover && File.Exists(BookCacheFile))
        {
            File.Delete(BookCoverCacheFile);
        }
    } 

    public FilePickerViewModel FilePicker { get; set; } = new FilePickerViewModel();
    private void AddNewBook(object? sender, FilePickedEventArgs e)
    {
        try
        {
            using EpubBookRef epubBook = EpubReader.OpenBook(e.File);
            byte[]? coverImage = epubBook.ReadCover();

          
            Book book = new Book(Books.Count, e.File, epubBook.Title, epubBook.Author, null);
            if (coverImage is null)
            {
                book.Cover = new Bitmap(AssetLoader.Open(new Uri(Constants.GENERIC_COVER_IMAGE_SOURCE)));
                Books.Add(book);
                SaveBookAsync(book);
                return;
            }

            MemoryStream CoverStream = new MemoryStream(coverImage);
            Bitmap CoverBitmap = Task.Run(() => Bitmap.DecodeToHeight(CoverStream, Constants.COVER_MAX_HEIGHT)).Result;
            book.Cover = CoverBitmap;
            book.HasCover = true;
            Books.Add(book);
            SaveBookAsync(book);

            using (Stream fs = File.OpenWrite(Path.Combine(Constants.COVER_IMAGE_FOLDER, book.Title + book.Id + ".bmp")))
            {
                CoverBitmap.Save(fs);
            }
        } 
        catch (System.Xml.XmlException x)
        {
            Debug.WriteLine($"Something is wrong with the book");
            Debug.WriteLine(x.Message);
            return;
        }
    }


    public async void LoadLibraryAsync()
    {
        if (!Directory.Exists(Constants.COVER_IMAGE_FOLDER))
        {
            Directory.CreateDirectory(Constants.COVER_IMAGE_FOLDER);
        }

        if (!Directory.Exists(Constants.BOOKS_DATA_FOLDER))
        {
            Directory.CreateDirectory(Constants.BOOKS_DATA_FOLDER);
            return;
        }

        foreach (string file in Directory.EnumerateFiles(Constants.BOOKS_DATA_FOLDER))
        {
            if (new DirectoryInfo(file).Extension != ".json") continue;
            Book? book = await ItemLoader.LoadItemAsync<Book>(file);

            if (book is null) continue;
            book.Cover = Task.Run(async () => await book.LoadCoverFromCache()).Result;
            Books.Add(book);
        }
    }

    public async void SaveBookAsync(Book book)
    {
        string SaveLocation = Path.Combine(Constants.BOOKS_DATA_FOLDER, book.Title + book.Id + ".json");

        await ItemLoader.SaveItemAsync(book, SaveLocation);
    }
}
