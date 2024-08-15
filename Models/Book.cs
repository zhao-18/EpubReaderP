using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;

namespace EpubReaderP.Models
{
    public class Book
    {
        /// <summary>
        /// Empty ctor for JSON serializer
        /// </summary>
        public Book() { 
            FilePath = string.Empty;
            Title = string.Empty;
            Author = string.Empty;
        }

        public Book(int id, string filePath, string title, string author, bool hasCover, int currentChapter0 = 0, int currentChapter1 = 0)
        {
            Id = id;
            FilePath = filePath;
            Title = title;
            Author = author;
            HasCover = hasCover;
            CurrentChapter0 = currentChapter0;
            CurrentChapter1 = currentChapter1;
        }

        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool HasCover { get; set; }
        public int CurrentChapter0 { get; set; }
        public int CurrentChapter1 { get; set; }
    }
}
