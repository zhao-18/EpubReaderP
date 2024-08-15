using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderP.Models
{
    public class FilePickedEventArgs : EventArgs
    {
        public string File { get; set; }

        public FilePickedEventArgs (string file)
        {
            File = file;
        }
    }
}
