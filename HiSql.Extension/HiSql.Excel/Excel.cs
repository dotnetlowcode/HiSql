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
using System.Threading.Tasks;

namespace HiSql.Extension
{
    public class Excel : IEnumerable<ExcelHeader>
    {

        ExcelOptions _options = new ExcelOptions();


        List<ExcelHeader> _lstheader = new List<ExcelHeader>();


        public Excel(ExcelOptions options)
        {
            _options = options;

            //HiSql.Excel.Properties.Resources.Excel_Template_Standard
        }

        public Excel()
        {
            _options = new ExcelOptions();
        }


        bool createExcelFile(int typ, string newpath)
        {
            bool flag = false;
            byte[] xlsbyte;

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
                var dirPath = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                if (System.IO.File.Exists(newpath))
                {
                    fs = new FileStream(newpath, FileMode.Open, FileAccess.Write, FileShare.None);
                }
                else
                    fs = new FileStream(newpath, FileMode.Create, FileAccess.Write, FileShare.None);


                fs.Write(xlsbyte, 0, xlsbyte.Length);
                fs.Close();
                flag = true;
            }
            catch (Exception e)
            {
                flag = false;

            }
            return flag;
        }


        public void WriteExcel(DataTable dt, string filepath, string sheetname = "")
        {


            bool _iscreate = createExcelFile((int)_options.TempType, filepath);
            if (_iscreate)
            {
                PageWriteExcel(filepath, dt, sheetname);
            }
        }

