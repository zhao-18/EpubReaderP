using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderP.Models
{
    public static class Constants
    {
        public const string COVER_IMAGE_FOLDER = "./Covers";
        public const string BOOKS_DATA_FOLDER = "./Books";
        public const string GENERIC_COVER_IMAGE_SOURCE = "avares://EpubReaderP/Assets/DefaultBookCover.bmp";

        public const int COVER_MAX_WIDTH = 256;
        public const int COVER_MAX_HEIGHT = 256;
    }
}
