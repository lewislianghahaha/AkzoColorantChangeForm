namespace Main
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.tm1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tmImportAkzo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmIAkzoContrast = new System.Windows.Forms.ToolStripMenuItem();
            this.tmExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tm3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tmSearchColorant = new System.Windows.Forms.ToolStripMenuItem();
            this.tmSearchFormula = new System.Windows.Forms.ToolStripMenuItem();
            this.tmClose = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.comProductList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comFactory = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gvdtl = new System.Windows.Forms.DataGridView();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tm1,
            this.tmExport,
            this.tm3,
            this.tmClose});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(972, 25);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // tm1
            // 
            this.tm1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmImportAkzo,
            this.toolStripSeparator1,
            this.tmIAkzoContrast});
            this.tm1.Name = "tm1";
            this.tm1.Size = new System.Drawing.Size(44, 21);
            this.tm1.Text = "导入";
            // 
            // tmImportAkzo
            // 
            this.tmImportAkzo.Name = "tmImportAkzo";
            this.tmImportAkzo.Size = new System.Drawing.Size(201, 22);
            this.tmImportAkzo.Text = "Akzo配方表";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // tmIAkzoContrast
            // 
            this.tmIAkzoContrast.Name = "tmIAkzoContrast";
            this.tmIAkzoContrast.Size = new System.Drawing.Size(201, 22);
            this.tmIAkzoContrast.Text = "Akzo与雅图色母对照表";
            // 
            // tmExport
            // 
            this.tmExport.Name = "tmExport";
            this.tmExport.Size = new System.Drawing.Size(44, 21);
            this.tmExport.Text = "导出";
            // 
            // tm3
            // 
            this.tm3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmSearchColorant,
            this.toolStripSeparator2,
            this.tmSearchFormula});
            this.tm3.Name = "tm3";
            this.tm3.Size = new System.Drawing.Size(44, 21);
            this.tm3.Text = "查阅";
            // 
            // tmSearchColorant
            // 
            this.tmSearchColorant.Name = "tmSearchColorant";
            this.tmSearchColorant.Size = new System.Drawing.Size(152, 22);
            this.tmSearchColorant.Text = "色母对照表";
            // 
            // tmSearchFormula
            // 
            this.tmSearchFormula.Name = "tmSearchFormula";
            this.tmSearchFormula.Size = new System.Drawing.Size(152, 22);
            this.tmSearchFormula.Text = "配方记录表";
            // 
            // tmClose
            // 
            this.tmClose.Name = "tmClose";
            this.tmClose.Size = new System.Drawing.Size(44, 21);
            this.tmClose.Text = "关闭";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGenerate);
            this.panel1.Controls.Add(this.comProductList);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comFactory);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(972, 41);
            this.panel1.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(272, 10);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "开始运算";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // comProductList
            // 
            this.comProductList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comProductList.FormattingEnabled = true;
            this.comProductList.Location = new System.Drawing.Point(76, 11);
            this.comProductList.Name = "comProductList";
            this.comProductList.Size = new System.Drawing.Size(121, 20);
            this.comProductList.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "产品系列";
            // 
            // comFactory
            // 
            this.comFactory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comFactory.FormattingEnabled = true;
            this.comFactory.Location = new System.Drawing.Point(839, 12);
            this.comFactory.Name = "comFactory";
            this.comFactory.Size = new System.Drawing.Size(121, 20);
            this.comFactory.TabIndex = 1;
            this.comFactory.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(793, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "制造商";
            this.label1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblCount);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 66);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(972, 25);
            this.panel2.TabIndex = 2;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(18, 6);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 12);
            this.lblCount.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gvdtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 91);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(972, 468);
            this.panel3.TabIndex = 3;
            // 
            // gvdtl
            // 
            this.gvdtl.AllowUserToAddRows = false;
            this.gvdtl.AllowUserToDeleteRows = false;
            this.gvdtl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvdtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvdtl.Location = new System.Drawing.Point(0, 0);
            this.gvdtl.Name = "gvdtl";
            this.gvdtl.ReadOnly = true;
            this.gvdtl.RowTemplate.Height = 23;
            this.gvdtl.Size = new System.Drawing.Size(972, 468);
            this.gvdtl.TabIndex = 0;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // Main
            // 
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 559);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.MainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "Main";
            this.Text = "色母转换查询功能";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvdtl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tm1;
        private System.Windows.Forms.ToolStripMenuItem tmImportAkzo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tmIAkzoContrast;
        private System.Windows.Forms.ToolStripMenuItem tmExport;
        private System.Windows.Forms.ToolStripMenuItem tm3;
        private System.Windows.Forms.ToolStripMenuItem tmSearchColorant;
        private System.Windows.Forms.ToolStripMenuItem tmClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comProductList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comFactory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView gvdtl;
        private System.Windows.Forms.ToolStripMenuItem tmSearchFormula;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

