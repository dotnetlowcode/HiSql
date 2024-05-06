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
        public ImageGetHelper() { }

        HttpClient client = new HttpClient();

        string BasePath = AppContext.BaseDirectory + "excel_cache_image";

        public void Dispose()
        {
            //递归清空临时文件夹里所有文件
            client.Dispose();
            ClearTempFile();
        }

        /// <summary>
        /// 清空临时文件夹
        /// </summary>
        private void ClearTempFile()
        {
            //删除BasePath下昨天之前创建的所有目录以及目录下的文件
            var dirs = Directory.GetDirectories(BasePath);
            foreach (var dir in dirs)
            {
                var dirName = Path.GetFileName(dir);
                var dirDate = DateTime.Parse(dirName);
                if (dirDate.Date < DateTime.Now.Date.AddDays(-1))
                {
                    //删除目录
                    Directory.Delete(dir, true);
                }
            }
        }

        public async Task<byte[]> GetImageData(string url)
        {
            //生成日期目录名 2022-01-01 生成目录名 22-01-01
            var dateStr = DateTime.Now.ToString("yyyy-MM-dd");
            var cachePath =
                AppContext.BaseDirectory
                + url.Replace("https://", $"excel_cache_image/{dateStr}/")
                    .Replace("?", "_")
                    .Replace("&", "-");
            var dir = Path.GetDirectoryName(cachePath) ?? string.Empty;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(cachePath))
            {
                Console.WriteLine("读取图片缓存: " + cachePath);
                return File.ReadAllBytes(cachePath);
            }
            try
            {
                var imageBytes = await WebHelper.GetImageDataByUrl(url, client);
                File.WriteAllBytes(cachePath, imageBytes);
                return imageBytes;
            }
            catch (Exception)
            {
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
