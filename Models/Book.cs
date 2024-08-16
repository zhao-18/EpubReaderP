using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersOne.Epub;
using Avalonia.Platform;

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

        public Book(int id, string filePath, string title, string author, Bitmap? cover, int currentChapter0 = 0, int currentChapter1 = 0)
        {
            Id = id;
            FilePath = filePath;
            Title = title;
            Author = author;
            Cover = cover;
            HasCover = cover != null;
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

        [JsonIgnore]
        public Bitmap? Cover { get; set; }

        public async Task<Bitmap> LoadCoverFromCache()
        {
            if (!HasCover)
            {
                return new Bitmap(AssetLoader.Open(new Uri(Constants.GENERIC_COVER_IMAGE_SOURCE)));
            }

            string CoverPath = Path.Combine(Constants.COVER_IMAGE_FOLDER, Title + Id + ".bmp");
            await using FileStream coverStream = File.OpenRead(CoverPath);
            return await Task.Run(() => Bitmap.DecodeToHeight(coverStream, Constants.COVER_MAX_HEIGHT));
        }
    }
}
