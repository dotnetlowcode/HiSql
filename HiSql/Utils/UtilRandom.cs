using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class UtilRandom
    {
        public static Random Random = new Random();

        /// <summary>
        /// 根据设置的权重权限分配
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static int GetRandomIndex(Dictionary<int, int> pars)
        {
            int maxValue = 0;
            foreach (var item in pars)
            {
                maxValue += item.Value;
            }
            var num = Random.Next(1, maxValue);
            var result = 0;
            var endValue = 0;
            foreach (var item in pars)
            {
                var index = pars.ToList().IndexOf(item);
                var beginValue = index == 0 ? 0 : pars[index - 1];
                endValue += item.Value;
                result = item.Key;
                if (num >= beginValue && num <= endValue)
                    break;
            }
            return result;
        }
    }
}
