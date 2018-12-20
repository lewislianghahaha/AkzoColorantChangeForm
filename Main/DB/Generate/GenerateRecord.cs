using System;
using System.Data;
using System.Data.SqlClient;

namespace Main.DB.Generate
{
    public class GenerateRecord
    {
        #region 根据指定条件查询出“AkzoFormula”表头信息

            private string _searchAkzo = @"
                                            SELECT * FROM dbo.AkzoFormula WHERE Factory='{0}'
                                          ";

        #endregion

        #region 根据指定条件查询出“AkzoFormula”表头信息(当制造商选了“全部”时执行)

        private string _searchAkzoAll = @"
                                            SELECT * FROM dbo.AkzoFormula
                                          ";

        #endregion

        #region 根据指定条件查询出"AkzoFormulaEntry"表体信息

        private string _searchAkzoEntry = @"
                                                    SELECT b.* FROM dbo.AkzoFormula a
                                                    INNER JOIN dbo.AkzoFormulaEntry b ON a.FormulaCode=b.FormulaCode
                                                    WHERE a.Factory='{0}'
                                               ";

        #endregion

        #region 根据指定条件查询出"AkzoFormulaEntry"表体信息(当制造商选了“全部”时执行)

        private string _searchAkzoEntryAll = @"
                                                   SELECT * FROM AkzoFormulaEntry
                                              ";

        #endregion

        #region 根据条件查询出"ColorCodeConstrast"表体信息

        private string _searchColordtl = @"
                                                  SELECT * FROM dbo.ColorCodeContrast WHERE TypeId='{0}'
                                              ";

        #endregion

        /// <summary>
        /// “运算"功能主体方法(目的:通过算法将两个表合拼并计算出新的值,最后存放到一个新的DataTable内)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="productid"></param>
        /// <returns></returns>
        public DataTable GetRecordToDataTable(string factory,int productid)
        {
            var resultdt=new DataTable();

            try
            {
                //先创建一个用于保存结果集的DATATABLE
                var newdt = CreateDt();
                //创建一个用于保存循环记录的DATATABLE
                var tempdt = TempDt();
                //根据条件获取AKZO配方相关记录(表头)
                var akzodt = GetDtDtl(factory,0, productid);
                //根据条件获取AKZO配方相关记录(表体)
                var akzoEntrydt = GetDtDtl(factory, 1, productid);
                //根据条件获取色母对照表相关记录
                var colorantdt = GetDtDtl(factory, 2 ,productid);
                //最后使用上面的DATATABLE进行运算得出最终的结果集。结果集以DT来存储
                resultdt = GenerateColorantPercent(newdt,tempdt,akzodt,akzoEntrydt,colorantdt);
            }
            catch (Exception ex)
            {
                resultdt.Columns.Clear();
                resultdt.Rows.Clear();
                throw new Exception(ex.Message);
            }

            return resultdt;
        }

