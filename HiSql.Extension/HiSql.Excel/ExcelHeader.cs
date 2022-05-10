using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Extension
{






    public class ExcelCell
    {


        private int _colidx = 1;
        private int _colspan = 1;//如果是1 表示没有跨列
        private int _rowspan = 1;//如果是1 表示没有跨行

        /// <summary>
        /// 开始列
        /// </summary>
        public int ColIdx { get=> _colidx; set=> _colidx=value; }
        /// <summary>
        /// 跨的列数
        /// </summary>
        public int ColSpan { get=> _colspan; set=> _colspan=value; }

        /// <summary>
        /// 跨的行数
        /// </summary>
        public int RowSpan { get=> _rowspan; set=> _rowspan=value; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    
    }

    public class ExcelHeader:IEnumerable<ExcelCell>
    {


        private int _rowidx = 1;


        /// <summary>
        /// 行
        /// </summary>
        public int RowIdx { get=> _rowidx; set=> _rowidx=value; }

        List<ExcelCell> _lstcell = new List<ExcelCell>();

        public List<ExcelCell> Cells { get => _lstcell; }

        public int CellCount {
            get => _lstcell.Count;
        }
        public ExcelHeader(int rowidx)
        {
            _rowidx = rowidx;
        }

        /// <summary>
        /// 添加单元格值
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public virtual ExcelHeader Add(string _value)
        {

            ExcelCell last = null;
            ExcelCell curr = new ExcelCell();
            if (_lstcell.Count > 0)
            {
                last = _lstcell[_lstcell.Count - 1];
                curr.ColIdx = last.ColIdx+(last.ColSpan-1) + 1;
            }
            else
            {
                curr.ColIdx = 1;
            }

            
            curr.Value = _value;
            _lstcell.Add(curr);
            return this;
        }


        public virtual ExcelHeader Add(ExcelCell cell)
        {

            ExcelCell last = null;
            if (_lstcell.Count > 0)
            {
                if (cell.ColIdx <= last.ColIdx+(last.ColSpan-1))
                    throw new Exception($"当前列索引{cell.ColIdx}不能比前面的小");
            }
            _lstcell.Add(cell);
            return this;
        }


        public IEnumerator<ExcelCell> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lstcell.GetEnumerator();
        }

    }
}
