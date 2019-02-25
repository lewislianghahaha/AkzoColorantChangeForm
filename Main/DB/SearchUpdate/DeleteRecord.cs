using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.DB.SearchUpdate
{
    public class DeleteRecord
    {
        Conn conn = new Conn();

        #region 清空记录

        private string _del = @"
                                    DELETE FROM dbo.AkzoFormula WHERE MacAdd='{0}' AND ImportDt='{1}'
                                    DELETE FROM dbo.AkzoFormulaEntry WHERE MacAdd='{0}' AND ImportDt='{1}'
                                    DELETE FROM dbo.ColorCodeContrast WHERE MacAdd='{0}' AND ImportDt='{1}'
                                ";

        #endregion

        /// <summary>
        /// 根据MAC地址以及当天日期对记录进行清空
        /// </summary>
        /// <param name="macadd"></param>
        public void DelRecord(string macadd)
        {
            try
            {
                using (var sql = new SqlConnection(conn.GetConnectionString()))
                {
                    sql.Open();
                    var com=new SqlCommand(string.Format(_del,macadd,DateTime.Now.Date),sql);
                    com.ExecuteNonQuery();
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
