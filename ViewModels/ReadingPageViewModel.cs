using EpubReaderP.Models;
using System;
using System.Collections.Generic;
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
        }

        public void SetBook(Book book)
        {
            Book = book;
            EpubBook = EpubReader.ReadBook(book.FilePath);
        }

        Book Book { get; set; }
        EpubBook EpubBook { get; set; }
    }
}
