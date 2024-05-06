using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using HiSql.Extension;

namespace HiSql.Excel
{
    public static class DataTableExcelHelper
    {
        /// <summary>
        /// DataTable转Excel
        /// </summary>
        /// <param name="tableTitle">表名</param>
        /// <param name="headers">列信息</param>
        /// <param name="table">表数据</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="sheetName">ExcelSheet名字(可选)</param>
        /// <returns></returns>
        public static async Task DataTableToExcel(
            string tableTitle,
            List<DataTableHeaderInfo> headers,
            DataTable table,
            string savePath,
            string sheetName = "Export"
        )
        {
            var excelObj = new ExcelExportHelperV2(savePath, tableTitle, headers, sheetName);
            using (var imageGetHelper = new ExcelImageGetHelper())
            {
                await excelObj.WriteDataTable(
                    table,
                    () => { },
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
                excelObj.SaveSheetToFile();
            }
        }
    }
}
