using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 动态类型
    /// </summary>
    public class TDynamic
    {

        private ExpandoObject _exObject;
        private IDictionary<string, object> _dicO;
        public TDynamic()
        {
            _exObject = new ExpandoObject();
            _dicO = ((IDictionary<string, object>)_exObject);
        }
        public TDynamic(ExpandoObject exobj)
        {
            _exObject = exobj;
            _dicO = ((IDictionary<string, object>)_exObject);
        }
        public TDynamic(IDictionary<string, object> dictionary)
        {
            _exObject = new ExpandoObject();
            _dicO = ((IDictionary<string, object>)_exObject);
            foreach (string key in dictionary.Keys)
            {
                _dicO.Add(key, dictionary[key]);
            }
        }

        public TDynamic(object objdata)
        {
            Type type = objdata.GetType();
            _exObject = new ExpandoObject();
            _dicO = ((IDictionary<string, object>)_exObject);
            if (type.FullName.IndexOf("f__AnonymousType") > 0)
            {
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                foreach (PropertyInfo p in attrs)
                {
                    _dicO.Add(p.Name, p.GetValue(objdata));
                }
            }
            else
            {
                throw new Exception("不支持该类型");
            }
        }
       

        public dynamic ToDynamic()
        {
            return (dynamic)_exObject;
        }

        public object this[string keyname]
        {
            get
            {
                if (_dicO.ContainsKey(keyname))
                    return _dicO[keyname];
                else
                    return null;

            }
            set
            {
                SetProperty(keyname, value);


            }
        }

        private ExpandoObject ToExpandoObject()
        {
            return _exObject;
        }
        private IDictionary<string, object> ToDictionary()
        {
            

            return _dicO;
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, object value)
        {
            if (_dicO.ContainsKey(name))
                _dicO[name] = value;
            else
                _dicO.Add(name, value);
        }


        public T Field<T>(string name)
        {
            return (T) GetProperty(name);
        }

        /// <summary>
        /// 显示转换为字典对象
        /// </summary>
        /// <param name="texobj"></param>
        public static explicit operator Dictionary<string, object>(TDynamic texobj)
        {

            IDictionary<string, object> _idic = texobj.ToDictionary();
            Dictionary<string, object> _dic = new Dictionary<string, object>();
            foreach (string n in _idic.Keys)
            {
                _dic.Add(n, _idic[n]);
            }

            return _dic;
            //return  texobj.ToDictionary();
        }

        /// <summary>
        /// 显示转换为ExpandoObject
        /// </summary>
        /// <param name="textobj"></param>
        public static explicit operator ExpandoObject(TDynamic textobj)
        {
            return textobj.ToExpandoObject();
        }
        /// <summary>
        /// 将字典显示转换为TExpandoObject
        /// </summary>
        /// <param name="dic"></param>
        public static explicit operator TDynamic(Dictionary<string, object> dic)
        {
            return new TDynamic(dic);
        }
        //public static explicit operator List<TDynamic>(List<TDynamic> dyns)
        //{
        //    List<ExpandoObject> lst = new List<ExpandoObject>();
        //    foreach (TDynamic tdyn in dyns)
        //    {
        //        lst.Add((ExpandoObject)tdyn);
        //    }
        //    return null;
        //}


        /// <summary>
        /// 显示的将ExpandoObject 转为TExpandoObject
        /// </summary>
        /// <param name="exobj"></param>
        public static explicit operator TDynamic(ExpandoObject exobj)
        {
            return new TDynamic(exobj);
        }
        

        public IEnumerable<string> GetPropertys()
        {
            return _dicO.Keys;
        }

        public object GetProperty(string name)
        {
            return _dicO[name];
        }
    }
}
