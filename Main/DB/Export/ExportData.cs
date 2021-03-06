﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using NPOI.XSSF.UserModel;

namespace Main.DB.Export
{
    public class ExportData
    {
        Conn conn = new Conn();

        #region 删除记录

        private string _DelRecord = @"
                                         DELETE FROM dbo.AkzoFormula WHERE MacAdd='{0}' AND ImportDt='{1}';
                                         DELETE FROM dbo.AkzoFormulaEntry WHERE MacAdd='{0}' AND ImportDt='{1}';
                                         DELETE FROM dbo.ColorCodeContrast WHERE MacAdd='{0}' AND ImportDt='{1}';
                                    ";

        #endregion

        /// <summary>
        /// 导出数据至EXCEL
        /// </summary>
        /// <param name="fileAddress">要导出的文件地址</param>
        /// <param name="dt">DataGridView数据源</param>
        /// <param name="macadd">用户MAC地址</param>
        /// <returns></returns>
        public string ExportDttoExcel(string fileAddress,DataTable dt,string macadd)
        {
            //流程:1)创建EXCEL导出所需的列,2)从DT内循环读取数据
            var result ="0";

            try
            {
                //声明一个WorkBook
                var xssfWorkbook = new XSSFWorkbook();
                for (var i = 0; i < 1; i++)//i为EXCEL的Sheet页数ID
                {
                    //为WorkBook创建work
                    var sheet = xssfWorkbook.CreateSheet("Sheet" + (i + 1)); 
                    //创建"标题行"
                    var row = sheet.CreateRow(0);
                    for (var l = 0; l < dt.Columns.Count + 1; l++)
                    {
                        //设置列宽度
                        sheet.SetColumnWidth(l, (int)((20 + 0.72) * 256));
                        //创建标题
                        switch (l)
                        {
                            case 0:
                                row.CreateCell(l).SetCellValue("制造商");
                                break;
                            case 1:
                                row.CreateCell(l).SetCellValue("Akzo色号");
                                break;
                            case 2:
                                row.CreateCell(l).SetCellValue("Akzo色母");
                                break;
                            case 3:
                                row.CreateCell(l).SetCellValue("累积量");
                                break;
                            case 4:
                                row.CreateCell(l).SetCellValue("Akzo色母量");
                                break;
                            case 5:
                                row.CreateCell(l).SetCellValue("浓度转换系数");
                                break;
                            case 6:
                                row.CreateCell(l).SetCellValue("雅图色母");
                                break;
                            case 7:
                                row.CreateCell(l).SetCellValue("雅图新色母量");
                                break;
                        }
                    }
                    //获取DT内的行记录步骤
                    for (var j = 0; j < dt.Rows.Count; j++)
                    {
                        //创建行(从第二行开始)
                        row = sheet.CreateRow(j + 1);
                        #region 设置行的第一行的值为自增值
                        //row.CreateCell(0).SetCellValue(j + 1);
                        #endregion
                        //循环获取DT内的列值记录
                        for (var k = 0; k < dt.Columns.Count; k++)
                        {
                           // row.CreateCell(k + 1).SetCellValue(dt.Rows[i * 10000 + j][k].ToString());
                           row.CreateCell(k).SetCellValue(dt.Rows[j][k].ToString());
                        }
                    }
                }
                //写入数据
                var file = new FileStream(fileAddress, FileMode.Create);
                xssfWorkbook.Write(file);
                file.Close();
                //在导出成功后,根据指定条件进行删除对应的记录
                DelDtRecord(macadd);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据指定条件删除“Akzo配方表”及"色母对照表"记录
        /// </summary>
        private void DelDtRecord(string macadd)
        {
            try
            {
                using (var sql = new SqlConnection(conn.GetConnectionString()))
                {
                    sql.Open();
                    var sqlCommand = new SqlCommand(string.Format(_DelRecord,macadd,DateTime.Now.Date),sql);
                    sqlCommand.ExecuteNonQuery();
                    sql.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
