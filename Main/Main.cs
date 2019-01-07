using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Main.DB;
using Main.Import;
using Main.Search;

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
                task.TaskId = 5;
                task.Factory = factoryName;
                task.pid = pid;

                //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (task.RestulTable.Rows.Count == 0) throw new Exception("运算不能成功,请联系管理人员.");
                //MessageBox.Show("运算成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gvdtl.DataSource = task.RestulTable;
                lblCount.Text = "查询的记录数为:"+ gvdtl.Rows.Count +"行";
                //若GridView中有某行的"浓度转换系数"为0的话。就将该行转为红色
                if (gvdtl.Rows.Count != 0) ChangeGrildColor((DataTable)gvdtl.DataSource);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择制造商下拉列表再继续", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入-Akzo配方表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmImportAkzo_Click(object sender, EventArgs e)
        {
            try
            {
                var importAkzoFormula = new ImportAkzoFormula();
                importAkzoFormula.StartPosition = FormStartPosition.CenterScreen;
                importAkzoFormula.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入-AKZO与雅图色母对照表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmIAkzoContrast_Click(object sender, EventArgs e)
        {
            try
            {
                var importAkzoColorant = new ImportAkzoColorant();
                importAkzoColorant.StartPosition = FormStartPosition.CenterParent;
                importAkzoColorant.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                var saveFileDialog = new SaveFileDialog { Filter = "Xlsx文件|*.xlsx" };
                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                var fileAdd = saveFileDialog.FileName;

                //将所需的值赋到Task类内
                task.TaskId = 6;
                task.FileAddress = fileAdd;
                task.ImporTable = (DataTable) gvdtl.DataSource;

                //使用子线程工作(作用:通过调用子线程进行控制Load窗体的关闭情况)
                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                var result = task.ImportResult;
                switch (result)
                {
                    case "0":
                        MessageBox.Show("导出成功!可从EXCEL中查阅导出效果", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    default:
                        throw (new Exception(result));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //清空原来DataGridView内的内容(无论成功与否都会执行)
            if (gvdtl.Rows.Count != 0) ClearDt((DataTable)gvdtl.DataSource);
            //将相关记录清空
            lblCount.Text = "";
        }

        /// <summary>
        /// 查询色母对照表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSearchColorant_Click(object sender, EventArgs e)
        {
            try
            {
                var search=new SearchForm();
                search.StartPosition=FormStartPosition.CenterScreen;
                search.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        /// 对GRIDVIEW的行改变颜色(注:若浓度系数为0的话,就变红色)
        /// </summary>
        /// <param name="dt"></param>
        private void ChangeGrildColor(DataTable dt)
        {
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var num = Convert.ToDecimal(dt.Rows[i][5]);
                if (num != 0) continue;
                gvdtl.Rows[i].Cells[0].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[1].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[2].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[3].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[4].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[5].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[6].Style.BackColor = Color.Red;
                gvdtl.Rows[i].Cells[7].Style.BackColor = Color.Red;
            }
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
    }
}
