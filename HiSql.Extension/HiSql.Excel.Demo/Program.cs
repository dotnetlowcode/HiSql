using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HiSql.Extension;

namespace HiSql.Excel.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await HiSqlExcelV2ImageExport();
            //ReadExcelName();
            //BuildExceBigData();
            //BuildExcel_1();
            //ReadExcel_1();

            //BuildExcel_2();
            //ReadExcel_2();

            //BuildExcel_3();
            //ReadExcel_3();

            //解析并保存 支付宝导出的excel帐单
            //InsertPay();


            //Console.WriteLine("生成完成");

            var s = Console.ReadLine();
        }

        static void ReadExcelName()
        {
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER }
            );
            List<string> names = excel
                .GetExcelSheetNames(@"D:\data\GD_UniqueCodeInfo1.xlsx")
                .Result;
            foreach (string name in names)
            {
                Console.WriteLine($"excel sheetName:{name}");
            }
        }

        static void ReadExcel_1()
        {
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER }
            );

            DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo1.xlsx", true).Result;
        }

        static void ReadExcel_2()
        {
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions()
                {
                    TempType = Extension.TempType.STANDARD,
                    DataBeginRow = 2,
                    HeaderRow = 1
                }
            );

            DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo2.xlsx", true).Result;
        }

        static void ReadExcel_3()
        {
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions()
                {
                    TempType = Extension.TempType.STANDARD,
                    DataBeginRow = 2,
                    HeaderRow = 1
                }
            );

            DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo3.xlsx", true).Result;
        }

        static void BuildExceBigData()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient2();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DataTable dt = sqlClient
                .HiSql("select * from S4_REP_ZRMB52_2022_05_12")
                .Take(100000)
                .Skip(1)
                .ToTable();
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct(
                "S4_REP_ZRMB52_2022_05_12"
            );
            sw.Stop();
            Console.WriteLine($"获取{dt.Rows.Count}条 耗时{sw.Elapsed}秒");

            // TempType = Extension.TempType.HEADER
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER }
            );
            excel.Add(new Extension.ExcelHeader(1).Add("表名").Add("S4_REP_ZRMB52_2022_05_12")); //标识表名

            Extension.ExcelHeader excelHeader = new Extension.ExcelHeader(2);
            Extension.ExcelHeader excelHeader3 = new Extension.ExcelHeader(3);
            foreach (DataColumn dataColumn in dt.Columns)
            {
                HiColumn hiColumn = tabInfo
                    .Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName))
                    .FirstOrDefault();
                if (hiColumn != null)
                {
                    excelHeader.Add(
                        string.IsNullOrEmpty(hiColumn.FieldDesc)
                            ? dataColumn.ColumnName
                            : hiColumn.FieldDesc
                    );
                }
                else
                    excelHeader.Add(dataColumn.ColumnName);

                excelHeader3.Add(dataColumn.ColumnName);
            }
            excel.Add(excelHeader); //字段中文描述
            excel.Add(excelHeader3); //字段名

            sw.Restart();
            sw.Start();
            //生成excel
            excel.WriteExcel(dt, @"D:\data\S4_REP_ZRMB52_2022_05_12.xlsx");
            sw.Stop();
            Console.WriteLine($"写入excel 数据插入{dt.Rows.Count}条 耗时{sw.Elapsed}秒");
        }

        /// <summary>
        /// 生成完整抬头的excel
        /// </summary>
        static void BuildExcel_1()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");

            // TempType = Extension.TempType.HEADER
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER }
            );
            excel.Add(new Extension.ExcelHeader(1).Add("表名").Add("GD_UniqueCodeInfo")); //标识表名

            Extension.ExcelHeader excelHeader = new Extension.ExcelHeader(2);
            Extension.ExcelHeader excelHeader3 = new Extension.ExcelHeader(3);
            foreach (DataColumn dataColumn in dt.Columns)
            {
                HiColumn hiColumn = tabInfo
                    .Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName))
                    .FirstOrDefault();
                if (hiColumn != null)
                {
                    excelHeader.Add(
                        string.IsNullOrEmpty(hiColumn.FieldDesc)
                            ? dataColumn.ColumnName
                            : hiColumn.FieldDesc
                    );
                }
                else
                    excelHeader.Add(dataColumn.ColumnName);

                excelHeader3.Add(dataColumn.ColumnName);
            }
            excel.Add(excelHeader); //字段中文描述
            excel.Add(excelHeader3); //字段名

            //生成excel
            excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo1.xlsx");
        }

        /// <summary>
        /// 生成标准excel
        /// </summary>
        static void BuildExcel_2()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");

            HiSql.Extension.Excel excel = new HiSql.Extension.Excel();
            //生成excel
            excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo2.xlsx");
        }

        /// <summary>
        /// 生成自定义抬头excel
        /// </summary>
        static void BuildExcel_3()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel();
            Extension.ExcelHeader excelHeader = new Extension.ExcelHeader(2);

            foreach (DataColumn dataColumn in dt.Columns)
            {
                HiColumn hiColumn = tabInfo
                    .Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName))
                    .FirstOrDefault();
                if (hiColumn != null)
                {
                    excelHeader.Add(
                        string.IsNullOrEmpty(hiColumn.FieldDesc)
                            ? dataColumn.ColumnName
                            : hiColumn.FieldDesc
                    );
                }
                else
                    excelHeader.Add(dataColumn.ColumnName);
            }
            excel.Add(excelHeader); //字段中文描述

            //生成excel
            excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo3.xlsx");
        }

        /// <summary>
        /// 解析支付宝帐单 excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sqlClient"></param>
        static void InsertPay()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            HiSql.Extension.Excel excel = new HiSql.Extension.Excel(
                new Extension.ExcelOptions() { TempType = Extension.TempType.STANDARD, EndRow = -1 }
            );
            //DataTable dt = excel.ExcelToDataTable(@"C:\Users\admin\Downloads\2088531658652104-20220505-086373743-账务组合查询.xls\20220505.xlsx", true);
            DataTable dt = excel
                .ExcelToDataTable(
                    @"C:\Users\admin\Downloads\2088531658652104-20220509-086513029-账务组合查询.xls\20220511.xlsx",
                    true
                )
                .Result;

            List<dynamic> list = new List<dynamic>();

            foreach (DataRow dr in dt.Rows)
            {
                TDynamic orderInfo = new TDynamic();
                var o = orderInfo.ToDynamic();

                o.PayNo = dr["支付宝流水号"].ToString().Trim();
                o.InDate = Convert.ToDateTime(dr["入账时间"].ToString().Trim());

                o.PayTrade = dr["支付宝交易号"].ToString().Trim();
                o.MerchantNo = dr["商户订单号"].ToString().Trim();
                o.TradeType = dr["账务类型"].ToString().Trim();
                o.InMoney = string.IsNullOrEmpty(dr["收入（+元）"].ToString().Trim())
                    ? 0
                    : Convert.ToDecimal(dr["收入（+元）"].ToString().Trim());
                o.OutMoney = string.IsNullOrEmpty(dr["支出（-元）"].ToString().Trim())
                    ? 0
                    : Convert.ToDecimal(dr["支出（-元）"].ToString().Trim());
                o.BlanceMoney = string.IsNullOrEmpty(dr["账户余额（元）"].ToString().Trim())
                    ? 0
                    : Convert.ToDecimal(dr["账户余额（元）"].ToString().Trim());
                o.ServiceMoney = string.IsNullOrEmpty(dr["服务费（元）"].ToString().Trim())
                    ? 0
                    : Convert.ToDecimal(dr["服务费（元）"].ToString().Trim());
                o.PayMethod = dr["支付渠道"].ToString().Trim();
                o.SignProduct = dr["签约产品"].ToString().Trim();
                o.OppAccount = dr["对方账户"].ToString().Trim();
                o.OppAccountName = dr["对方名称"].ToString().Trim();
                o.BlankOrderNo = dr["银行订单号"].ToString().Trim();
                o.ProductName = dr["商品名称"].ToString().Trim();
                o.Descript = dr["备注"].ToString().Trim();
                o.BasicBusOrderNo = dr["业务基础订单号"].ToString().Trim();
                o.BusOrderNo = dr["业务订单号"].ToString().Trim();
                o.AccFrom = dr["业务账单来源"].ToString().Trim();
                o.BusDesc = dr["业务描述"].ToString().Trim();
                o.PayDesc = dr["付款备注"].ToString().Trim();
                o.TradeNumber = "";

                Dictionary<string, string> _dic = HiSql.Tool.RegexGrp(
                    @"(?<=T\d+P)(?<order>\d{19,})",
                    o.MerchantNo
                );

                if (_dic.ContainsKey("order"))
                {
                    o.TradeNumber = _dic["order"].ToString();
                }
                else
                    o.TradeNumber = "";

                list.Add(o);
            }

            sqlClient.Modi("H_PayDetail", list).ExecCommand();
        }

        /// <summary>
        /// 带图片DataTable转Excel导出
        /// </summary>
        static async Task HiSqlExcelV2ImageExport()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            DataTable dt = sqlClient.HiSql("select * from ThProductSpu").Skip(1).Take(10).ToTable();
            //通过dt获取表头初始化List<DataTableHeaderInfo> headers
            var headers = new List<DataTableHeaderInfo>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var headName = dt.Columns[i].ColumnName;
                var headObj = new DataTableHeaderInfo
                {
                    Title = headName,
                    Description = headName,
                    ValueType = ExcelValueType.Text,
                };
                if (headObj.Title == "PicPath")
                {
                    headObj.ValueType = ExcelValueType.Image;
                }
                headers.Add(headObj);
            }
            var savePath = AppContext.BaseDirectory + "/Export/test.xlsx";
            var excelObj = new ExcelExportHelperV2(savePath, true);

            await excelObj.WriteDataTableToSheet("测试表", headers, dt, "sheet1");

            await excelObj.WriteDataTableToSheet("测试2", headers, dt, "sheet2");

            excelObj.SaveSheetToFile();
        }
    }
}
