using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
//using Microsoft.Data;
using System.Data.Common;
namespace HiSql
{
    /// <summary>
    /// 基于标准的DbParameter 继承扩展了该类
    /// </summary>
    public partial class HiParameter : DbParameter
    {

        private object _value;
        /// <summary>
        /// 指定参数和值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public HiParameter(string name, object value)
        {
            _value = value;
            //var str=Newtonsoft.Json.JsonConvert.SerializeObject(value);
            //_value=Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            //this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;//DbParameter 中的属性

            if (value != null)
                ConvertDataType(value.GetType());

        }

        /// <summary>
        /// 参数参数和值 并指定值的类型 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public HiParameter(string name, object value, Type type)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            if (type != null)
                ConvertDataType(type);
        }
        public HiParameter(string name, object value, Type type, ParameterDirection direction)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            if (type != null)
                ConvertDataType(type);

            this.Direction = direction;
        }

        public HiParameter(string name, object value, Type type, ParameterDirection direction, int size)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            if (type != null)
                ConvertDataType(type);

            this.Direction = direction;

            this.Size = size;

        }

        public HiParameter(string name, object value, DbType type)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            this.DbType = type;
        }
        public HiParameter(string name, object value, DbType type, ParameterDirection direction)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            this.DbType = type;

            this.Direction = direction;
        }

        public HiParameter(string name, object value, DbType type, ParameterDirection direction, int size)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;
            this.DbType = type;

            this.Direction = direction;

            this.Size = size;
        }




        public HiParameter(string name, object value, ParameterDirection direction)
        {
            _value = value;
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _value = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(str);

            this.Value = _value;//DbParameter 中的属性
            this.ParameterName = name;//DbParameter 中的属性

            if (value != null)
                ConvertDataType(value.GetType());


            this.Direction = direction;
        }
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override int Size { get; set; }
        public override string SourceColumn { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override object Value { get; set; }

        public object Values { get => _value; set=>_value=value; }

    public override void ResetDbType()
        {
            this.DbType = System.Data.DbType.String;
        }

        public string UdtTypeName
        {
            get;
            set;
        }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbTypeName { get; set; }

  
        public bool IsJosn { get; set; }

        public bool IsArray { get; set; }


        /// <summary>
        /// 转换数据类型  
        /// </summary>
        /// <param name="type"></param>
        private void ConvertDataType(Type type)
        {
            if (type == Constants.ByteArrayType)
            {
                this.DbType = System.Data.DbType.Binary;
            }
            else if (type == Constants.GuidType)
            {
                this.DbType = System.Data.DbType.Guid;
            }
            else if (type == Constants.IntType)
            {
                this.DbType = System.Data.DbType.Int32;
            }
            else if (type == Constants.ShortType)
            {
                this.DbType = System.Data.DbType.Int16;
            }
            else if (type == Constants.LongType)
            {
                this.DbType = System.Data.DbType.Int64;
            }
            else if (type == Constants.DateType)
            {
                this.DbType = System.Data.DbType.DateTime;
            }
            else if (type == Constants.DobType)
            {
                this.DbType = System.Data.DbType.Double;
            }
            else if (type == Constants.DecType)
            {
                this.DbType = System.Data.DbType.Decimal;
            }
            else if (type == Constants.ByteType)
            {
                this.DbType = System.Data.DbType.Byte;
            }
            else if (type == Constants.FloatType)
            {
                this.DbType = System.Data.DbType.Single;
            }
            else if (type == Constants.BoolType)
            {
                this.DbType = System.Data.DbType.Boolean;
            }
            else if (type == Constants.StringType)
            {
                this.DbType = System.Data.DbType.String;
            }
            else if (type == Constants.DateTimeOffsetType)
            {
                this.DbType = System.Data.DbType.DateTimeOffset;
            }
            else if (type == Constants.TimeSpanType)
            {
                if (this.Value != null)
                    this.Value = this.Value.ToString();
            }
            else if (type != null && type.IsEnum())
            {
                this.DbType = System.Data.DbType.Int64;
            }

        }
    }
}
