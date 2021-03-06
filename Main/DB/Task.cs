﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Main.DB.Export;
using Main.DB.Generate;
using Main.DB.Import;
using Main.DB.SearchUpdate;

namespace Main.DB
{
    public class Task
    {
        ImportData import=new ImportData();
        SearchData searchData=new SearchData();
        GenerateRecord generate=new GenerateRecord();
        SearchData search=new SearchData();
        ExportData exportData=new ExportData();
        DeleteRecord del=new DeleteRecord();

        private int _taskid;
        private string _fileAddress;        //文件地址
        private string _tablename;          //数据表名
        private DataTable _resultTable;     //记录集DATATABLE（从EXCEL导入的DataTable）
        private DataTable _importTable;     //获取从DataGridView中的DataTable记录集
        private string _importResult = "0"; //返回是否成功的提示信息(注:导入或导出记录时使用)
        private string _factory;
        private int _pid;
        private string _MacAdd;
        private DateTime _importdt;

        /// <summary>
        /// 中转ID
        /// </summary>
        public int TaskId { set { _taskid = value; } }

        /// <summary>
        /// //接收文件地址信息
        /// </summary>
        public string FileAddress { set { _fileAddress = value; } }

        /// <summary>
        /// 接收指定的表名(Excel导入物理表时使用)
        /// </summary>
        public string Tablename { set { _tablename = value; } }
        /// <summary>
        /// 接收GridView内的DataTable
        /// </summary>
        public DataTable ImporTable { set { _importTable = value; } }

        /// <summary>
        /// 接收“制造商”下拉列表所选择的选项
        /// </summary>
        public string Factory {set { _factory = value; }}

        /// <summary>
        /// 接收“产品系列”下拉列表所选择的选项
        /// </summary>
        public int pid {set { _pid = value; }}

        /// <summary>
        /// 接收MAC地址
        /// </summary>
        public string MacAdd {set { _MacAdd = value; }}

        /// <summary>
        /// 接收导入的日期("查询"时使用)
        /// </summary>
        public DateTime Dt {set { _importdt = value; }}


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
                    ImportExcelToDb(_tablename,_importTable,_MacAdd);
                    break;
                //下拉列表数据获取(查询)“制造商”
                case 3:
                    SearchList();
                    break;
                //查询色母对照表(色母对照表时使用)
                case 4:
                    SearchProddt(_pid,_MacAdd,_importdt);
                    break;
                //运算功能
                case 5:
                    GenerateRecord(_factory,_pid,_MacAdd);
                    break;
                //导出功能
                case 6:
                    ExportDtToExcel(_fileAddress,_importTable,_MacAdd);
                    break;
                //查询AKZO配方(查询AKZO配方表使用)
                case 7:
                    SearchFormula(_factory,_MacAdd,_importdt);
                    break;
                //针对MAC地址以及当天日期进行清空记录
                case 8:
                    DelRecord(_MacAdd);
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

        /// <summary>
        /// 将DataTable的内容插入至数据库指定的表内
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="macadd"></param>
        private void ImportExcelToDb(string tableName,DataTable dt,string macadd)
        {
            _importResult = import.ImportExcelToDb(tableName,dt,macadd);
        }

        /// <summary>
        /// 制造商下拉列表
        /// </summary>
        private void SearchList()
        {
            _resultTable=searchData.SearchList();
        }

        /// <summary>
        /// 运算功能
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="pid"></param>
        private void GenerateRecord(string factory,int pid,string macadd)
        {
            _resultTable = generate.GetRecordToDataTable(factory,pid,macadd);
        }

        /// <summary>
        /// 查询色母对照表
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="macadd"></param>
        /// <param name="dtTime"></param>
        private void SearchProddt(int pid,string macadd,DateTime dtTime)
        {
            _resultTable = search.SearchProductList(pid,macadd,dtTime);
        }

        /// <summary>
        /// 将DATATABLE的数据导出至指定的EXCEL文件内
        /// </summary>
        private void ExportDtToExcel(string fileAddress, DataTable dt,string macadd)
        {
            _importResult = exportData.ExportDttoExcel(fileAddress, dt,macadd);
        }

        /// <summary>
        /// 查询AKZO配方记录明细表
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="macadd"></param>
        /// <param name="dtTime"></param>
        private void SearchFormula(string factory,string macadd,DateTime dtTime)
        {
            _resultTable = search.SearchdtlList(factory,macadd,dtTime);
        }

        private void DelRecord(string macadd)
        {
             del.DelRecord(macadd);
        }
    }
}
