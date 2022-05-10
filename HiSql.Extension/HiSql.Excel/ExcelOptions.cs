using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Extension
{

    public enum TempType
    { 
        STANDARD=1,
        HEADER=2,
    }


    /// <summary>
    /// excel操作配置选项
    /// </summary>
    public class ExcelOptions
    {


        private TempType _temptype= TempType.STANDARD;

        /// <summary>
        /// 开始行
        /// </summary>
        private int _beginrow = 1;

        /// <summary>
        /// 
        /// </summary>
        private int _begincol = 1;



        private int _endrow = 0;
        

        /// <summary>
        /// 数据开始行
        /// </summary>
        private int _databeginrow = 4;


        private int _headerrow = 3;

        /// <summary>
        /// 一个sheet的最大记录数
        /// </summary>
        private int _sheetmaxrow = 1048576;
        /// <summary>
        /// 一个sheet的最在列数
        /// </summary>
        private int _sheetmaxcol = 16384;


        /// <summary>
        /// 单个sheet 设置最大的行数
        /// </summary>
        private int _sheetrow = 500000;

        private bool _isheader = true;

        private string _sheetname = "";

        /// <summary>
        /// 开始行
        /// </summary>
        public int BeginRow { get=> _beginrow; set=> _beginrow=value; }

        /// <summary>
        /// 开始列
        /// </summary>
        public int BeginCol { get=> _begincol; set=> _begincol=value; }


        /// <summary>
        /// 数据开始行
        /// </summary>
        public int DataBeginRow { get => _databeginrow; set => _databeginrow = value; }


        /// <summary>
        /// 标题行
        /// </summary>

        public int HeaderRow { get => _headerrow; set => _headerrow=value; }


        /// <summary>
        /// Excel模版类型
        /// </summary>
        public TempType TempType
        {
            get => _temptype;
            set=> _temptype = value;
        }


        /// <summary>
        /// 设置及获取sheet的最大记录数
        /// </summary>
        public int SheetRow { get => _sheetrow; set {
                if (value <= _sheetmaxrow)
                    _sheetrow = value;
                else
                    throw new Exception($"设置的Sheet行数超过了Excel最大支持行数{_sheetmaxrow}");
            } 
        }


        /// <summary>
        /// 是否抬头
        /// </summary>

        public bool IsHeader { get=> _isheader; set=> _isheader=value; }



        public int EndRow { get { return _endrow; } set {

                if (value >= 0) {
                    _endrow = 0;
                }else
                    _endrow = value; 
            } 
        }


        /// <summary>
        /// Excel的sheetname
        /// </summary>
        public string SheetName { 
        get { return _sheetname; }  set { _sheetname = value; }
        }
    }
}
