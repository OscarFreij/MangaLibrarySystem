using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaLibrarySystem
{
    public static class StorageController
    {
        public static string StorageDirectory;

        public static async Task<byte[]> ReadFileBytes(FileType fileType, string fileId)
        {
            string subDirectory = string.Empty;
            switch(fileType)
            {
                case FileType.Image:
                    subDirectory = "/image";
                    break;

                case FileType.Data:
                    subDirectory = "/data";
                    break;
            }

            byte[] fileBytes = null;
            using (FileStream fs = File.Open(StorageDirectory + subDirectory + "/" + fileId, FileMode.Open))
            {
                fileBytes = new byte[fs.Length];
                await fs.ReadAsync(fileBytes, 0, (int)fs.Length);
            }

            return fileBytes;
        }

        public enum FileType
        {
            Image = 0,
            Data = 1
        }
    }
}
