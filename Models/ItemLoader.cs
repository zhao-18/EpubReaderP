using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EpubReaderP.Models
{
    public static class ItemLoader
    {

        public static async Task<T?> LoadItemAsync<T>(string path)
        {
            await using FileStream fs = File.OpenRead(path);
            T? Item = await JsonSerializer.DeserializeAsync<T>(fs);

            return Item;
        }

        public static async Task SaveItemAsync<T>(T Item,  string path)
        {
            using FileStream fs = File.OpenWrite(path);
            fs.SetLength(0);
            fs.Flush();
            await JsonSerializer.SerializeAsync(fs, Item);
        }
    }
}
