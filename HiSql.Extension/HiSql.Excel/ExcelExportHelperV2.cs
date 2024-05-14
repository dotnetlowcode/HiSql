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
        List<DataTableHeaderInfo> headers;

        // /// <summary>
        // /// Excel单个sheet最大行数
        // /// </summary>
        //int maxSheetRow = 1000000;

        IWorkbook workbook;

        private ISheet sheet;

        // //定义是否启用宏
        // private bool EnableMacro = false;

        /// <summary>
        /// Excel初始化
        /// </summary>
        /// <param name="fileSavePath">保存路径</param>
        /// <param name="enableMacro">是否启用宏</param>
        public ExcelExportHelperV2(string fileSavePath, bool enableMacro = false)
        {
            byte[] xlsbyte;
            if (enableMacro)
            {
                // this.EnableMacro = true;
                fileSavePath = fileSavePath.Replace(".xlsx", ".xlsm");
                xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_StandardV2;
            }
            else
            {
                xlsbyte = HiSql.Excel.Properties.Resources.Excel_Template_Standard;
            }
            var dirPath = Path.GetDirectoryName(fileSavePath) ?? string.Empty;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(fileSavePath, xlsbyte);
            filePath = fileSavePath;
            FileStream file = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite
            );
            //将文件读到内存，在内存中操作excel
            this.workbook = new SXSSFWorkbook(new XSSFWorkbook(file));
        }

        Dictionary<string, DataTableHeaderInfo> headerMap =
            new Dictionary<string, DataTableHeaderInfo>();

        public async Task WriteDataTableToSheet(
            string tableTitle,
            List<DataTableHeaderInfo> headers,
            DataTable table,
            //定义导出进度action
            Action<int> progressAction = null,
            string sheetName = "Export"
        )
        {
            InitHeader(tableTitle, headers, sheetName);
            using (var imageGetHelper = new ExcelImageGetHelper())
            {
                var max = table.Rows.Count;
                var index = 0;
                //进度百分比0-100
                var progress = 0;
                await WriteDataTable(
                    table,
                    () =>
                    {
                        if (progressAction != null)
                        {
                            index++;
                            var tempValue = index * 100 / max;
                            if (tempValue > progress)
                            {
                                progressAction(tempValue);
                            }
                        }
                    },
                    async (sheet, row, cell, headerInfo) =>
                    {
                        var columnIndex = cell.ColumnIndex;
                        if (headerInfo.ValueType == ExcelValueType.Image)
                        {
                            row.Height = 2000;
                            var imgPath = cell.StringCellValue;
                            if (imgPath.StartsWith("//"))
                            {
                                imgPath = "https:" + imgPath;
                            }
                            imgPath = imgPath.Replace("w_80,h_80", "w_500,h_500"); //替换为大图
                            var imgId = await imageGetHelper.getImageId(sheet.Workbook, imgPath);
                            if (imgId == -1)
                            {
                                //图片不存在
                                return;
                            }
                            var patriarch = sheet.CreateDrawingPatriarch();
                            int rowIndex = cell.RowIndex;
                            var h = row.Height;
                            sheet.SetColumnWidth(columnIndex, Convert.ToInt32(row.Height * 2.4));
                            var anchor = patriarch.CreateAnchor(
                                1,
                                1,
                                1,
                                h - 2,
                                columnIndex,
                                rowIndex,
                                columnIndex + 1,
                                rowIndex + 1
                            );
                            patriarch.CreatePicture(anchor, imgId);
                        }
                    }
                );
                if (index < max)
                {
                    progressAction(100);
                }
            }
        }

        int sheetIndex = 0;

        public void InitHeader(
            string tableName,
            List<DataTableHeaderInfo> headers,
            string sheetName
        )
        {
            TableName = tableName;
            this.headers = headers;
            sheet = workbook.GetSheet(sheetName);
            if (sheet == null)
            {
                sheet = workbook.CreateSheet(sheetName);
            }
            workbook.SetSheetOrder(sheetName, sheetIndex++);
            //初始表头
            var excelRow = sheet.CreateRow(0);
            var tableNameTitleCell = excelRow.CreateCell(0);
            tableNameTitleCell.SetCellValue("表名");
            var tableNameValueCell = excelRow.CreateCell(1);
            tableNameValueCell.SetCellValue(this.TableName);
            var cnTitleRow = sheet.CreateRow(1);
            var enTitleRow = sheet.CreateRow(2);
            headerMap.Clear();
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

        private XSSFCellStyle CreateStyle()
        {
            //文本样式
            XSSFCellStyle textStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            textStyle.Alignment = HorizontalAlignment.Center;
            textStyle.VerticalAlignment = VerticalAlignment.Center;
            textStyle.BorderBottom = BorderStyle.Thin;
            textStyle.BorderLeft = BorderStyle.Thin;
            textStyle.BorderRight = BorderStyle.Thin;
            textStyle.BorderTop = BorderStyle.Thin;
            //自动换行
            //textStyle.WrapText = true;
            return textStyle;
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
            //文本样式
            XSSFCellStyle textStyle = CreateStyle();
            //日期样式
            XSSFCellStyle dateStyle = CreateStyle();
            XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                rowHandlerFun();
                var excelRow = sheet.CreateRow(sheet.LastRowNum + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell _dCell = excelRow.CreateCell(j);
                    _dCell.CellStyle = textStyle;
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
                            _dCell.CellStyle = dateStyle;
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
            //设置当前选中的sheet为第1个
            workbook.SetActiveSheet(0);
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
