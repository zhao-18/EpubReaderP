using EpubReaderP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Book Book { get; set; }
        public EpubBook EpubBook { get; set; }
    }
}
