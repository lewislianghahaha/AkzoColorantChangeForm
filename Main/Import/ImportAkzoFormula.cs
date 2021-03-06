﻿using System;
using System.Data;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Main.DB;

namespace Main.Import
{
    public partial class ImportAkzoFormula : Form
    {
        Task task=new Task();
        Load load=new Load();

        public ImportAkzoFormula()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmOpenexcel.Click += TmOpenexcel_Click;
            tmImport.Click += TmImport_Click;
            tmClose.Click += TmClose_Click;
        }

        /// <summary>
        /// 打开EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmOpenexcel_Click(object sender, EventArgs e)
        {
            var openFileDialog=new OpenFileDialog {Filter = "Xlsx文件|*.xlsx"};
            if(openFileDialog.ShowDialog()!=DialogResult.OK)return;
            var fileAdd = openFileDialog.FileName;

            //将所需的值赋到Task类内
            task.TaskId = 1;
            task.FileAddress = fileAdd;
            task.Tablename = "AkzoFormula";

            //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.RestulTable.Rows.Count==0)throw new Exception("不能成功导入EXCEL内容,请检查模板是否正确.");
            //MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            gvdtl.DataSource =task.RestulTable;
        }

        /// <summary>
        /// 导入功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.Rows.Count == 0) throw new Exception("没有记录不能导入,请重新导入Excel记录");

                //将所需的值赋到Task类内
                task.TaskId = 2;
                task.Tablename = "AkzoFormula";
                task.ImporTable = (DataTable)gvdtl.DataSource;
                task.MacAdd = GetMacAddress();  //获取用户的MAC地址

                //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterParent;
                load.ShowDialog();

                var result = task.ImportResult;
                switch (result)
                {
                    case "0":
                        MessageBox.Show("导入成功!可执行运算功能", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    default:
                        throw (new Exception("导入有误,出现了重复项,请到主窗体通过查阅->Akzo配方表进行查阅,确定后再次进行导入"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //清空原来DataGridView内的内容(无论成功与否都会执行,若DataGridView内没有记录的话就不用执行)
            if(gvdtl.Rows.Count!=0) ClearDt((DataTable)gvdtl.DataSource);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() => {
                load.Close();
            }));
        }

        /// <summary>
        /// 清空DataTable
        /// </summary>
        private void ClearDt(DataTable dt)
        {
            try
            {
                dt.Rows.Clear();
                dt.Columns.Clear();
                gvdtl.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取MAC地址
        /// </summary>
        /// <returns></returns>
        private string GetMacAddress()
        {
            try
            {
                string strMac = string.Empty;
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();
                foreach (var mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        strMac = mo["MacAddress"].ToString();
                    }
                }
                moc = null;
                mc = null;
                return strMac;
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
