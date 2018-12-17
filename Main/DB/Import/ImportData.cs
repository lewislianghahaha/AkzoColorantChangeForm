using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var importExcelDt = new DataTable();
            var dt = new DataTable();

            try
            {
                //使用NPOI技术进行导入EXCEL至DATATABLE
                importExcelDt = OpenExcelToDataTable(fileAddress, tableName);
                //将从EXCEL过来的记录集为空的行清除
                dt = RemoveEmptyRows(importExcelDt);
            }
            catch (Exception ex)
            {
                dt.Rows.Clear();
                dt.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dt;
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
                                dataColumn.ColumnName = "Akzo配方号";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 2:
                                dataColumn.ColumnName = "Akzo色母";
                                dataColumn.DataType = Type.GetType("System.String");
                                break;
                            case 3:
                                dataColumn.ColumnName = "Akzo色母量";
                                dataColumn.DataType = Type.GetType("System.Decimal"); 
                                break;
                        }
                        dt.Columns.Add(dataColumn);
                    }
                }
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
                    //获取当前行
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
                    return cell.ToString();//
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
                //将整行都为空白的记录
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
        /// <returns></returns>
        public string ImportExcelToDb(string tableName, DataTable dt)
        {
            return null;
        }
    }
}
