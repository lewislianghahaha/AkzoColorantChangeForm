using System;
using System.Data;
using System.Windows.Forms;
using Main.Import;

namespace Main
{
    public partial class Main : Form
    {
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

        //制造商下拉列表
        private void ComFactory_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 运算功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 导入-Akzo配方表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmImportAkzo_Click(object sender, EventArgs e)
        {
            ImportAkzoFormula importAkzoFormula=new ImportAkzoFormula();
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
            
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmExport_Click(object sender, EventArgs e)
        {
             
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
    }
}
