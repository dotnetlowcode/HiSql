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
             
            Stream stream = null; ;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

                formatter.Serialize(stream, t);
                
            }
            catch (Exception E)
            {
                throw new Exception($"文件[{path}]保存失败,错误:{E.Message.ToString()}");
            }
            finally
            {
                stream.Close();
            }
            
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
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                myt = (T)formatter.Deserialize(stream);
                
            }
            catch (Exception E)
            {
                throw new Exception($"文件[{path}]读取失败,错误:{E.Message.ToString()}");
            }
            finally
            {
                stream.Close();
            }
            return myt;
        }
    }
}
