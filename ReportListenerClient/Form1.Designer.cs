namespace ReportListenerClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
         #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonHugoWriter = new System.Windows.Forms.RadioButton();
            this.radioButtonLogWriter = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageHugoData = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.checkBoxAutoRefresh = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonRawData = new System.Windows.Forms.RadioButton();
            this.radioButtonReports = new System.Windows.Forms.RadioButton();
            this.checkBoxConsolidate = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelReceived = new System.Windows.Forms.Label();
            this.labelSaved = new System.Windows.Forms.Label();
            this.labelErrors = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonRealTick = new System.Windows.Forms.RadioButton();
            this.radioButtonTWS = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageHugoData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPageLog.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(16, 12);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(138, 23);
            this.buttonLogIn.TabIndex = 0;
            this.buttonLogIn.Text = "Log In";
            this.buttonLogIn.UseVisualStyleBackColor = true;
            this.buttonLogIn.Click += new System.EventHandler(this.buttonLogIn_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(16, 39);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(138, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start Listener";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(16, 66);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(138, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop Listener";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonHugoWriter);
            this.groupBox1.Controls.Add(this.radioButtonLogWriter);
            this.groupBox1.Location = new System.Drawing.Point(304, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(110, 83);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Writer";
            // 
            // radioButtonHugoWriter
            // 
            this.radioButtonHugoWriter.AutoSize = true;
            this.radioButtonHugoWriter.Checked = true;
            this.radioButtonHugoWriter.Location = new System.Drawing.Point(28, 47);
            this.radioButtonHugoWriter.Name = "radioButtonHugoWriter";
            this.radioButtonHugoWriter.Size = new System.Drawing.Size(51, 17);
            this.radioButtonHugoWriter.TabIndex = 1;
            this.radioButtonHugoWriter.TabStop = true;
            this.radioButtonHugoWriter.Text = "Hugo";
            this.radioButtonHugoWriter.UseVisualStyleBackColor = true;
            this.radioButtonHugoWriter.Click += new System.EventHandler(this.radioButtonHugoWriter_Click);
            // 
            // radioButtonLogWriter
            // 
            this.radioButtonLogWriter.AutoSize = true;
            this.radioButtonLogWriter.Location = new System.Drawing.Point(28, 18);
            this.radioButtonLogWriter.Name = "radioButtonLogWriter";
            this.radioButtonLogWriter.Size = new System.Drawing.Size(43, 17);
            this.radioButtonLogWriter.TabIndex = 0;
            this.radioButtonLogWriter.Text = "Log";
            this.radioButtonLogWriter.UseVisualStyleBackColor = true;
            this.radioButtonLogWriter.Click += new System.EventHandler(this.radioButtonLogWriter_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageHugoData);
            this.tabControl1.Controls.Add(this.tabPageLog);
            this.tabControl1.Location = new System.Drawing.Point(12, 101);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1182, 553);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPageHugoData
            // 
            this.tabPageHugoData.Controls.Add(this.dataGridView1);
            this.tabPageHugoData.Location = new System.Drawing.Point(4, 22);
            this.tabPageHugoData.Name = "tabPageHugoData";
            this.tabPageHugoData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHugoData.Size = new System.Drawing.Size(1174, 527);
            this.tabPageHugoData.TabIndex = 0;
            this.tabPageHugoData.Text = "Reports";
            this.tabPageHugoData.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1178, 524);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.listBoxLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(1174, 527);
            this.tabPageLog.TabIndex = 1;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.Location = new System.Drawing.Point(3, 0);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(1171, 524);
            this.listBoxLog.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 657);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 5;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(672, 27);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.CloseUp += new System.EventHandler(this.dateTimePicker1_CloseUp);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(1052, 23);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(138, 23);
            this.buttonRefresh.TabIndex = 7;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // checkBoxAutoRefresh
            // 
            this.checkBoxAutoRefresh.AutoSize = true;
            this.checkBoxAutoRefresh.Checked = true;
            this.checkBoxAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoRefresh.Location = new System.Drawing.Point(1052, 59);
            this.checkBoxAutoRefresh.Name = "checkBoxAutoRefresh";
            this.checkBoxAutoRefresh.Size = new System.Drawing.Size(88, 17);
            this.checkBoxAutoRefresh.TabIndex = 8;
            this.checkBoxAutoRefresh.Text = "Auto Refresh";
            this.checkBoxAutoRefresh.UseVisualStyleBackColor = true;
            this.checkBoxAutoRefresh.CheckedChanged += new System.EventHandler(this.checkBoxAutoRefresh_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(601, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Import Date:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonRawData);
            this.groupBox2.Controls.Add(this.radioButtonReports);
            this.groupBox2.Location = new System.Drawing.Point(900, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(128, 83);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display";
            // 
            // radioButtonRawData
            // 
            this.radioButtonRawData.AutoSize = true;
            this.radioButtonRawData.Location = new System.Drawing.Point(25, 47);
            this.radioButtonRawData.Name = "radioButtonRawData";
            this.radioButtonRawData.Size = new System.Drawing.Size(73, 17);
            this.radioButtonRawData.TabIndex = 1;
            this.radioButtonRawData.Text = "Raw Data";
            this.radioButtonRawData.UseVisualStyleBackColor = true;
            this.radioButtonRawData.Click += new System.EventHandler(this.radioButtonDisplay_Click);
            // 
            // radioButtonReports
            // 
            this.radioButtonReports.AutoSize = true;
            this.radioButtonReports.Checked = true;
            this.radioButtonReports.Location = new System.Drawing.Point(25, 20);
            this.radioButtonReports.Name = "radioButtonReports";
            this.radioButtonReports.Size = new System.Drawing.Size(62, 17);
            this.radioButtonReports.TabIndex = 0;
            this.radioButtonReports.TabStop = true;
            this.radioButtonReports.Text = "Reports";
            this.radioButtonReports.UseVisualStyleBackColor = true;
            this.radioButtonReports.Click += new System.EventHandler(this.radioButtonDisplay_Click);
            // 
            // checkBoxConsolidate
            // 
            this.checkBoxConsolidate.AutoSize = true;
            this.checkBoxConsolidate.Location = new System.Drawing.Point(1052, 82);
            this.checkBoxConsolidate.Name = "checkBoxConsolidate";
            this.checkBoxConsolidate.Size = new System.Drawing.Size(81, 17);
            this.checkBoxConsolidate.TabIndex = 11;
            this.checkBoxConsolidate.Text = "Consolidate";
            this.checkBoxConsolidate.UseVisualStyleBackColor = true;
            this.checkBoxConsolidate.Click += new System.EventHandler(this.checkBoxConsolidate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(439, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Reports received:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(489, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Errors:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(451, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Reports saved:";
            // 
            // labelReceived
            // 
            this.labelReceived.Location = new System.Drawing.Point(530, 30);
            this.labelReceived.Name = "labelReceived";
            this.labelReceived.Size = new System.Drawing.Size(50, 13);
            this.labelReceived.TabIndex = 15;
            this.labelReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSaved
            // 
            this.labelSaved.Location = new System.Drawing.Point(530, 51);
            this.labelSaved.Name = "labelSaved";
            this.labelSaved.Size = new System.Drawing.Size(50, 13);
            this.labelSaved.TabIndex = 16;
            this.labelSaved.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelErrors
            // 
            this.labelErrors.Location = new System.Drawing.Point(530, 73);
            this.labelErrors.Name = "labelErrors";
            this.labelErrors.Size = new System.Drawing.Size(50, 13);
            this.labelErrors.TabIndex = 17;
            this.labelErrors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonTWS);
            this.groupBox3.Controls.Add(this.radioButtonRealTick);
            this.groupBox3.Location = new System.Drawing.Point(172, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(110, 83);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reader";
            // 
            // radioButtonRealTick
            // 
            this.radioButtonRealTick.AutoSize = true;
            this.radioButtonRealTick.Location = new System.Drawing.Point(18, 20);
            this.radioButtonRealTick.Name = "radioButtonRealTick";
            this.radioButtonRealTick.Size = new System.Drawing.Size(68, 17);
            this.radioButtonRealTick.TabIndex = 0;
            this.radioButtonRealTick.Text = "RealTick";
            this.radioButtonRealTick.UseVisualStyleBackColor = true;
            this.radioButtonRealTick.Click += new System.EventHandler(this.radioButtonRealTick_Click);
            // 
            // radioButtonTWS
            // 
            this.radioButtonTWS.AutoSize = true;
            this.radioButtonTWS.Checked = true;
            this.radioButtonTWS.Location = new System.Drawing.Point(18, 47);
            this.radioButtonTWS.Name = "radioButtonTWS";
            this.radioButtonTWS.Size = new System.Drawing.Size(50, 17);
            this.radioButtonTWS.TabIndex = 1;
            this.radioButtonTWS.TabStop = true;
            this.radioButtonTWS.Text = "TWS";
            this.radioButtonTWS.UseVisualStyleBackColor = true;
            this.radioButtonTWS.Click += new System.EventHandler(this.radioButtonTWS_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 688);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.labelErrors);
            this.Controls.Add(this.labelSaved);
            this.Controls.Add(this.labelReceived);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxConsolidate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxAutoRefresh);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonLogIn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageHugoData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPageLog.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLogIn;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonHugoWriter;
        private System.Windows.Forms.RadioButton radioButtonLogWriter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageHugoData;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.CheckBox checkBoxAutoRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonRawData;
        private System.Windows.Forms.RadioButton radioButtonReports;
        private System.Windows.Forms.CheckBox checkBoxConsolidate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelReceived;
        private System.Windows.Forms.Label labelSaved;
        private System.Windows.Forms.Label labelErrors;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonTWS;
        private System.Windows.Forms.RadioButton radioButtonRealTick;
    }
}