        /// <summary>
        /// “运算”功能的核心方法(重)
        /// </summary>
        /// <param name="newdt">结果集DT</param>
        /// <param name="tempdt">临时表DT</param>
        /// <param name="akzodt">AKZO表头DT</param>
        /// <param name="akzoEntrydt">AKZO表体DT</param>
        /// <param name="colorantdt">色母对照表DT</param>
        /// <returns></returns>
        private DataTable GenerateColorantPercent(DataTable newdt,DataTable tempdt,DataTable akzodt,DataTable akzoEntrydt,DataTable colorantdt)
        {
            //中间值
            decimal tempNum = 0;
            //合计数
            decimal total = 0;
            //转换系数
            decimal ChangeNum = 0;

            try
            {
                //以AKZO表头作循环条件
                foreach (DataRow rows in akzodt.Rows)
                {
                    //根据AKZO表头的"AKZO号"作为条件，在ENTRY表内查询相关记录集
                    var row = akzoEntrydt.Select("FormulaCode='" + rows[1] + "'");
                    //循环读取row数组(注:在此过程中要计算出中间值以及总计)
                    for (var i = 0; i < row.Length; i++)
                    {
                        var newrow = tempdt.NewRow();
                        newrow["FactoryCode"] = rows["Factory"];  //制造商
                        newrow["AkzoCode"] = rows["FormulaCode"]; //AKZO号
                        newrow["AkzoColorant"] = row[i][1];       //AKZO色母
                        newrow["AkzoColorantParent"] = row[i][2]; //AKZO色母量
                        //以AKZO色母号为条件,查询"色母对照表"的相关记录
                        var colorantrow = colorantdt.Select("AkzoColorant='" + row[i][1] + "'");
                        //当查询不到结果时,转换系数为0，而雅图色母为空显示
                        if (colorantrow.Length < 1)
                        {
                            newrow["YatuColorant"] = "";                 //雅图色母
                            newrow["Num"] = 0;                          //浓度转换系数
                            ChangeNum = 0;                             //用变量记录转换系数
                        }
                        else
                        {
                            newrow["YatuColorant"] = colorantrow[0][1];           //雅图色母
                            newrow["Num"] = colorantrow[0][2];                   //浓度转换系数
                            ChangeNum = Convert.ToDecimal(colorantrow[0][2]);   //用变量记录转换系数
                        }
                        //色母量中间值
                        newrow["tempNum"] = decimal.Round(Convert.ToDecimal(row[i][2]) * ChangeNum, 3);                

                        tempdt.Rows.Add(newrow);
                        //计算并记录中间值 公式:AKZO色母量*浓度转换系数
                        tempNum = decimal.Round(Convert.ToDecimal(row[i][2]) * ChangeNum, 3);
                        //计算并记录合计数 公式:SUM(中间值)
                        total += tempNum;
                    }
                    //循环临时表 作用:1)计算雅图最终色母量 2)将结果集赋给Newdt内 (注:最终色母量=中间值*100/合计数)
                    foreach (DataRow temprows in tempdt.Rows)
                    {
                        var finialrow = newdt.NewRow();
                        finialrow["制造商"] = temprows["FactoryCode"];
                        finialrow["Akzo配方号"] = temprows["AkzoCode"];
                        finialrow["Akzo色母"] = temprows["AkzoColorant"];
                        finialrow["Akzo色母量"] = temprows["AkzoColorantParent"];
                        finialrow["浓度转换系数"] = temprows["Num"];
                        finialrow["雅图色母"] = temprows["YatuColorant"];
                        finialrow["雅图新色母量"] = decimal.Round(Convert.ToDecimal(temprows["tempNum"])*100 / total,3);
                        newdt.Rows.Add(finialrow);
                    }
                    //执行完成后。将TempDt临时表的行清空,作下一次循环使用
                    tempdt.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                newdt.Rows.Clear();
                newdt.Columns.Clear();
                throw new Exception(ex.Message);
            }

            return newdt;
        }

        /// <summary>
        /// 根据条件获取数据集相关记录（包括AKZO表头 表体以及色母对照表）
        /// </summary>
        /// <param name="factory">制造商</param>
        /// <param name="markid">0:AKZO表头 1:AKZO表体 2:色母对照表</param>
        /// <param name="pid">产品系列ID</param>
        /// <returns></returns>
        private DataTable GetDtDtl(string factory,int markid,int pid)
        {
            var ds = new DataSet();

            try
            {
                ds = Getdtl(markid, factory,pid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 根据条件从指定的物理表内获取记录集
        /// </summary>
        /// <param name="markid">分支判断ID</param>
        /// <param name="factory">制造商</param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private DataSet Getdtl(int markid,string factory,int pid)
        {
            var ds = new DataSet();
            var sqlscript = string.Empty;
            var conn = new Conn();

            try
            {
                switch (markid)
                {
                    //0:表示AKZO表头信息获取
                    case 0:
                        sqlscript = factory == "全部" ? _searchAkzoAll : string.Format(_searchAkzo, factory);
                        break;
                    //1:表示AKZO表体信息获取
                    case 1:
                        sqlscript = factory == "全部" ? _searchAkzoEntryAll : string.Format(_searchAkzoEntry, factory);
                        break;
                    //2:表示色母对照表信息获取
                    case 2:
                        sqlscript = string.Format(_searchColordtl,pid);
                        break;
                }

                using (var sql = new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter = new SqlDataAdapter(sqlscript, sql);
                    sqlDataAdapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
            return ds;
        }

        /// <summary>
        /// 创建指定的表格信息(为最后的结果DT作准备)
        /// </summary>
        /// <returns></returns>
        private DataTable CreateDt()
        {
            var dt = new DataTable();

            try
            {
                for (var i = 0; i < 7; i++)
                {
                    var dc = new DataColumn();

                    switch (i)
                    {
                        case 0:
                            dc.ColumnName = "制造商";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 1:
                            dc.ColumnName = "Akzo配方号";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 2:
                            dc.ColumnName = "Akzo色母";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 3:
                            dc.ColumnName = "Akzo色母量";
                            dc.DataType = Type.GetType("System.Decimal"); 
                            break;
                        case 4:
                            dc.ColumnName = "浓度转换系数";
                            dc.DataType = Type.GetType("System.Decimal");
                            break;
                        case 5:
                            dc.ColumnName = "雅图色母";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 6:
                            dc.ColumnName = "雅图新色母量";
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
        /// 临时表:作用->保存以AKZO号为区间的数据集,并将最终结果插入至NEWDT内;注:每一次循环后会进行清空Row.Clear()
        /// </summary>
        /// <returns></returns>
        private DataTable TempDt()
        {
            var dt = new DataTable();
            try
            {
                for (var i = 0; i < 7; i++)
                {
                    var dc = new DataColumn();

                    switch (i)
                    {
                        case 0: //制造商
                            dc.ColumnName = "FactoryCode";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 1: //AKZO号
                            dc.ColumnName = "AkzoCode";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 2: //AKZO色母
                            dc.ColumnName = "AkzoColorant";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 3: //AKZO色母量
                            dc.ColumnName = "AkzoColorantParent";
                            dc.DataType = Type.GetType("System.Decimal");
                            break;
                        case 4: //浓度转换系数
                            dc.ColumnName = "Num";
                            dc.DataType = Type.GetType("System.Decimal");
                            break;
                        case 5: //Yatu色母
                            dc.ColumnName = "YatuColorant";
                            dc.DataType = Type.GetType("System.String");
                            break;
                        case 6://色母量中间值
                            dc.ColumnName = "tempNum";
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
    }
}
