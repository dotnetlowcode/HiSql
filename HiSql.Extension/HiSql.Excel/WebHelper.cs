using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HiSql.Extension
{
    public class WebHelper
    {
        /// <summary>
        /// 下载远程图片为字节码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetImageDataByUrl(string url, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return Array.Empty<byte>();
        }
    }
}
