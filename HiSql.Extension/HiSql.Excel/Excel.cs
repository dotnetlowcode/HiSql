using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Text;

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


        public void WriteExcel(DataTable dt, string filepath, string sheetname = "", Action<ISheet, IRow, ICell> cellRenderFun = null)
        {
            bool _iscreate = createExcelFile((int)_options.TempType, filepath);
            if (_iscreate)
            {
                PageWriteExcel(filepath, dt, sheetname, cellRenderFun);
            }
        }

        public async Task<T> ReaderExcel<T>(string filePath, Func<IWorkbook, Task<T>> operateFun)
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
                    var result=await operateFun(workbook);
                    
                    workbook.Close();
                    workbook.Dispose();
                    return result;
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
        public  Task<DataTable> ExcelToDataTable(string filePath, bool isColumnName, string sheetName = null, int? top = null)
        {
             return ReaderExcel(filePath,async (workbook) =>
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
                                switch (cell.CellType)
                                {
                                    case NPOI.SS.UserModel.CellType.String:
                                        dataRow[j] = cell.StringCellValue; break;
                                     case NPOI.SS.UserModel.CellType.Numeric:
                                        if (DateUtil.IsCellDateFormatted(cell))
                                            dataRow[j] = cell.DateCellValue;
                                        else
                                            dataRow[j] = cell.NumericCellValue.ToString();
                                         break;
                                     case NPOI.SS.UserModel.CellType.Boolean:
                                        dataRow[j] = cell.BooleanCellValue.ToString();
                                         break;
                                    case NPOI.SS.UserModel.CellType.Formula:
                                         try
                                         {
                                             dataRow[j] =   GetEvalValue(cell);
                                         }
                                         catch (Exception ex)
                                         {
                                             // 处理公式计算异常
                                             dataRow[j] = "#N/A";
                                         }
                                         break;
                                     case NPOI.SS.UserModel.CellType.Blank:
                                         dataRow[j] = "";
                                         break;
                                    case NPOI.SS.UserModel.CellType.Error:
                                        dataRow[j] = cell.ErrorCellValue.ToString();
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


        public Task ExcelReaderAction(string filePath, bool isColumnName, Func<DataTable, int, int, Task<bool>> dataTableAction, int buffCount = 1000, string sheetName = null, int? top = null)
        {
            return ReaderExcel(filePath, async (workbook) =>
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
                    return Task.CompletedTask;
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
                    return Task.CompletedTask;
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
                        return Task.CompletedTask;
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
                                switch (cell.CellType)
                                {
                                    case CellType.String:
                                        dataRow[j] = cell.StringCellValue; break;
                                    case CellType.Numeric:
                                        if (DateUtil.IsCellDateFormatted(cell))
                                            dataRow[j] = cell.DateCellValue;
                                        else
                                            dataRow[j] = cell.NumericCellValue.ToString();
                                        break;
                                    case CellType.Boolean:
                                        dataRow[j] = cell.BooleanCellValue.ToString();
                                        break;
                                    case CellType.Formula:
                                        try
                                        {
                                            dataRow[j] = GetEvalValue(cell);
                                        }
                                        catch (Exception ex)
                                        {
                                            // 处理公式计算异常
                                            dataRow[j] = "#N/A";
                                        }
                                        break;
                                    case CellType.Blank:
                                        dataRow[j] = "";
                                        break;
                                    case CellType.Error:
                                        dataRow[j] = cell.ErrorCellValue.ToString();
                                        break;
                                }
                            }
                        }
                        dataTable.Rows.Add(dataRow);
                        if (dataTable.Rows.Count >= buffCount)
                        {
                            var isContinum = await dataTableAction(dataTable, i, rowCount);
                            if (!isContinum)
                            {
                                break;
                            }
                            dataTable.Rows.Clear();
                        }
                    }
                    await dataTableAction(dataTable, rowCount, rowCount);
                }
               
                return Task.CompletedTask;
            });
        }


        public async Task ReaderBigExcel(string filePath, Func<DataTable, int, int, Task<bool>> dataTableAction,Func<string,string,object> cellValue ,int buffCount = 1000)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IExcelDataReader reader = null;
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                var config = new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1252)
                };
                if (Path.GetExtension(filePath).Equals(".xls"))
                    reader = ExcelReaderFactory.CreateBinaryReader(stream, config);
                else if (Path.GetExtension(filePath).Equals(".xlsx"))
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream, config);

                var dataTable = new DataTable();
                for (int i = 1; i < this._options.HeaderRow; i++)
                {
                    reader.Read();//跳到表头行
                }
               
                int rowCount = 0;
                while (reader.Read())
                {
                    if (rowCount == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                           var cellObj=  reader.GetValue(i);
                            if (cellObj == null)
                            {
                                break;
                            }
                            dataTable.Columns.Add(cellObj.ToString());
                        }
                    }
                    else
                    {
                        DataRow row = dataTable.NewRow();
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            var cellObj = reader.GetValue(i);
                            if (cellObj == null)
                            {
                                row[i] = "";
                            }
                            else if(cellValue!=null)
                            {
                                row[i] = cellValue(dataTable.Columns[i].ColumnName, cellObj.ToString());
                            }
                            else
                            {
                                row[i] =  cellObj.ToString();
                            }
                        }
                        dataTable.Rows.Add(row);
                        if (dataTable.Rows.Count >= buffCount)
                        {
                            var isContinum = await dataTableAction(dataTable, rowCount, reader.RowCount);
                            if (!isContinum)
                            {
                                break;
                            }
                            dataTable.Rows.Clear();
                        }
                    }
                    rowCount++;
                }
                if (dataTable.Rows.Count > 0)
                {
                    await dataTableAction(dataTable, reader.RowCount, reader.RowCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
        
        }




        /// <summary>
        /// 获取公式计算结果
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string GetEvalValue(ICell cell)
        {
            var formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(cell.Sheet.Workbook);
            NPOI.SS.UserModel.CellValue cellValueObj = formulaEvaluator.Evaluate(cell);
            switch (cellValueObj.CellType)
            {
                case NPOI.SS.UserModel.CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cellValueObj.FormatAsString();
                    }
                    else
                    {
                        return cellValueObj.NumberValue.ToString();
                    }
                case NPOI.SS.UserModel.CellType.Boolean:
                    return  cellValueObj.BooleanValue.ToString();
                case NPOI.SS.UserModel.CellType.String:
                    return cellValueObj.StringValue;
                default:
                    return  "";
            }
        }



        /// <summary>
        /// 获取ExcelSheetNames
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Task<List<string>> GetExcelSheetNames(string filePath)
        {
            return ReaderExcel(filePath,async (workBook) =>
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

        private void PageWriteExcel(string newfile, DataTable dtable, string sheetname = "", Action<ISheet, IRow, ICell> cellRenderFun = null)
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

            pageWriteExcel(dtable, _actidx, beginRow, newfile, cellRenderFun);


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
        public async Task<bool> DataTableToExcel(Func<Task<Tuple<DataTable, int>>> getData, string sheetName, string excelpath, Dictionary<string, string> headerMap, int headerRowNumber, Action<IRow, ICell, string> cellRenderFun)
        {
            var fs = new FileStream(excelpath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var workbook = new XSSFWorkbook(fs);//将文件读到内存，在内存中操作excel
            //var xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            ISheet xssfsheet = workbook.GetSheet(sheetName);
            fs.Close();
            try
            {
                await WriteSheetData(workbook, xssfsheet, getData, headerMap, headerRowNumber, cellRenderFun);
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


        public async Task WriteSheetData(XSSFWorkbook workbook, ISheet xssfsheet, Func<Task<Tuple<DataTable, int>>> getData, Dictionary<string, string> headerMap, int headerRowNumber, Action<IRow, ICell, string> cellRenderFun)
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
                    if (beginRow == 0)
                    {
                        beginRow = beginRow + 1 + headerRowNumber;//从表头的下一行开始写
                    }
                    else
                    {
                        beginRow = beginRow + 1;
                    }
                    var excelRow = xssfsheet.CreateRow(beginRow);
                    var dtRow = dt.Rows[i];
                    for (int j = 0; j < headerMap.Keys.Count; j++)
                    {
                        var headKey = headerRow.GetCell(j)?.StringCellValue;
                        if (headKey == null)
                        {
                            continue;
                        }
                        var dtKey = headerMap[headKey];
                        var dtCell = dtRow[dtKey];
                        var dtColumn = dt.Columns[dtKey];
                        ICell _dcell = excelRow.CreateCell(j);
                        var _value = dtCell.ToString().Trim();
                        cellRenderFun(excelRow, _dcell, dtKey);
                        if (dtColumn.DataType == typedec || dtColumn.DataType == typeint || dtColumn.DataType == typeint64 || dtColumn.DataType == typefloat)
                        {
                            if (_value.Length <= 10)
                            {
                                _dcell.SetCellType(NPOI.SS.UserModel.CellType.Numeric);
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
        private bool pageWriteExcel(DataTable dt, int sheetidx, int currrowidx, string excelpath, Action<ISheet, IRow, ICell> cellRenderFun)
        {
            FileStream file = new FileStream(excelpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


            XSSFWorkbook workbook = new XSSFWorkbook(file);//将文件读到内存，在内存中操作excel

            SXSSFWorkbook xssfworkbook = new SXSSFWorkbook(workbook, 1000);
            var w = xssfworkbook.GetSheetAt(sheetidx);
            SXSSFSheet xssfsheet = xssfworkbook.GetSheetAt(sheetidx) as SXSSFSheet;
            // 
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
                                _dcell.SetCellType(NPOI.SS.UserModel.CellType.Numeric);
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
                        cellRenderFun(xssfsheet, excelRow, _dcell);




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
