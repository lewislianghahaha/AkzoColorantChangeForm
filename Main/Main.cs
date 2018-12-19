using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Main.DB;
using Main.Import;

namespace Main
{
    public partial class Main : Form
    {
        Task task=new Task();
        Load load=new Load();

        public Main()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnShowTypeList();
        }

        private void OnRegisterEvents()
        {
            tmImportAkzo.Click += TmImportAkzo_Click;
            tmIAkzoContrast.Click += TmIAkzoContrast_Click;
            tmExport.Click += TmExport_Click;
            tmSearchColorant.Click += TmSearchColorant_Click;
            tmClose.Click += TmClose_Click;
            btnGenerate.Click += BtnGenerate_Click;
            comFactory.Click += ComFactory_Click;
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

            comProductList.DataSource = dt;
            comProductList.DisplayMember = "Name"; //设置显示值
            comProductList.ValueMember = "Id";    //设置默认值内码
        }

        //制造商下拉列表(获取数据的"制造商"值并进行显示)
        private void ComFactory_Click(object sender, EventArgs e)
        {
            try
            {
                //将所需的值赋到Task类内
                task.TaskId = 3;
                task.StartTask();
                comFactory.DataSource = task.RestulTable;
                comFactory.DisplayMember = "Factory";     //设置显示值
                comFactory.ValueMember = "Factory";       //设置默认值内码
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 运算功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                //获取所选择的“制造商”下拉列表值
                var dvList = (DataRowView) comFactory.Items[comFactory.SelectedIndex];
                var factoryName = Convert.ToString(dvList["Factory"]);

                //获取所选择的“产品系列”下拉列表值
                var dvProductlist = (DataRowView)comProductList.Items[comProductList.SelectedIndex];
                var pid = Convert.ToInt32(dvProductlist["Id"]);

                //将所需的值赋到Task类内
                task.TaskId = 6;
                task.Factory = factoryName;
                task.pid = pid;

                //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (task.RestulTable.Rows.Count == 0) throw new Exception("运算不能成功,请联系管理人员.");
                MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gvdtl.DataSource = task.RestulTable;
                //若GridView中有某行的"浓度转换系数"为0的话。就将该行转为红色
                ChangeGrildColor((DataTable)gvdtl.DataSource);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入-Akzo配方表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmImportAkzo_Click(object sender, EventArgs e)
        {
            var importAkzoFormula=new ImportAkzoFormula();
            importAkzoFormula.StartPosition= FormStartPosition.CenterScreen;
            importAkzoFormula.ShowDialog();
        }

        /// <summary>
        /// 导入-AKZO与雅图色母对照表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmIAkzoContrast_Click(object sender, EventArgs e)
        {
            var importAkzoColorant=new ImportAkzoColorant();
            importAkzoColorant.StartPosition=FormStartPosition.CenterParent;
            importAkzoColorant.ShowDialog();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmExport_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.Rows.Count==0) throw new Exception("没有执行结果,不能执行导出操作");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查询及编辑色母对照表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSearchColorant_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
        /// 对GRIDVIEW的行改变颜色(注:若浓度系数为0的话)
        /// </summary>
        /// <param name="dt"></param>
        private void ChangeGrildColor(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
            }
            foreach (DataRow rows in dt.Rows)
            {

                gvdtl.Rows[0].Cells[1].Style.BackColor = Color.Red;
            }
        }

    }
}
