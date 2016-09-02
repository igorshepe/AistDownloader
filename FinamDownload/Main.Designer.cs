using System.ComponentModel;
using System.Windows.Forms;

namespace FinamDownloader
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxYesterday = new System.Windows.Forms.CheckBox();
            this.checkBoxMergeFiles = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxDateFromTxt = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonTXTDir = new System.Windows.Forms.Button();
            this.textBoxTXTDir = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxPeriod = new System.Windows.Forms.ComboBox();
            this.comboBoxTimeCandle = new System.Windows.Forms.ComboBox();
            this.checkBoxFileheaderRow = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSplitChar = new System.Windows.Forms.ComboBox();
            this.labelSplitChar = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxUrlSecurity = new System.Windows.Forms.TextBox();
            this.buttonAddUrlSecurity = new System.Windows.Forms.Button();
            this.treeViewSecurity = new System.Windows.Forms.TreeView();
            this.buttonDownloadTxt = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonCancelDownload = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(421, 147);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxYesterday);
            this.tabPage1.Controls.Add(this.checkBoxMergeFiles);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.checkBoxDateFromTxt);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.dateTimePickerTo);
            this.tabPage1.Controls.Add(this.dateTimePickerFrom);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.buttonTXTDir);
            this.tabPage1.Controls.Add(this.textBoxTXTDir);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(413, 121);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "System";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxYesterday
            // 
            this.checkBoxYesterday.AutoSize = true;
            this.checkBoxYesterday.Location = new System.Drawing.Point(257, 64);
            this.checkBoxYesterday.Name = "checkBoxYesterday";
            this.checkBoxYesterday.Size = new System.Drawing.Size(73, 17);
            this.checkBoxYesterday.TabIndex = 23;
            this.checkBoxYesterday.Text = "Yesterday";
            this.checkBoxYesterday.UseVisualStyleBackColor = true;
            this.checkBoxYesterday.CheckedChanged += new System.EventHandler(this.checkBoxYesterday_CheckedChanged);
            // 
            // checkBoxMergeFiles
            // 
            this.checkBoxMergeFiles.AutoSize = true;
            this.checkBoxMergeFiles.Location = new System.Drawing.Point(96, 93);
            this.checkBoxMergeFiles.Name = "checkBoxMergeFiles";
            this.checkBoxMergeFiles.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMergeFiles.TabIndex = 22;
            this.checkBoxMergeFiles.UseVisualStyleBackColor = true;
            this.checkBoxMergeFiles.CheckedChanged += new System.EventHandler(this.checkBoxMergeFiles_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Merge files";
            // 
            // checkBoxDateFromTxt
            // 
            this.checkBoxDateFromTxt.AutoSize = true;
            this.checkBoxDateFromTxt.Location = new System.Drawing.Point(257, 37);
            this.checkBoxDateFromTxt.Name = "checkBoxDateFromTxt";
            this.checkBoxDateFromTxt.Size = new System.Drawing.Size(70, 17);
            this.checkBoxDateFromTxt.TabIndex = 20;
            this.checkBoxDateFromTxt.Text = "FromTXT";
            this.checkBoxDateFromTxt.UseVisualStyleBackColor = true;
            this.checkBoxDateFromTxt.CheckStateChanged += new System.EventHandler(this.checkBoxDateFromTxt_CheckStateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "To";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "From";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(97, 62);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(153, 20);
            this.dateTimePickerTo.TabIndex = 17;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(97, 35);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(153, 20);
            this.dateTimePickerFrom.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Folder history";
            // 
            // buttonTXTDir
            // 
            this.buttonTXTDir.Location = new System.Drawing.Point(365, 6);
            this.buttonTXTDir.Name = "buttonTXTDir";
            this.buttonTXTDir.Size = new System.Drawing.Size(32, 23);
            this.buttonTXTDir.TabIndex = 12;
            this.buttonTXTDir.Text = "...";
            this.buttonTXTDir.UseVisualStyleBackColor = true;
            this.buttonTXTDir.Click += new System.EventHandler(this.buttonTXTDir_Click);
            // 
            // textBoxTXTDir
            // 
            this.textBoxTXTDir.Location = new System.Drawing.Point(97, 8);
            this.textBoxTXTDir.Name = "textBoxTXTDir";
            this.textBoxTXTDir.Size = new System.Drawing.Size(262, 20);
            this.textBoxTXTDir.TabIndex = 11;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.comboBoxPeriod);
            this.tabPage2.Controls.Add(this.comboBoxTimeCandle);
            this.tabPage2.Controls.Add(this.checkBoxFileheaderRow);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.comboBoxSplitChar);
            this.tabPage2.Controls.Add(this.labelSplitChar);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(413, 121);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Finam";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Period";
            // 
            // comboBoxPeriod
            // 
            this.comboBoxPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPeriod.FormattingEnabled = true;
            this.comboBoxPeriod.Location = new System.Drawing.Point(88, 64);
            this.comboBoxPeriod.Name = "comboBoxPeriod";
            this.comboBoxPeriod.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPeriod.TabIndex = 16;
            // 
            // comboBoxTimeCandle
            // 
            this.comboBoxTimeCandle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimeCandle.Location = new System.Drawing.Point(88, 10);
            this.comboBoxTimeCandle.Name = "comboBoxTimeCandle";
            this.comboBoxTimeCandle.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTimeCandle.TabIndex = 14;
            // 
            // checkBoxFileheaderRow
            // 
            this.checkBoxFileheaderRow.AutoSize = true;
            this.checkBoxFileheaderRow.Location = new System.Drawing.Point(88, 96);
            this.checkBoxFileheaderRow.Name = "checkBoxFileheaderRow";
            this.checkBoxFileheaderRow.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFileheaderRow.TabIndex = 13;
            this.checkBoxFileheaderRow.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Fileheader row";
            // 
            // comboBoxSplitChar
            // 
            this.comboBoxSplitChar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSplitChar.FormattingEnabled = true;
            this.comboBoxSplitChar.Location = new System.Drawing.Point(88, 37);
            this.comboBoxSplitChar.Name = "comboBoxSplitChar";
            this.comboBoxSplitChar.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSplitChar.TabIndex = 11;
            // 
            // labelSplitChar
            // 
            this.labelSplitChar.AutoSize = true;
            this.labelSplitChar.Location = new System.Drawing.Point(6, 40);
            this.labelSplitChar.Name = "labelSplitChar";
            this.labelSplitChar.Size = new System.Drawing.Size(51, 13);
            this.labelSplitChar.TabIndex = 10;
            this.labelSplitChar.Text = "Split char";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Time candle";
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Location = new System.Drawing.Point(342, 269);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(80, 23);
            this.buttonSaveSettings.TabIndex = 21;
            this.buttonSaveSettings.Text = "Save settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.buttonSaveSettings);
            this.groupBox1.Controls.Add(this.textBoxUrlSecurity);
            this.groupBox1.Controls.Add(this.buttonAddUrlSecurity);
            this.groupBox1.Controls.Add(this.treeViewSecurity);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 299);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // textBoxUrlSecurity
            // 
            this.textBoxUrlSecurity.Location = new System.Drawing.Point(5, 269);
            this.textBoxUrlSecurity.Name = "textBoxUrlSecurity";
            this.textBoxUrlSecurity.Size = new System.Drawing.Size(100, 20);
            this.textBoxUrlSecurity.TabIndex = 23;
            this.textBoxUrlSecurity.Text = "http://www.finam.ru/profile/mosbirzha-fyuchersy/si/";
            this.textBoxUrlSecurity.Visible = false;
            // 
            // buttonAddUrlSecurity
            // 
            this.buttonAddUrlSecurity.Location = new System.Drawing.Point(209, 269);
            this.buttonAddUrlSecurity.Name = "buttonAddUrlSecurity";
            this.buttonAddUrlSecurity.Size = new System.Drawing.Size(75, 23);
            this.buttonAddUrlSecurity.TabIndex = 24;
            this.buttonAddUrlSecurity.Text = "Add security";
            this.buttonAddUrlSecurity.UseVisualStyleBackColor = true;
            this.buttonAddUrlSecurity.Visible = false;
            this.buttonAddUrlSecurity.Click += new System.EventHandler(this.buttonAddUrlSecurity_Click);
            // 
            // treeViewSecurity
            // 
            this.treeViewSecurity.CheckBoxes = true;
            this.treeViewSecurity.Location = new System.Drawing.Point(6, 173);
            this.treeViewSecurity.Name = "treeViewSecurity";
            this.treeViewSecurity.Size = new System.Drawing.Size(417, 90);
            this.treeViewSecurity.TabIndex = 26;
            this.treeViewSecurity.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSecurity_AfterCheck);
            // 
            // buttonDownloadTxt
            // 
            this.buttonDownloadTxt.Location = new System.Drawing.Point(343, 16);
            this.buttonDownloadTxt.Name = "buttonDownloadTxt";
            this.buttonDownloadTxt.Size = new System.Drawing.Size(80, 23);
            this.buttonDownloadTxt.TabIndex = 27;
            this.buttonDownloadTxt.Text = "Download";
            this.buttonDownloadTxt.UseVisualStyleBackColor = true;
            this.buttonDownloadTxt.Click += new System.EventHandler(this.buttonDownloadTxt_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(6, 45);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(417, 122);
            this.textBoxLog.TabIndex = 28;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 173);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(419, 23);
            this.progressBar1.TabIndex = 29;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // buttonCancelDownload
            // 
            this.buttonCancelDownload.Location = new System.Drawing.Point(262, 16);
            this.buttonCancelDownload.Name = "buttonCancelDownload";
            this.buttonCancelDownload.Size = new System.Drawing.Size(75, 23);
            this.buttonCancelDownload.TabIndex = 30;
            this.buttonCancelDownload.Text = "Cancel";
            this.buttonCancelDownload.UseVisualStyleBackColor = true;
            this.buttonCancelDownload.Click += new System.EventHandler(this.buttonCancelDownload_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxLog);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.buttonCancelDownload);
            this.groupBox2.Controls.Add(this.buttonDownloadTxt);
            this.groupBox2.Location = new System.Drawing.Point(12, 317);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(430, 202);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 527);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(472, 566);
            this.MinimumSize = new System.Drawing.Size(472, 566);
            this.Name = "Main";
            this.Text = "Download quotes from Finam";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Label label4;
        private Label label3;
        private DateTimePicker dateTimePickerTo;
        private DateTimePicker dateTimePickerFrom;
        private Label label1;
        private Button buttonTXTDir;
        private TextBox textBoxTXTDir;
        private TabPage tabPage2;
        private ComboBox comboBoxTimeCandle;
        private CheckBox checkBoxFileheaderRow;
        private Label label5;
        private ComboBox comboBoxSplitChar;
        private Label labelSplitChar;
        private Label label6;
        private Label label2;
        private ComboBox comboBoxPeriod;
        private Button buttonSaveSettings;
        private GroupBox groupBox1;
        private TextBox textBoxUrlSecurity;
        private Button buttonAddUrlSecurity;
        private TreeView treeViewSecurity;
        private Button buttonDownloadTxt;
        private TextBox textBoxLog;
        private CheckBox checkBoxMergeFiles;
        private Label label7;
        private CheckBox checkBoxYesterday;
        public BackgroundWorker backgroundWorker1;
        public ProgressBar progressBar1;
        public CheckBox checkBoxDateFromTxt;
        private GroupBox groupBox2;
        public Button buttonCancelDownload;
    }
}

