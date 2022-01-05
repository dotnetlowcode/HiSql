using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    internal class SynTaxQueue
    {
        List<string> _queue = new List<string>();


        public List<string> Queue {
            get { return _queue; }
            set { _queue = value; }
        }
        public SynTaxQueue()
        { 
        
        }
        public void Add(string name)
        {
            _queue.Add(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iseq">是否完全等于 默认false</param>
        /// <returns></returns>
        public bool HasQueue(string name, bool iseq = false)
        {
            if(!iseq)
                return _queue.Where(q => q.ToLower().IndexOf(name.ToLower()) == 0).Count() > 0;
            else
                return _queue.Where(q => q.ToLower() == name.ToLower()).Count() > 0;
        }
        

        public string LastQueue(int idx = 0)
        {
            if (idx > 0) idx = 0;
            if (_queue.Count < Math.Abs(idx)) return "";
            if (_queue.Count == 0) return "";
            else return _queue[_queue.Count - 1 + idx];
        }

        /// <summary>
        /// 统计某个操作出现的次数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isseq"></param>
        /// <returns></returns>
        public int Count(string name,bool isseq=false)
        {
            if (!isseq)
                return _queue.Where(q => q.ToLower().IndexOf(name.ToLower()) == 0).Count();
            else
                return _queue.Where(q => q.ToLower() == name.ToLower()).Count() ;

        }

    }
}
