using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{


    public enum SnowType
    {
        IdWorker=0,
        IdSnow=1

    }


    /// <summary>
    /// 雪花ID生成工具
    /// </summary>
    public class Snowflake 
    {
        static IdGenerate idGenerate = null;

        static long _tick = -1L;

        static int _workerid = 0;

        static object lckObj = new object();

        static object lckGenerateObj = new object();

        static SnowType snowType=SnowType.IdSnow;


        /// <summary>
        /// 指定雪花ID生成引擎
        /// </summary>

        public static SnowType SnowType
        {
            get { return snowType; }
            set { snowType = value; }
        }

        /// <summary>
        ///  设置时间戳
        /// </summary>
        public static long TickTick
        {
            get {
                if (_tick < 0L)
                    _tick=(long) (new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)- new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                return _tick; }
            set {
                idGenerate = getIdGenerate();
                _tick = value;
            }
        }

        /// <summary>
        /// 指定机器编码 0-31之间
        /// </summary>
        public static int WorkerId
        {
            get { return _workerid; }
            set {
                _workerid = value;
                idGenerate = getIdGenerate();
            }
        }


        static IdGenerate getIdGenerate()
        {
            if (snowType == SnowType.IdWorker)
            {
                return new IdWorker(_workerid, TickTick);
            }else
                return new IdSnow(_workerid, TickTick);
        }

        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <param name="workid"></param>
        /// <returns></returns>
        public static long NextId()
        {
            if (idGenerate == null)
            {
                lock (lckGenerateObj)
                {
                    if (idGenerate == null)
                    {
                        idGenerate = getIdGenerate();
                    }
                }
            }
            lock (lckObj)
            {
                return idGenerate.NextId();
            }
        }

        /// <summary>
        /// 获取指定数量的雪花ID
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<long> NextId(int count)
        {
            if (count <= 0)
                throw new Exception($"创建雪花ID的数量不能小于或等于0");
            if (idGenerate == null)
            {
                lock (lckGenerateObj)
                {
                    if (idGenerate == null)
                    {
                        idGenerate = getIdGenerate();
                    }
                }
            }
            List<long> list = new List<long>();
            lock (lckObj)
            { 
                for (int i = 0; i < count; i++)
                {
                    list.Add(idGenerate.NextId());
                }
            }
            return list;
        }
    }
}
