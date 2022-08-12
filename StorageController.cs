using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static async void WriteFileBytes(FileType fileType, string fileId, byte[] data)
        {
            string subDirectory = string.Empty;
            switch (fileType)
            {
                case FileType.Image:
                    subDirectory = "/image";
                    break;

                case FileType.Data:
                    subDirectory = "/data";
                    break;
            }

            using (FileStream fs = File.Open(StorageDirectory + subDirectory + "/" + fileId, FileMode.Create))
            {
                await fs.WriteAsync(data, 0, (int)data.Length);
            }

            return;
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        public enum FileType
        {
            Image = 0,
            Data = 1
        }
    }
}
