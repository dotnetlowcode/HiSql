using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;

using NPOI.OOXML.XWPF;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace HiSql.Extension
{
    public class Excel: IEnumerable<ExcelHeader>
    {

        ExcelOptions _options=new ExcelOptions();


        List<ExcelHeader> _lstheader=new List<ExcelHeader> ();


        public Excel(ExcelOptions options)
        {
            _options = options;

            //HiSql.Excel.Properties.Resources.Excel_Template_Standard
        }

        public Excel()
        {
            _options = new ExcelOptions();
        }


        bool createExcelFile(int typ,string newpath)
        {
            bool flag = false;
            byte[] xlsbyte ;

            switch (typ)
            {
                case 1:
                    //标准excel
                    xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_Standard;
                    break;
                case 2:
                    //带表头的excel
                    xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_Header;
                    //xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_Standard;
                    break;
                default:
                    throw new Exception($"未识别的excel创建类型");
            }
            
            try
            {
                FileStream fs;
                if (System.IO.File.Exists(newpath))
                {
                    fs = new FileStream(newpath, FileMode.Open, FileAccess.Write, FileShare.None);
                }
                else
                    fs = new FileStream(newpath, FileMode.Create, FileAccess.Write, FileShare.None);


                fs.Write(xlsbyte, 0, xlsbyte.Length);
                fs.Close();
                flag = true;
            }catch(Exception e)
            {
                flag = false;

            }
            return flag;
        }


        public void WriteExcel(DataTable dt, string filepath,string sheetname="")
        {

            
            bool _iscreate= createExcelFile((int)_options.TempType,filepath);
            if (_iscreate)
            {
                PageWriteExcel(filepath, dt, sheetname);
            }
        }

        public DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    

                    // workbook=WorkbookFactory.Create(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(_options.HeaderRow-1);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = _options.BeginCol - 1; i < cellCount; ++i)  //firstRow.FirstCellNum
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = _options.BeginCol - 1; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = _options.DataBeginRow-1; i <= rowCount + _options.EndRow; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = _options.BeginCol-1; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        private void PageWriteExcel(string newfile, DataTable dtable,string sheetname="")
        {

            var count = dtable.Rows.Count;
            FileStream file = new FileStream(newfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


            IWorkbook workbook = new XSSFWorkbook(file);//将文件读到内存，在内存中操作excel

            //if (newfile.IndexOf(".xlsx") > 0)
            //    workbook2 = new XSSFWorkbook(file);
            //// 2003版本  
            //else if (newfile.IndexOf(".xls") > 0)
            //    workbook2 = new HSSFWorkbook(file);
            //SXSSFWorkbook xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            //int _actidx = xssfworkbook.ActiveSheetIndex;

            int _actidx = workbook.ActiveSheetIndex;
            //SXSSFSheet xssfsheet = null;

            ISheet xssfsheet = null;






            List<IName> lstsheets = new List<IName>();

            if (!string.IsNullOrEmpty(sheetname))
            {
                lstsheets=(List < IName > )workbook.GetAllNames();

                if (lstsheets.Any(n => n.SheetName.Equals(sheetname)))
                {
                    xssfsheet = workbook.GetSheet(sheetname) as SXSSFSheet; ;
                }
                else
                {
                    xssfsheet= workbook.CreateSheet(sheetname) as SXSSFSheet;
                    xssfsheet.SetActive(true);
                }
            }
            else
            {
                //xssfworkbook.CreateSheet(sheetname);
                xssfsheet = workbook.GetSheetAt(_actidx);//as SXSSFSheet;
            }
            
            file.Close();
            //var pages = Math.Ceiling(Convert.ToDouble(count) / PageSize);
            //将内存数据写到文件

            //for (int pageIndex = 1; pageIndex <= pages; pageIndex++)
            //{
            //    var startRow = (pageIndex - 1) * PageSize + 1;
            //    var endRow = pageIndex * PageSize;
            //    var dt = GetPagedTable(dtable, pageIndex);

            var beginRow = _options.BeginRow-1;//+ startRow - 1;//这里的beginRow这么处理，是因为我的Excel标题是在第三行


            if (_options.TempType == TempType.HEADER)
            {
                foreach (ExcelHeader header in _lstheader)
                {
                    beginRow =  header.RowIdx - 1+(_options.BeginRow - 1);
                    IRow excelRow = null;
                    
                    excelRow = xssfsheet.GetRow(beginRow);
                    if(excelRow==null)
                        excelRow = xssfsheet.CreateRow(beginRow);

                    ICellStyle rowStyle = excelRow.RowStyle;


                    foreach (ExcelCell cell in header.Cells)
                    {
                        ICell _dcell = null;

                        
                        _dcell = excelRow.GetCell(cell.ColIdx - 1);
                        if(_dcell==null)
                            _dcell = excelRow.CreateCell(cell.ColIdx - 1);

                        ICellStyle cellStyle=_dcell.CellStyle;

                        if(cellStyle.FillBackgroundColorColor == null)
                            _dcell.CellStyle = rowStyle;
                        else
                            _dcell.CellStyle = cellStyle;

                        _dcell.SetCellValue(cell.Value);
                        


                    }
                    //excelRow.RowStyle = rowStyle;
                    
                }
            }
            else {
                bool _custheader = false;
                if (_lstheader.Count > 0)
                {
                    if (_lstheader[_lstheader.Count - 1].CellCount == dtable.Columns.Count)
                    {
                        beginRow = _lstheader[_lstheader.Count - 1].RowIdx - 1 + (_options.BeginRow - 1);
                        IRow excelRow = xssfsheet.CreateRow(beginRow);
                        foreach (ExcelCell cell in _lstheader[_lstheader.Count - 1].Cells)
                        {
                            ICell _dcell = excelRow.CreateCell(cell.ColIdx - 1);
                            _dcell.SetCellValue(cell.Value);
                        }
                        _custheader = true;
                    }
                }

                if (!_custheader)
                {
                    IRow excelRow = xssfsheet.CreateRow(beginRow);
                    int _colidx = 0;
                    foreach (DataColumn dc in dtable.Columns)
                    {
                        ICell _dcell = excelRow.CreateCell(_colidx);
                        _dcell.SetCellValue(dc.ColumnName);
                        _colidx++;
                    }
                }

            }

            //保存抬头数据
            using (FileStream fs = File.OpenWrite(newfile))
            {
                xssfsheet.ForceFormulaRecalculation = false;
                workbook.Write(fs);
                //workbook2.Dispose();

                workbook.Close();
                if (fs != null) fs.Close();
            }





            int pageSize = 1000;
            int pageCount = dtable.Rows.Count > pageSize ? dtable.Rows.Count % pageSize == 0 ? dtable.Rows.Count / pageSize : dtable.Rows.Count / pageSize + 1 : 1;


            ////写数据
            //for (var i = 0; i < dtable.Rows.Count; i++)
            //{
            //    beginRow = beginRow + 1;
            //    var excelRow = xssfsheet.CreateRow(beginRow);

            //    for (int j = 0; j < dtable.Columns.Count; j++)
            //    {
            //        ICell _dcell = excelRow.CreateCell(j);

            //        _dcell.SetCellValue(dtable.Rows[i][j].ToString().Trim());
            //    }
            //}

            pageWriteExcel(dtable, _actidx, beginRow, newfile);


            //fs.Flush();



            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //Thread.Sleep(500);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetidx"></param>
        /// <param name="currrowidx"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageidx"></param>
        /// <param name="excelpath"></param>
        /// <returns></returns>
        //private bool pageWriteExcel(DataTable dt,int sheetidx,int currrowidx, int pagesize, int pageidx,string excelpath)
        private bool pageWriteExcel(DataTable dt,int sheetidx,int currrowidx, string excelpath)
        {
            FileStream file = new FileStream(excelpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


            XSSFWorkbook workbook = new XSSFWorkbook(file);//将文件读到内存，在内存中操作excel

            SXSSFWorkbook xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            SXSSFSheet xssfsheet = xssfworkbook.GetSheetAt(sheetidx) as SXSSFSheet;
            file.Close();

            int beginRow = currrowidx;

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                beginRow = beginRow + 1;
                var excelRow = xssfsheet.CreateRow(beginRow);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell _dcell = excelRow.CreateCell(j);

                    _dcell.SetCellValue(dt.Rows[i][j].ToString().Trim());
                }
            }

            using (FileStream fs = File.OpenWrite(excelpath))
            {
                xssfsheet.ForceFormulaRecalculation = true;
                xssfworkbook.Write(fs);
                xssfworkbook.Dispose();
                xssfworkbook.Close();
                if (fs != null) fs.Close();
            }

            return true;
        }


        public virtual Excel Add(ExcelHeader header)
        {

            if(header == null )
                throw new Exception($"标题信息为空");
            if(header.CellCount == 0)
                throw new Exception($"标题信息为空");

            ExcelHeader last = null;
            if (_lstheader.Count > 0)
            {
                last = _lstheader[_lstheader.Count - 1];


                if (header.RowIdx <= last.RowIdx)
                    throw new Exception($"当前行索引{header.RowIdx}不能比前面的小");

                header.RowIdx=last.RowIdx+1;
            }
            else
            {
                header.RowIdx = 1;
            }
            _lstheader.Add(header);

            return this;
        }

        public IEnumerator<ExcelHeader> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lstheader.GetEnumerator();
        }

    }
}