        public T ReaderExcel<T>(string filePath, Func<IWorkbook, T> operateFun)
        {
            FileStream fs = null;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    IWorkbook workbook = null;
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);
                    return operateFun(workbook);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return default;
        }



        /// <summary>
        /// 读取Excel数据
        /// </summary>
        /// <param name="filePath">Excel路径</param>
        /// <param name="isColumnName">是否构建表格列</param>
        /// <param name="sheetName">读取的SheetName</param>
        /// <param name="top">读取前多少行</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(string filePath, bool isColumnName, string sheetName = null, int? top = null)
        {
            return ReaderExcel(filePath, (workbook) =>
             {
                 DataTable dataTable = null;
                 DataColumn column = null;
                 DataRow dataRow = null;
                 ISheet sheet = null;
                 IRow row = null;
                 ICell cell = null;
                 int startRow = 0;
                 if (workbook == null)
                 {
                     return null;
                 }
                 if (!string.IsNullOrEmpty(sheetName))
                 {
                     sheet = workbook.GetSheet(sheetName);
                 }
                 if (sheet == null)
                 {
                     sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                 }
                 dataTable = new DataTable();
                 if (sheet == null)
                 {
                     return null;
                 }
                 int rowCount = sheet.LastRowNum;//总行数 
                 if (top != null && top < rowCount)
                 {
                     rowCount = top.Value;
                 }
                 if (rowCount >= 0)
                 {
                     IRow firstRow = sheet.GetRow(_options.HeaderRow - 1);//第一行  
                     if (firstRow == null)
                     {
                         return dataTable;//空Excel
                     }
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
                     for (int i = _options.DataBeginRow - 1; i <= rowCount + _options.EndRow; ++i)
                     {
                         row = sheet.GetRow(i);
                         if (row == null) continue;

                         dataRow = dataTable.NewRow();
                         for (int j = _options.BeginCol - 1; j < cellCount; ++j)
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
                 return dataTable;
             });
        }



        /// <summary>
        /// 获取ExcelSheetNames
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<string> GetExcelSheetNames(string filePath)
        {
            return ReaderExcel(filePath, (workBook) =>
             {
                 var sheetCount = workBook.NumberOfSheets;
                 var sheetNames = new List<string>();
                 for (int i = 0; i < sheetCount; i++)
                 {
                     sheetNames.Add(workBook.GetSheetAt(i).SheetName);
                 }
                 return sheetNames;
             });
        }

        private void PageWriteExcel(string newfile, DataTable dtable, string sheetname = "")
        {

            var count = dtable.Rows.Count;
            FileStream file = new FileStream(newfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);



            XSSFWorkbook workbook = new XSSFWorkbook(file);//将文件读到内存，在内存中操作excel

            SXSSFWorkbook xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            int _actidx = workbook.ActiveSheetIndex;
            SXSSFSheet xssfsheet = xssfworkbook.GetSheetAt(_actidx) as SXSSFSheet;
            file.Close();








            List<IName> lstsheets = new List<IName>();


            var beginRow = _options.BeginRow - 1;//+ startRow - 1;//这里的beginRow这么处理，是因为我的Excel标题是在第三行


            if (_options.TempType == TempType.HEADER)
            {
                foreach (ExcelHeader header in _lstheader)
                {
                    beginRow = header.RowIdx - 1 + (_options.BeginRow - 1);
                    IRow excelRow = null;

                    excelRow = xssfsheet.GetRow(beginRow);
                    if (excelRow == null)
                        excelRow = xssfsheet.CreateRow(beginRow);

                    ICellStyle rowStyle = excelRow.RowStyle;


                    foreach (ExcelCell cell in header.Cells)
                    {
                        ICell _dcell = null;


                        _dcell = excelRow.GetCell(cell.ColIdx - 1);
                        if (_dcell == null)
                            _dcell = excelRow.CreateCell(cell.ColIdx - 1);

                        ICellStyle cellStyle = _dcell.CellStyle;

                        if (cellStyle.FillBackgroundColorColor == null)
                            _dcell.CellStyle = rowStyle;
                        else
                            _dcell.CellStyle = cellStyle;

                        _dcell.SetCellValue(cell.Value);



                    }
                    //excelRow.RowStyle = rowStyle;

                }
            }
            else
            {
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
                xssfsheet.ForceFormulaRecalculation = true;
                xssfworkbook.Write(fs);
                xssfworkbook.Dispose();
                xssfworkbook.Close();
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
        /// DataTable数据写入Excel
        /// </summary>
        /// <param name="getData"></param>
        /// <param name="sheetName"></param>
        /// <param name="excelpath"></param>
        /// <param name="headerMap"></param>
        /// <param name="headerRowNumber"></param>
        /// <returns></returns>
        public async Task<bool> DataTableToExcel(Func<Task<Tuple<DataTable, int>>> getData, string sheetName, string excelpath, Dictionary<string, string> headerMap, int headerRowNumber)
        {
            var fs = new FileStream(excelpath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var workbook = new XSSFWorkbook(fs);//将文件读到内存，在内存中操作excel
            //var xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            ISheet xssfsheet = workbook.GetSheet(sheetName);
            fs.Close();
            try
            {
                await WriteSheetData(workbook, xssfsheet, getData, headerMap, headerRowNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            using (FileStream stream = new FileStream(excelpath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(stream);
            }

            //using (FileStream fWrite = File.OpenWrite(excelpath))
            //{
            //    xssfsheet.ForceFormulaRecalculation = true;
            //    xssfworkbook.Write(fWrite);
            //    xssfworkbook.Dispose();
            //    xssfworkbook.Close();
            //    if (fs != null) fs.Close();
            //}
            return true;
        }


        public async Task WriteSheetData(XSSFWorkbook workbook, ISheet xssfsheet, Func<Task<Tuple<DataTable, int>>> getData, Dictionary<string, string> headerMap, int headerRowNumber)
        {
            var headerRow = workbook.GetSheet(xssfsheet.SheetName).GetRow(headerRowNumber); //xssfsheet.GetRow(headerRowNumber);
            if (headerRow == null)
            {
                throw new Exception("缺少表头，Excel有问题！");
            }
            Type typeint = typeof(int);
            Type typeint64 = typeof(Int64);
            Type typefloat = typeof(float);
            Type typedec = typeof(decimal);
            Type typedatetime = typeof(DateTime);
            XSSFCellStyle xSSFCellStyle1 = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();
            while (true)
            {
                var dataResult = await getData();
                if (dataResult == null)
                {
                    break;
                }
                int beginRow = dataResult.Item2;
                var dt = dataResult.Item1;
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    beginRow += 1;
                    var excelRow = xssfsheet.CreateRow(beginRow);
                    var dtRow = dt.Rows[i];
                    for (int j = 0; j < headerMap.Keys.Count; j++)
                    {
                        var headKey = headerRow.GetCell(j).StringCellValue;
                        var dtKey = headerMap[headKey];
                        var dtCell = dtRow[dtKey];
                        var dtColumn = dt.Columns[dtKey];
                        ICell _dcell = excelRow.CreateCell(j);
                        var _value = dtCell.ToString().Trim();
                        if (dtColumn.DataType == typedec || dtColumn.DataType == typeint || dtColumn.DataType == typeint64 || dtColumn.DataType == typefloat)
                        {
                            if (_value.Length <= 10)
                            {
                                _dcell.SetCellType(CellType.Numeric);
                                if (_value.IndexOf(".") > 0)
                                    _dcell.SetCellValue(Convert.ToDouble(_value));
                                else
                                    _dcell.SetCellValue(Convert.ToInt64(_value));
                            }
                            else
                                _dcell.SetCellValue(_value);
                        }
                        else if (dtColumn.DataType == typedatetime)
                        {

                            _dcell.SetCellValue(Convert.ToDateTime(_value));

                            xSSFCellStyle1.DataFormat = format.GetFormat("yyyy-MM-dd");
                            _dcell.CellStyle = xSSFCellStyle1;

                        }
                        else
                            _dcell.SetCellValue(_value);
                    }
                }
            }
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
        private bool pageWriteExcel(DataTable dt, int sheetidx, int currrowidx, string excelpath)
        {
            FileStream file = new FileStream(excelpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


            XSSFWorkbook workbook = new XSSFWorkbook(file);//将文件读到内存，在内存中操作excel

            SXSSFWorkbook xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            SXSSFSheet xssfsheet = xssfworkbook.GetSheetAt(sheetidx) as SXSSFSheet;
            file.Close();

            int beginRow = currrowidx;

            Type typeint = typeof(int);
            Type typeint64 = typeof(Int64);
            Type typefloat = typeof(float);
            Type typedec = typeof(decimal);
            Type typedatetime = typeof(DateTime);
            XSSFCellStyle xSSFCellStyle1 = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();

            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    beginRow = beginRow + 1;
                    var excelRow = xssfsheet.CreateRow(beginRow);

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell _dcell = excelRow.CreateCell(j);
                        var _value = dt.Rows[i][j].ToString().Trim();
                        if (dt.Columns[j].DataType == typedec || dt.Columns[j].DataType == typeint || dt.Columns[j].DataType == typeint64 || dt.Columns[j].DataType == typefloat)
                        {
                            if (_value.Length <= 10)
                            {
                                _dcell.SetCellType(CellType.Numeric);
                                if (_value.IndexOf(".") > 0)
                                    _dcell.SetCellValue(Convert.ToDouble(_value));
                                else
                                    _dcell.SetCellValue(Convert.ToInt64(_value));
                            }
                            else
                                _dcell.SetCellValue(_value);


                        }
                        else if (dt.Columns[j].DataType == typedatetime)
                        {
                            if (!string.IsNullOrEmpty(_value))
                                _dcell.SetCellValue(Convert.ToDateTime(_value));

                            xSSFCellStyle1.DataFormat = format.GetFormat("yyyy-MM-dd");
                            _dcell.CellStyle = xSSFCellStyle1;

                        }
                        else
                            _dcell.SetCellValue(_value);





                    }
                }

            }
            else
            {
                beginRow = beginRow + 1;
                var excelRow = xssfsheet.CreateRow(beginRow);
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

            if (header == null)
                throw new Exception($"标题信息为空");
            if (header.CellCount == 0)
                throw new Exception($"标题信息为空");

            ExcelHeader last = null;
            if (_lstheader.Count > 0)
            {
                last = _lstheader[_lstheader.Count - 1];


                if (header.RowIdx <= last.RowIdx)
                    throw new Exception($"当前行索引{header.RowIdx}不能比前面的小");

                header.RowIdx = last.RowIdx + 1;
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
