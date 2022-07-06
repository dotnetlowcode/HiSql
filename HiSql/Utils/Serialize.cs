using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Serialize<T> where T : class
    {
        /// <summary>
        /// 序列化 可以将任意实体类序列化至文件中
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void ToFile(T t, string path)
        {
             
            Stream stream = null;
            bool _error = false;
            var _msg = string.Empty;
            try
            {
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                var json=Newtonsoft.Json.JsonConvert.SerializeObject(t);
                byte[] bs = Encoding.Unicode.GetBytes(json);
                stream.Write(bs,0,bs.Length);
            }
            catch (Exception E)
            {
                _error = true;
                _msg = $"文件[{path}]保存失败,错误:{E.Message.ToString()}";
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            if(_error)
                throw new Exception(_msg);
            
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetFile(string path)
        {
            T myt=null;
            Stream stream = null;
            bool _error = false;
            var _msg = string.Empty;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                string json = Encoding.Unicode.GetString(bytes);
                myt=Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception E)
            {
                _error = true;
                _msg = $"文件[{path}]读取失败,错误:{E.Message.ToString()}";
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            if (_error)
                throw new Exception(_msg);
            return myt;
        }
    }
}
