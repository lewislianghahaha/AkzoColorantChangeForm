using System;
using System.Data;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Main.DB;

namespace Main.Search
{
    public partial class SearchForm : Form
    {
        Task task=new Task();
        Load load=new Load();

        public SearchForm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnShowTypeList();
        }

        private void OnRegisterEvents()
        {
            btnSearch.Click += BtnSearch_Click;
            tmClose.Click += TmClose_Click;
        }

        /// <summary>
        /// 产品系列下拉列表
        /// </summary>
        private void OnShowTypeList()
        {
            var dt = new DataTable();

            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }

            //创建行内容
            for (var j = 0; j < 2; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "1";
                        dr[1] = "1K BaseCoat";
                        break;
                    case 1:
                        dr[0] = "2";
                        dr[1] = "2K TopCoat";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comList.DataSource = dt;
            comList.DisplayMember = "Name"; //设置显示值
            comList.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, System.EventArgs e)
        {
            try
            {
                //获取所选择的“产品系列”下拉列表值
                var dvProductlist = (DataRowView)comList.Items[comList.SelectedIndex];
                var pid = Convert.ToInt32(dvProductlist["Id"]);

                //将所需的值赋到Task类内
                task.TaskId = 4;
                task.pid = pid;
                task.MacAdd = GetMacAddress(); //获取MAC地址
                task.Dt = dtp.Value.Date; //获取指定的导入日期

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
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关闭功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmClose_Click(object sender, System.EventArgs e)
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
