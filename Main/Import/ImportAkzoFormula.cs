using System;
using System.IO;
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

            //使用子线程工作(作用:通过调用子线程进行控制LoadForm窗体的关闭情况)
            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.RestulTable.Rows.Count==0)throw new Exception("不能成功导入,请检查导入模板是否有误.");
            MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            gvdtl.DataSource =task.RestulTable;
        }

        /// <summary>
        /// 导入功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmImport_Click(object sender, EventArgs e)
        {
            
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
    }
}
