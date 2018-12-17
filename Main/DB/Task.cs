using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Main.DB.Import;

namespace Main.DB
{
    public class Task
    {
        ImportData import=new ImportData();


        private int _taskid;
        private string _fileAddress;
        private string _tablename;
        private DataTable _resultTable;
        private string _importResult = "0";


        /// <summary>
        /// 中转ID
        /// </summary>
        public int TaskId { set { _taskid = value; } }

        /// <summary>
        /// //接收文件地址信息
        /// </summary>
        public string FileAddress { set { _fileAddress = value; } }

        /// <summary>
        /// 接收指定的表名(Excel上传时使用)
        /// </summary>
        public string Tablename { set { _tablename = value; } }









        /// <summary>
        ///返回DataTable至主窗体
        /// </summary>
        public DataTable RestulTable => _resultTable;

        /// <summary>
        /// 返回导入数据库结果(作用:导入数据库后使用)
        /// </summary>
        public string ImportResult => _importResult;


        /// <summary>
        /// 作功能中转使用
        /// </summary>
        public void StartTask()
        {
            Thread.Sleep(1000);

            switch (_taskid)
            {
                //打开EXCEL并导入至DataTable功能
                case 1:
                    OpenExceltoDt(_fileAddress,_tablename);
                    break;
                //导入数据库功能
                case 2:

                    break;
                //查询色母对照表
                case 3:

                    break;
                //编辑色母对照表
                case 4:

                    break;
                //运算功能
                case 5:

                    break;
                //导出功能
                case 6:

                    break;
            }
        }

        /// <summary>
        /// 打开EXCEL并导入至DT
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <param name="tableName"></param>
        private void OpenExceltoDt(string fileAddress, string tableName)
        {
            _resultTable = import.OpenExceltoDt(fileAddress, tableName);
        }


    }
}
