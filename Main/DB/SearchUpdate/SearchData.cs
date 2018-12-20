using System;
using System.Data;
using System.Data.SqlClient;

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

        #region 查询产品系列对应的记录集

            private string _SearchProdList = @"
                                                   SELECT * FROM dbo.ColorCodeContrast WHERE TypeId='{0}'
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
        /// 根据产品系列下拉框所选择的条件。获得对应的记录集
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public DataTable SearchProductList(int pid)
        {
            var ds=new DataSet();

            try
            {
                using (var sql=new SqlConnection(conn.GetConnectionString()))
                {
                    var sqlDataAdapter=new SqlDataAdapter(string.Format(_SearchProdList,pid),sql);
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
