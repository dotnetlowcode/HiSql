using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;

namespace HiSql.Extension
{
    public enum ExcelValueType
    {
        Text,
        DateTime,
        Number,
        Image
    }

    public class DataTableHeaderInfo : HeaderInfo
    {
        /// <summary>
        /// 列值类型
        /// </summary>
        public ExcelValueType ValueType { get; set; } = ExcelValueType.Text;
    }

    public class ExcelExportHelperV2
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// 表名
        /// </summary>
        private string TableName { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        readonly List<DataTableHeaderInfo> headers;

        /// <summary>
        /// Excel单个sheet最大行数
        /// </summary>
        int maxSheetRow = 100000;

        readonly IWorkbook workbook;

        private ISheet sheet;

        public ExcelExportHelperV2(
            string fileSavePath,
            string tableName,
            List<DataTableHeaderInfo> headers,
            string sheetName = "Export"
        )
        {
            TableName = tableName;
            this.headers = headers;
            filePath = fileSavePath;
            var dirPath = Path.GetDirectoryName(fileSavePath) ?? string.Empty;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            workbook = new SXSSFWorkbook(new XSSFWorkbook());
            sheet = workbook.CreateSheet(sheetName);
            InitHeader();
        }

        readonly Dictionary<string, DataTableHeaderInfo> headerMap =
            new Dictionary<string, DataTableHeaderInfo>();

        private void InitHeader()
        {
            //初始表头
            var excelRow = sheet.CreateRow(0);
            var tableNameTitleCell = excelRow.CreateCell(0);
            tableNameTitleCell.SetCellValue("表名");
            var tableNameValueCell = excelRow.CreateCell(1);
            tableNameValueCell.SetCellValue(this.TableName);
            var cnTitleRow = sheet.CreateRow(1);
            var enTitleRow = sheet.CreateRow(2);
            for (int i = 0; i < headers.Count; i++)
            {
                var headerObj = headers[i];
                headerMap.Add(headerObj.Title, headerObj);
                var cnCellObj = cnTitleRow.CreateCell(i);
                var enCellObj = enTitleRow.CreateCell(i);
                cnCellObj.SetCellValue(headerObj.Description);
                enCellObj.SetCellValue(headerObj.Title);
            }
        }

        public async Task WriteDataTable(
            DataTable dt,
            Action rowHandlerFun,
            Func<ISheet, IRow, ICell, DataTableHeaderInfo, Task> cellHandlerFun
        )
        {
            Type typeInt = typeof(int);
            Type typeInt64 = typeof(long);
            Type typeFloat = typeof(float);
            Type typeDec = typeof(decimal);
            Type typeDatetime = typeof(DateTime);
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                rowHandlerFun();
                var excelRow = sheet.CreateRow(sheet.LastRowNum + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell _dCell = excelRow.CreateCell(j);
                    XSSFCellStyle xSSFCellStyle1 = (XSSFCellStyle)workbook.CreateCellStyle();
                    //居中样式
                    xSSFCellStyle1.Alignment = HorizontalAlignment.Center;
                    xSSFCellStyle1.VerticalAlignment = VerticalAlignment.Center;
                    xSSFCellStyle1.BorderBottom = BorderStyle.Thin;
                    xSSFCellStyle1.BorderLeft = BorderStyle.Thin;
                    xSSFCellStyle1.BorderRight = BorderStyle.Thin;
                    xSSFCellStyle1.BorderTop = BorderStyle.Thin;
                    _dCell.CellStyle = xSSFCellStyle1;
                    var _value = dt.Rows[i][j].ToString().Trim();
                    var columnObj = dt.Columns[j];
                    if (
                        columnObj.DataType == typeDec
                        || columnObj.DataType == typeInt
                        || columnObj.DataType == typeInt64
                        || columnObj.DataType == typeFloat
                    )
                    {
                        if (_value.Length <= 10)
                        {
                            _dCell.SetCellType(CellType.Numeric);
                            if (_value == "")
                            {
                                _dCell.SetCellValue(_value);
                            }
                            else if (_value.IndexOf(".") > 0)
                                _dCell.SetCellValue(Convert.ToDouble(_value));
                            else
                                _dCell.SetCellValue(Convert.ToInt64(_value));
                        }
                        else
                            _dCell.SetCellValue(_value);
                    }
                    else if (columnObj.DataType == typeDatetime)
                    {
                        if (!string.IsNullOrEmpty(_value))
                        {
                            _dCell.SetCellValue(Convert.ToDateTime(_value));
                            XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();
                            xSSFCellStyle1.DataFormat = format.GetFormat("yyyy-MM-dd");
                            _dCell.CellStyle = xSSFCellStyle1;
                        }
                    }
                    else
                        _dCell.SetCellValue(_value);
                    var headInfo = headerMap[columnObj.ColumnName];
                    await cellHandlerFun(sheet, excelRow, _dCell, headInfo);
                }
            }
        }

        /// <summary>
        /// 保存excel
        /// 注意：不管这个excel多少数据，这个保存只要最后保存一次就可以了，中间不要做保存操作
        /// </summary>
        public void SaveSheetToFile()
        {
            FileStream file = null;
            try
            {
                Console.WriteLine("开始保存excel文件");
                file = new FileStream(filePath, FileMode.OpenOrCreate);
                workbook.Write(file, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
                Console.WriteLine("Excel保存结束");
            }
        }
    }
}
