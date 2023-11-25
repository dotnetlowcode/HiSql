using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace HiSql.GUI.Helper
{
    public class HeaderInfo
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }



    public class ExcelExportHelper
    {

        /// <summary>
        /// 文件路径
        /// </summary>
        private string filePath;

        /// <summary>
        /// 表名
        /// </summary>
        private string TableName { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        List<HeaderInfo> headers = new List<HeaderInfo>();

      

        public ExcelExportHelper(string fileSavePath,string tableName, List<HeaderInfo>  headers)
        {
            this.TableName = tableName;
            this.headers = headers;
            this.filePath= fileSavePath;
            byte[] xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_Standard;
            var dirPath = Path.GetDirectoryName(fileSavePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            //复制模板excel到新路径
            FileStream  fs = new FileStream(fileSavePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            fs.Write(xlsbyte, 0, xlsbyte.Length);
            fs.Close();
        }



        private SXSSFSheet currentOperateSheet;

        public SXSSFSheet SetCurrentOperateSheet(string sheetName)
        {
            if( currentOperateSheet?.SheetName == sheetName)
            {
                return currentOperateSheet;
            }
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            XSSFWorkbook _workbook = new XSSFWorkbook(file);
            var workbook = new SXSSFWorkbook(_workbook, 1000);
            var sheetIndex = workbook.GetSheetIndex(sheetName);
            file.Close();
            if (sheetIndex == -1)
            {
                workbook.CreateSheet(sheetName);
                currentOperateSheet = workbook.GetSheet(sheetName) as SXSSFSheet;
                InitHeader();
            }
            else
            {
                currentOperateSheet = workbook.GetSheet(sheetName) as SXSSFSheet;
            }
            currentOperateSheet.ForceFormulaRecalculation = true;
            return currentOperateSheet;
        }


        private void InitHeader()
        {
            SXSSFSheet sheet = currentOperateSheet;
            //初始表头
            var excelRow = sheet.CreateRow(0);
            var tableNameTitleCell=   excelRow.CreateCell(0);
            tableNameTitleCell.SetCellValue("表名");
            var tableNameValueCell = excelRow.CreateCell(1);
            tableNameValueCell.SetCellValue(this.TableName);
            var cnTitleRow = sheet.CreateRow(1);
            var enTitleRow = sheet.CreateRow(2);
            for (int i = 0; i < headers.Count; i++)
            {
                var headerObj = headers[i];
               var cnCellObj=  cnTitleRow.CreateCell(i);
               var enCellObj = enTitleRow.CreateCell(i);
               cnCellObj.SetCellValue(headerObj.Description);
               enCellObj.SetCellValue(headerObj.Title);
            }
        }


        public async Task WriteDataTable(DataTable dt, Action rowHandlerFun, Func<SXSSFSheet, IRow, ICell, Task> cellHandlerFun)
        {

            Type typeint = typeof(int);
            Type typeint64 = typeof(long);
            Type typefloat = typeof(float);
            Type typedec = typeof(decimal);
            Type typedatetime = typeof(DateTime);
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                rowHandlerFun();
                SXSSFSheet sheet = this.currentOperateSheet;
                var workbook = sheet.Workbook;
                XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();
                XSSFCellStyle xSSFCellStyle1 = (XSSFCellStyle)workbook.CreateCellStyle();
                var excelRow = sheet.CreateRow(sheet.LastRowNum+1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell _dcell = excelRow.CreateCell(j);
                    var _value = dt.Rows[i][j].ToString().Trim();
                    if (dt.Columns[j].DataType == typedec || dt.Columns[j].DataType == typeint || dt.Columns[j].DataType == typeint64 || dt.Columns[j].DataType == typefloat)
                    {
                        if (_value.Length <= 10)
                        {
                            _dcell.SetCellType(CellType.Numeric);
                            if (_value == "")
                            {
                                _dcell.SetCellValue(_value);
                            }
                            else if (_value.IndexOf(".") > 0)
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
                    await cellHandlerFun(sheet,excelRow, _dcell);
                }
            }
        }

     

        public  void SaveSheetToFile()
        {
            if(currentOperateSheet == null)
            {
                return;
            }
            using (FileStream fs = File.OpenWrite(filePath))
            {
                var workbook = this.currentOperateSheet.Workbook as SXSSFWorkbook;
                workbook.Write(fs);
                workbook.Dispose();
                workbook.Close();
                fs.Close();
                currentOperateSheet = null;//重置sheet
            }
        }
    }
}
