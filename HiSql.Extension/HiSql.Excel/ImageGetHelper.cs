using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace HiSql.Extension
{
    public class ImageGetHelper : IDisposable
    {
        readonly Dictionary<string, string> _images = new Dictionary<string, string>();

        private readonly string tempPath =
            AppContext.BaseDirectory + "/" + Guid.NewGuid().ToString();

        public ImageGetHelper()
        {
            Directory.CreateDirectory(tempPath);
        }

        HttpClient client = new HttpClient();

        public void Dispose()
        {
            //递归清空临时文件夹里所有文件
            Directory.Delete(tempPath, true);
            client.Dispose();
        }

        public async Task<byte[]> GetImageData(string url)
        {
            if (_images.TryGetValue(url, out string value))
            {
                var localPath = value;
                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return Array.Empty<byte>();
                }
                return File.ReadAllBytes(localPath);
            }
            try
            {
                var imageBytes = await WebHelper.GetImageDataByUrl(url, client);
                var savePath = tempPath + "/" + Guid.NewGuid().ToString() + ".png";
                File.WriteAllBytes(savePath, imageBytes);
                //await File.WriteAllBytesAsync(savePath, imageBytes);
                _images.Add(url, savePath);
                return imageBytes;
            }
            catch (Exception)
            {
                _images.Add(url, string.Empty);
                return Array.Empty<byte>();
            }
        }
    }

    public class ExcelImageGetHelper : ImageGetHelper
    {
        Dictionary<string, int> workbookImageMap = new Dictionary<string, int>();

        public async Task<int> getImageId(IWorkbook _workBook, string url)
        {
            if (workbookImageMap.ContainsKey(url))
            {
                //复用图片Id，减小Excel大小
                var imageId = workbookImageMap[url];
                return imageId;
            }
            var imgData = await base.GetImageData(url);
            if (imgData.Length == 0)
            {
                return -1;
            }
            var imgId = _workBook.AddPicture(imgData, PictureType.JPEG);
            workbookImageMap[url] = imgId;
            return imgId;
        }
    }
}
