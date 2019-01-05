using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Main.DB.Import
{
    public class ImportData
    {
        /// <summary>
        /// 打开EXCEL并将结果集以DATASET型式返回
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable OpenExceltoDt(string fileAddress, string tableName)
        {
            var resultdt = new DataTable();

            try
            {
                //使用NPOI技术进行导入EXCEL至DATATABLE
                var importExcelDt = OpenExcelToDataTable(fileAddress, tableName);
                //将从EXCEL过来的记录集为空的行清除
                var tempdt = RemoveEmptyRows(importExcelDt);
                //对获取出来的DT中的"AKZO色母量"进行计算(注:利用同一个制造商及AKZO色号内的累积量,计算出对应的色母量)
                resultdt = GenerateColorParcent(tempdt);
            }
            catch (Exception ex)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return resultdt;
        }

        /// <summary>
        /// 读取EXCEL内容到DATATABLE内
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable OpenExcelToDataTable(string fileAddress,string tableName)
        {
            IWorkbook wk;
            var dt=new DataTable();

            using (var fsRead = File.OpenRead(fileAddress))
            {
                wk = new XSSFWorkbook(fsRead);
                //获取第一个sheet
                var sheet = wk.GetSheetAt(0);
                //获取第一行
                var hearRow = sheet.GetRow(0);

                //创建列标题
                //"Akzo配方表"使用
                if (tableName == "AkzoFormula")
                {
                    for (int i = hearRow.FirstCellNum; i < hearRow.Cells.Count; i++)
                    {
                        var dataColumn = new DataColumn();

                        switch (i)
                        {
                            case 0:
                                dataColumn.ColumnName = "制造商";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 1:
                                dataColumn.ColumnName = "Akzo色号";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 2:
                                dataColumn.ColumnName = "Akzo色母";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 3:
                                dataColumn.ColumnName = "累积量";
                                dataColumn.DataType = Type.GetType("System.Decimal"); 
                                break;
                            case 4:
                                dataColumn.ColumnName = "Akzo色母量";
                                dataColumn.DataType = Type.GetType("System.Decimal"); 
                                break;
                        }
                        dt.Columns.Add(dataColumn);
                    }
                }
                //"色母对照表"使用
                else
                {
                    for (int i = hearRow.FirstCellNum; i < hearRow.Cells.Count; i++)
                    {
                        var dataColumn = new DataColumn();

                        switch (i)
                        {
                            case 0:
                                dataColumn.ColumnName = "Akzo色母";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 1:
                                dataColumn.ColumnName = "雅图色母";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 2:
                                dataColumn.ColumnName = "浓度转换系数";
                                dataColumn.DataType = Type.GetType("System.Decimal"); 
                                break;
                            case 3:
                                dataColumn.ColumnName = "产品系列类型";
                                dataColumn.DataType = Type.GetType("System.Int32"); 
                                break;
                        }
                        dt.Columns.Add(dataColumn);
                    }
                }

                //创建完标题后,开始从第二行起读取对应列的值
                for (var r = 1; r <= sheet.LastRowNum; r++)
                {
                    var result = false;
                    var dr = dt.NewRow();
                    //获取当前行(注:只能获取行中有值的项,为空的项不能获取)
                    var row = sheet.GetRow(r);
                    //读取每列
                    for (var j = 0; j < row.Cells.Count; j++)
                    {
                        //循环获取行中的单元格
                        var cell = row.GetCell(j);
                        dr[j] = GetCellValue(cell);
                        //全为空就不取
                        if (dr[j].ToString() != "")
                        {
                            result = true;
                        }
                    }
                    if (result == true)
                    {
                        //把每行增加到DataTable
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 检查单元格的数据类型并获其中的值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (DateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型                    
                    return cell.ToString();
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        var e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        /// <summary>
        ///  将从EXCEL导入的DATATABLE的空白行清空
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected DataTable RemoveEmptyRows(DataTable dt)
        {
            var removeList = new List<DataRow>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var isNull = true;
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    //将不为空的行标记为False
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {
                        isNull = false;
                    }
                }
                //将整行都为空白的记录进行记录
                if (isNull)
                {
                    removeList.Add(dt.Rows[i]);
                }
            }

            //将整理出来的所有空白行通过循环进行删除
            for (var i = 0; i < removeList.Count; i++)
            {
                dt.Rows.Remove(removeList[i]);
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable内的记录导入至数据库(使用SqlBulkCopy类进行批量插入)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ImportExcelToDb(string tableName, DataTable dt)
        {
            var result = "0";

            try
            {
                #region
                //if (tableName == "ColorantContrast")
                //    for (var i = 0; i < dt.Rows.Count; i++)
                //    {
                //        for (var j = 0; j < dt.Columns.Count; j++)
                //        {
                //            dt.Rows[i].BeginEdit();
                //            dt.Rows[i][3] = DateTime.Now.Date;
                //            dt.Rows[i][4] = brandId;
                //            dt.Rows[i].EndEdit();
                //        }
                //    }
                #endregion

                //若tablename为"AkzoFormula"时,表示要将传过来的DT分拆两个表进行插入
                if (tableName == "AkzoFormula")
                {
                    //先将传过来的DT信息拆分并放到一个表头临时表并插入至数据表(插入至AkzoFormula表)
                    var fdt = CreateFormualDt();
                    var akzoDt = ChangeDt(dt, fdt, 0);
                    Importdt("AkzoFormula", akzoDt);

                    //再将传过来的DT信息拆分并放到一个表体临时表并插入至数据表(插入至AkzoFormulaEntry表)
                    var entryDt = CreateFormualEntryDt();
                    var akzoEntrydt = ChangeDt(dt, entryDt, 1);
                    Importdt("AkzoFormulaEntry", akzoEntrydt);
                }
                //否则tablename为"ColorCodeContrast",表示插入色母对照表,按常规使用SqlBulkCopy方法进行插入数据
                else
                {
                    Importdt(tableName,dt);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 将记录导入至对应的表格
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        private void Importdt(string tableName, DataTable dt)
        {
            var conn = new Conn();
            var sqlcon = conn.GetConnectionString();
            // sqlcon.Open(); 若返回一个SqlConnection的话,必须要显式打开 
            //注:1)要插入的DataTable内的字段数据类型必须要与数据库内的一致;并且要按数据表内的字段顺序 2)SqlBulkCopy类只提供将数据写入到数据库内
            using (var sqlBulkCopy = new SqlBulkCopy(sqlcon))
            {
                sqlBulkCopy.BatchSize = 500;                    //表示以500行 为一个批次进行插入
                sqlBulkCopy.DestinationTableName = tableName;  //数据库中对应的表名
                sqlBulkCopy.NotifyAfter = dt.Rows.Count;      //赋值DataTable的行数
                sqlBulkCopy.WriteToServer(dt);               //数据导入数据库
                sqlBulkCopy.Close();                        //关闭连接 
            }
            // sqlcon.Close();
        }

        /// <summary>
        /// 创建指定的表格表头信息(为最后插入时作准备)
        /// </summary>
        /// <returns></returns>
        private DataTable CreateFormualDt()
        {
            var dt = new DataTable();
            try
            {
                for (var i = 0; i < 2; i++)
                {
                    var dc = new DataColumn();

                    switch (i)
                    {
                        case 0:
                            dc.ColumnName = "Factory";  //制造商
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 1:
                            dc.ColumnName = "ColorCode"; //Akzo色号
                            dc.DataType = Type.GetType("System.String");
                            break;
                    }
                    dt.Columns.Add(dc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// 创建指定的表格表体信息(为最后插入时作准备)
        /// </summary>
        /// <returns></returns>
        private DataTable CreateFormualEntryDt()
        {
            var dt = new DataTable();
            try
            {
                for (var i = 0; i < 4; i++)
                {
                    var dc = new DataColumn();

                    switch (i)
                    {
                        case 0:
                            dc.ColumnName = "ColorCode";        //Akzo色号
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 1:
                            dc.ColumnName = "AkzoColorant";     //Akzo色母
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 2:
                            dc.ColumnName = "Cumulant";         //Akzo累积量
                            dc.DataType = Type.GetType("System.Decimal"); 
                            break;
                        case 3:
                            dc.ColumnName = "ColorantParent";  //AKZO色母量
                            dc.DataType = Type.GetType("System.Decimal");
                            break;
                    }
                    dt.Columns.Add(dc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// 分拆方法
        /// </summary>
        /// <param name="dt">从DataGrild内获取的DATATABLE</param>
        /// <param name="newdt">返回的新DATATABLE</param>
        /// <param name="markId">0:表头 1:表体</param>
        /// <returns></returns>
        private DataTable ChangeDt(DataTable dt,DataTable newdt,int markId)
        {
            //中转值
            var factory = string.Empty;
            var akzoCode = string.Empty;

            try
            {
                switch (markId)
                {
                    //为0时表示要创建表头信息
                    case 0:
                        foreach (DataRow rows in dt.Rows)
                        {
                            var newrow = newdt.NewRow();
                            if (factory == "" && akzoCode == "")
                            {
                                newrow["Factory"] = rows["制造商"];
                                newrow["ColorCode"] = rows["Akzo色号"];

                                factory = Convert.ToString(rows["制造商"]);
                                akzoCode = Convert.ToString(rows["Akzo色号"]);
                            }
                            else
                            {
                                if (factory == Convert.ToString(rows["制造商"]) && akzoCode == Convert.ToString(rows["Akzo色号"]))
                                {
                                    continue;
                                }
                                else
                                {
                                    newrow["Factory"] = rows["制造商"];
                                    newrow["ColorCode"] = rows["Akzo色号"];

                                    factory = Convert.ToString(rows["制造商"]);
                                    akzoCode = Convert.ToString(rows["Akzo色号"]);
                                }
                            }
                            newdt.Rows.Add(newrow);
                        }
                        break;
                    //为1时表示要创建表体信息
                    case 1:
                        foreach (DataRow rows in dt.Rows)
                        {
                            var newrow = newdt.NewRow();
                            newrow["ColorCode"] = rows["Akzo色号"];
                            newrow["AkzoColorant"] = rows["Akzo色母"];
                            newrow["Cumulant"] = rows["累积量"];
                            newrow["ColorantParent"] = rows["Akzo色母量"];
                            newdt.Rows.Add(newrow);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return newdt;
        }

        /// <summary>
        /// 利用累积量计算出对应的Akzo色母量(注:对传递过来的DT进行更新并赋值；最后输出)
        /// </summary>
        /// <param name="tempdt">为从EXCEL里导入的DT记录</param>
        /// <returns></returns>
        private DataTable GenerateColorParcent(DataTable tempdt)
        {
            string markid=string.Empty;
            //记录累积量
            decimal cumulantTemp=0; 

            try
            {
                //创建表头临时表
                var fdt = CreateFormualDt();
                //根据EXCEL的DT获取其表头信息,以DT为返回结果
                var akzoDt = ChangeDt(tempdt, fdt, 0);

                foreach (DataRow akzorows in akzoDt.Rows)
                {
                    var rows = tempdt.Select("Akzo色号 = '" + akzorows[1] + "' and 制造商 = '" + akzorows[0] + "'");

                    foreach (DataRow row in rows)
                    {
                        row.BeginEdit();
                        //运算AKZO色母量;注:除第一行的AKZO色母量就是累积量外。其它行的AKZO色母量就是该行的累积量与上一行的累积量进行相减得出
                        if (markid == "")
                        {
                            row[4] = Convert.ToDecimal(row[3]);
                            cumulantTemp = Convert.ToDecimal(row[3]);
                            markid = "Y";
                        }
                        else
                        {
                            row[4] = Convert.ToDecimal(row[3])-cumulantTemp;
                            cumulantTemp = Convert.ToDecimal(row[3]);
                        }
                        row.EndEdit();
                    }
                    //每次循环完将markid标记初始化
                    markid = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return tempdt;
        }
    }
}
