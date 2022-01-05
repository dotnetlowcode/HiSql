using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// Hone 语法元素
    /// </summary>
    public struct HsonElement : IComparable<HsonElement>, IEquatable<HsonElement>
    {
        private readonly string _name;

        /// <summary>
        /// 需要自定义HsonValue
        /// </summary>
        private readonly object _value;
        public HsonElement(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            ValidateElementName(name);
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }
        public object Value
        {
            get { return _value; }
        }
        public static bool operator ==(HsonElement lhs, HsonElement rhs)
        {
            return Equals(lhs, rhs);
        }
        public static bool operator !=(HsonElement lhs, HsonElement rhs)
        {
            return !(lhs == rhs);
        }

        public HsonElement DeepClone()
        {
            object _o = _value;
            return new HsonElement(_name, _o);
        }
        private static void ValidateElementName(string name)
        {
            if (name.IndexOf('\0') >= 0)
            {
                throw new ArgumentException("Element name cannot contain null (0x00) characters");
            }
        }
        public HsonElement Clone()
        {
            return new HsonElement(_name, _value);
        }
        public bool Equals(HsonElement rhs)
        {
            return _name == rhs._name && _value == rhs._value;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(HsonElement)) { return false; }
            return Equals((HsonElement)obj);
        }

        public int CompareTo(HsonElement other)
        {
            int r = _name.CompareTo(other._name);
            if (r != 0) { return r; }
            return ((string)_value).CompareTo(other._value.ToString());
        }
        public override int GetHashCode()
        {
            // see Effective Java by Joshua Bloch
            int hash = 17;
            hash = 37 * hash + _name.GetHashCode();
            hash = 37 * hash + _value.GetHashCode();
            return hash;
        }
        public override string ToString()
        {
            return string.Format("{0}={1}", _name, _value);
        }
    }
}
