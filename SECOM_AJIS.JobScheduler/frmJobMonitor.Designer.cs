namespace SECOM_AJIS.JobScheduler
{
    partial class frmJobMonitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJobMonitor));
            this.timLoadBatchQueue = new System.Windows.Forms.Timer(this.components);
            this.dgvJobQueue = new System.Windows.Forms.DataGridView();
            this.runIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scheduleIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.batchCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.batchNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nextRunDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remarkDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsBatchQueue = new System.Windows.Forms.BindingSource(this.components);
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblLastLoad = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.miMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobQueue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBatchQueue)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timLoadBatchQueue
            // 
            this.timLoadBatchQueue.Tick += new System.EventHandler(this.timLoadBatchQueue_Tick);
            // 
            // dgvJobQueue
            // 
            this.dgvJobQueue.AllowUserToAddRows = false;
            this.dgvJobQueue.AllowUserToDeleteRows = false;
            this.dgvJobQueue.AutoGenerateColumns = false;
            this.dgvJobQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvJobQueue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.runIdDataGridViewTextBoxColumn,
            this.scheduleIdDataGridViewTextBoxColumn,
            this.batchCodeDataGridViewTextBoxColumn,
            this.batchNameDataGridViewTextBoxColumn,
            this.nextRunDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.remarkDataGridViewTextBoxColumn,
            this.startTimeDataGridViewTextBoxColumn,
            this.endTimeDataGridViewTextBoxColumn,
            this.lastUpdateDataGridViewTextBoxColumn});
            this.dgvJobQueue.DataSource = this.bsBatchQueue;
            this.dgvJobQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvJobQueue.Location = new System.Drawing.Point(0, 24);
            this.dgvJobQueue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvJobQueue.Name = "dgvJobQueue";
            this.dgvJobQueue.ReadOnly = true;
            this.dgvJobQueue.Size = new System.Drawing.Size(1087, 399);
            this.dgvJobQueue.TabIndex = 0;
            // 
            // runIdDataGridViewTextBoxColumn
            // 
            this.runIdDataGridViewTextBoxColumn.DataPropertyName = "RunId";
            this.runIdDataGridViewTextBoxColumn.HeaderText = "RunId";
            this.runIdDataGridViewTextBoxColumn.Name = "runIdDataGridViewTextBoxColumn";
            this.runIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.runIdDataGridViewTextBoxColumn.Width = 50;
            // 
            // scheduleIdDataGridViewTextBoxColumn
            // 
            this.scheduleIdDataGridViewTextBoxColumn.DataPropertyName = "ScheduleId";
            this.scheduleIdDataGridViewTextBoxColumn.HeaderText = "ScheduleId";
            this.scheduleIdDataGridViewTextBoxColumn.Name = "scheduleIdDataGridViewTextBoxColumn";
            this.scheduleIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.scheduleIdDataGridViewTextBoxColumn.Width = 70;
            // 
            // batchCodeDataGridViewTextBoxColumn
            // 
            this.batchCodeDataGridViewTextBoxColumn.DataPropertyName = "BatchCode";
            this.batchCodeDataGridViewTextBoxColumn.HeaderText = "BatchCode";
            this.batchCodeDataGridViewTextBoxColumn.Name = "batchCodeDataGridViewTextBoxColumn";
            this.batchCodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // batchNameDataGridViewTextBoxColumn
            // 
            this.batchNameDataGridViewTextBoxColumn.DataPropertyName = "BatchName";
            this.batchNameDataGridViewTextBoxColumn.HeaderText = "BatchName";
            this.batchNameDataGridViewTextBoxColumn.Name = "batchNameDataGridViewTextBoxColumn";
            this.batchNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.batchNameDataGridViewTextBoxColumn.Width = 120;
            // 
            // nextRunDataGridViewTextBoxColumn
            // 
            this.nextRunDataGridViewTextBoxColumn.DataPropertyName = "NextRun";
            this.nextRunDataGridViewTextBoxColumn.HeaderText = "NextRun";
            this.nextRunDataGridViewTextBoxColumn.Name = "nextRunDataGridViewTextBoxColumn";
            this.nextRunDataGridViewTextBoxColumn.ReadOnly = true;
            this.nextRunDataGridViewTextBoxColumn.Width = 120;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Width = 50;
            // 
            // remarkDataGridViewTextBoxColumn
            // 
            this.remarkDataGridViewTextBoxColumn.DataPropertyName = "Remark";
            this.remarkDataGridViewTextBoxColumn.HeaderText = "Remark";
            this.remarkDataGridViewTextBoxColumn.Name = "remarkDataGridViewTextBoxColumn";
            this.remarkDataGridViewTextBoxColumn.ReadOnly = true;
            this.remarkDataGridViewTextBoxColumn.Width = 150;
            // 
            // startTimeDataGridViewTextBoxColumn
            // 
            this.startTimeDataGridViewTextBoxColumn.DataPropertyName = "StartTime";
            this.startTimeDataGridViewTextBoxColumn.HeaderText = "StartTime";
            this.startTimeDataGridViewTextBoxColumn.Name = "startTimeDataGridViewTextBoxColumn";
            this.startTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.startTimeDataGridViewTextBoxColumn.Width = 120;
            // 
            // endTimeDataGridViewTextBoxColumn
            // 
            this.endTimeDataGridViewTextBoxColumn.DataPropertyName = "EndTime";
            this.endTimeDataGridViewTextBoxColumn.HeaderText = "EndTime";
            this.endTimeDataGridViewTextBoxColumn.Name = "endTimeDataGridViewTextBoxColumn";
            this.endTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.endTimeDataGridViewTextBoxColumn.Width = 120;
            // 
            // lastUpdateDataGridViewTextBoxColumn
            // 
            this.lastUpdateDataGridViewTextBoxColumn.DataPropertyName = "LastUpdate";
            this.lastUpdateDataGridViewTextBoxColumn.HeaderText = "LastUpdate";
            this.lastUpdateDataGridViewTextBoxColumn.Name = "lastUpdateDataGridViewTextBoxColumn";
            this.lastUpdateDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastUpdateDataGridViewTextBoxColumn.Width = 120;
            // 
            // bsBatchQueue
            // 
            this.bsBatchQueue.DataSource = typeof(SECOM_AJIS.DataEntity.Common.tbs_BatchQueue);
            // 
            // pnlFooter
            // 
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.lblLastLoad);
            this.pnlFooter.Controls.Add(this.lblVersion);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 423);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1087, 24);
            this.pnlFooter.TabIndex = 1;
            // 
            // lblLastLoad
            // 
            this.lblLastLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastLoad.Location = new System.Drawing.Point(791, 5);
            this.lblLastLoad.Name = "lblLastLoad";
            this.lblLastLoad.Size = new System.Drawing.Size(282, 14);
            this.lblLastLoad.TabIndex = 1;
            this.lblLastLoad.Text = "Last Queue Loaded: 99/99/9999 99:99:99 PM";
            this.lblLastLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 5);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(246, 14);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "SECOM-SIMS Job Scheduler Version X.X.X.X";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1087, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // miMenu
            // 
            this.miMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExit});
            this.miMenu.Name = "miMenu";
            this.miMenu.Size = new System.Drawing.Size(50, 20);
            this.miMenu.Text = "&Menu";
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(92, 22);
            this.miExit.Text = "E&xit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // frmJobMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 447);
            this.Controls.Add(this.dgvJobQueue);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmJobMonitor";
            this.Text = "Job Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmJobMonitor_FormClosing);
            this.Load += new System.EventHandler(this.frmJobMonitor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobQueue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBatchQueue)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timLoadBatchQueue;
        private System.Windows.Forms.DataGridView dgvJobQueue;
        private System.Windows.Forms.BindingSource bsBatchQueue;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblLastLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn runIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn scheduleIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn batchCodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn batchNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nextRunDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remarkDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdateDataGridViewTextBoxColumn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miMenu;
        private System.Windows.Forms.ToolStripMenuItem miExit;

    }
}

