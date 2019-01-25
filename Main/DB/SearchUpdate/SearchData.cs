using System;
using System.Data;
using System.Data.SqlClient;
using NPOI.OpenXmlFormats.Dml;

namespace Main.DB.SearchUpdate
{
    public class SearchData
    {
        Conn conn=new Conn();

        #region 查询制造商列表

            private string _SearchList = @"SELECT x.Factory
                                           FROM (
                                                    SELECT '全部' AS Factory,0 AS fseq
                                                    UNION
                                                    SELECT DISTINCT A.Factory,1 AS fseq
                                                    FROM dbo.AkzoFormula A)x
                                           ORDER BY x.fseq";

        #endregion

        #region 查询产品系列对应的色母对照表明细记录 作用:查询“色母对照表”时使用

        private string _SearchProdList = @"
                                                   SELECT a.AkzoColorant as 'Akzo色母',a.Colorant as '雅图色母', a.Num as '浓度转换系数'
                                                   FROM dbo.ColorCodeContrast a WHERE a.TypeId='{0}' and a.MacAdd='{1}' and a.ImportDt='{2}'
                                              ";

        #endregion

        #region 根据制造商查询AKZO配方明细 作用:查询“配方记录表”时使用

        private string _SearchFormula = @"
                                                  SELECT B.ColorCode AS 'Akzo色号',B.AkzoColorant AS 'Akzo色母',B.Cumulant as '累积量',B.ColorantParent as 'Akzo色母量'
                                                  FROM dbo.AkzoFormula A
                                                  INNER JOIN dbo.AkzoFormulaEntry B ON A.ColorCode=B.ColorCode
                                                  WHERE A.Factory='{0}' and a.MacAdd='{1}' and a.ImportDt='{2}'
                                             ";

        #endregion

        #region 根据制造商查询AKZO配方明细(全部) 作用:查询“配方记录表”时使用

        private string _SearchFormulaAll = @"
                                                    SELECT A.ColorCode AS 'Akzo色号',A.AkzoColorant AS 'Akzo色母',A.Cumulant as '累积量',A.ColorantParent as 'Akzo色母量'
                                                    FROM dbo.AkzoFormulaEntry A
                                                    where  a.MacAdd='{0}' and a.ImportDt='{1}'
                                                ";

        #endregion

        #region 获取AkzoFormula表信息(表头信息) 导入记录时使用

        private string _SearchAkzo = @"
                                            SELECT Factory,ColorCode FROM AkzoFormula a where a.MacAdd='{0}' AND a.ImportDt='{1}'
                                      ";

        #endregion


        /// <summary>
        /// 获取“制造商”下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable SearchList()
        {
            var ds=new DataSet();

            try
            {
                using (var sql = new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter=new SqlDataAdapter(_SearchList,sql);
                    sqlDataAdapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds.Tables[0].Rows.Clear();
                ds.Tables[0].Columns.Clear();
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 根据产品系列下拉框所选择的条件。获取对应的色母对照表明细记录
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="macadd"></param>
        /// <param name="dtTime"></param>
        /// <returns></returns>
        public DataTable SearchProductList(int pid,string macadd,DateTime dtTime)
        {
            var ds=new DataSet();

            try
            {
                using (var sql=new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter=new SqlDataAdapter(string.Format(_SearchProdList,pid,macadd,dtTime),sql);
                    sqlDataAdapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds.Tables[0].Rows.Clear();
                ds.Tables[0].Columns.Clear();
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 根据制造商获取对应的AKZO配方明细记录
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="macadd"></param>
        /// <param name="dtTime"></param>
        /// <returns></returns>
        public DataTable SearchdtlList(string factory,string macadd,DateTime dtTime)
        {
            var ds=new DataSet();
            try
            {
                using (var sql=new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter=new SqlDataAdapter(factory == "全部" ? string.Format(_SearchFormulaAll,macadd,dtTime) : 
                        string.Format(_SearchFormula, factory,macadd,dtTime), sql);
                    sqlDataAdapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds.Tables[0].Rows.Clear();
                ds.Tables[0].Columns.Clear();
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 查询AkzoFormula表头信息记录(在导入信息的时候作判断是否存在之用)
        /// </summary>
        /// <param name="macadd"></param>
        /// <returns></returns>
        public DataTable SearchFormulaList(string macadd)
        {
            var ds=new DataSet();
            try
            {
                using (var sql = new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter = new SqlDataAdapter(string.Format(_SearchAkzo,macadd,DateTime.Now.Date),sql);
                    sqlDataAdapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds.Tables[0].Rows.Clear();
                ds.Tables[0].Columns.Clear();
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];
        }
    }
}
