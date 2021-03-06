﻿using System;
using System.Data;
using System.Management;
using System.Threading;
using Main.DB;
using System.Windows.Forms;

namespace Main.Search
{
    public partial class SearchFormulaForm : Form
    {
        Task task=new Task();
        Load load = new Load();

        public SearchFormulaForm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            comfactory.Click += Comfactory_Click;
            btnSearch.Click += BtnSearch_Click;
            tmClose.Click += TmClose_Click;
        }

        /// <summary>
        /// 制造商下拉列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comfactory_Click(object sender, EventArgs e)
        {
            try
            {
                //将所需的值赋到Task类内
                task.TaskId = 3;
                task.StartTask();
                comfactory.DataSource = task.RestulTable;
                comfactory.DisplayMember = "Factory";     //设置显示值
                comfactory.ValueMember = "Factory";       //设置默认值内码
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查询功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //获取所选择的“制造商”下拉列表值
                var dvfactorylist = (DataRowView)comfactory.Items[comfactory.SelectedIndex];
                var factory = Convert.ToString(dvfactorylist["Factory"]);

                //将所需的值赋到Task类内
                task.TaskId = 7;
                task.Factory = factory;
                task.MacAdd = GetMacAddress(); //获取MAC地址
                task.Dt = dtp.Value.Date;  //获取指定的导入日期

                //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (task.RestulTable.Rows.Count == 0) throw new Exception("查询不成功,请联系管理人员.");
                if (task.RestulTable.Rows.Count > 0)
                    gvdtl.DataSource = task.RestulTable;
                lblcount.Text = "查询的记录数为:" + gvdtl.Rows.Count + "行";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Contains("InvalidArgument") ? "请选择制造商下拉列表" : ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关闭功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() => {
                load.Close();
            }));
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
