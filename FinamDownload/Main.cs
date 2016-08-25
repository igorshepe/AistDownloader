using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.String;

namespace FinamDownloader
{
    public partial class Main : Form
    {
        readonly Props _props = new Props();
        public List<SecurityInfo> Security = new List<SecurityInfo>();

        private bool _firststart = true;
        public int LoadinProgressBar = 0;

        public Main()
        {
            
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;


            comboBoxPeriod.Items.AddRange(new object[]
            {
                 "tics",
                 "1 min",
                 "5 min",
                 "10 min",
                 "15 min",
                 "30 min",
                 "1 hour",
                 "1 day",
                 "1 weak",
                 "1 month"
            });
            comboBoxSplitChar.Items.AddRange(new object[]
            {
                 "запятая (,)",
                 "точка (.)",
                 "точка с запятой (;)",
                 "табуляция (>)",
                 "пробел ( )"

            });
            comboBoxTimeCandle.Items.AddRange(new object[]
            {
                 "Open time",
                 "Close time"


            });
            ReadSetting();
            
        }

       public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
       {
            //TODO: Разобраться с доступом к обьектам из другого потока и с прогрессбаром из другого потока

            backgroundWorker1.ReportProgress(10);

            if (backgroundWorker1.CancellationPending)
                return;

            if (checkBoxDateFromTxt.Checked)
            {
                var fileData = LoadTxtFile();
                //backgroundWorker1.ReportProgress(20);
                FinamLoading.Download(Security, fileData);
                //backgroundWorker1.ReportProgress(50);

            }
            else
            {
               // backgroundWorker1.ReportProgress(20);
                var fileData = new List<FileSecurity>();
                FinamLoading.Download(Security, fileData);
               // backgroundWorker1.ReportProgress(50);
            }
           backgroundWorker1.ReportProgress(100);

        }
       public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
       {
            

            progressBar1.Value = e.ProgressPercentage;
        }
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(this, @"Download complite");
        }
        
        private void buttonCancelDownload_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }


        private void WriteSetting()
        {
            _props.Fields.Folder = textBoxTXTDir.Text ;

            _props.Fields.TimeFrom = dateTimePickerFrom.Value;

            _props.Fields.TimeTo = dateTimePickerTo.Value ;

            _props.Fields.Period = comboBoxPeriod.SelectedIndex  ;

            _props.Fields.SplitChar = comboBoxSplitChar.SelectedIndex;

            _props.Fields.TimeCandle = comboBoxTimeCandle.SelectedIndex;

            _props.Fields.FileheaderRow = checkBoxFileheaderRow.Checked;

            _props.Fields.DateFromTxt =  checkBoxDateFromTxt.Checked ;

            _props.Fields.MergeFiles = checkBoxMergeFiles.Checked;

            _props.Fields.Yesterday = checkBoxYesterday.Checked;

            _props.Fields.Security = Security;

            _props.WriteXml();
        }
        private void ReadSetting()
        {
            _props.ReadXml();

            textBoxTXTDir.Text = _props.Fields.Folder;

            dateTimePickerFrom.Value = _props.Fields.TimeFrom;

            dateTimePickerTo.Value = _props.Fields.TimeTo;

            comboBoxPeriod.SelectedIndex = _props.Fields.Period;

            comboBoxSplitChar.SelectedIndex = _props.Fields.SplitChar;

            comboBoxTimeCandle.SelectedIndex = _props.Fields.TimeCandle;

            checkBoxFileheaderRow.Checked =  _props.Fields.FileheaderRow ;

            checkBoxDateFromTxt.Checked = _props.Fields.DateFromTxt;

            checkBoxMergeFiles.Checked = _props.Fields.MergeFiles;

            checkBoxYesterday.Checked =  _props.Fields.Yesterday;

            Security = _props.Fields.Security;
            
            LoadTreeview();
            
            _firststart = false; // для проверки изменения состояния чекбокса treeview
        }

         
        public int Period
        {
            get { return comboBoxPeriod.SelectedIndex; }
        }

        public object PeriodItem
        {
            get { return comboBoxPeriod.SelectedItem; }
        }
        public DateTime DateFrom
        {
            get { return dateTimePickerFrom.Value; }
        }

        public DateTime DateTo
        {
            get { return dateTimePickerTo.Value; }}

        public int SplitChar
        {
            get { return comboBoxSplitChar.SelectedIndex+1; }// нет значения 0
        }

        public int TimeCandle
        {
            get { return comboBoxTimeCandle.SelectedIndex; }
        }

        public CheckState DateFromeTxt
        {
            get { return checkBoxDateFromTxt.CheckState; }
        }
         

        public bool FileheaderRow
        {
            get { return checkBoxFileheaderRow.Checked; }
        }

        public bool MergeFile
        {
            get { return checkBoxMergeFiles.Checked; }
        }

        public int LoadingProgressBar
        {
            get { return LoadinProgressBar; }
            
        }
       
        private void buttonTXTDir_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
            if (DialogResult.OK != folderBrowserDialog1.ShowDialog())
                return;
            textBoxTXTDir.Text = folderBrowserDialog1.SelectedPath;
        }

         
        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            WriteSetting();
        }

        private void buttonAddUrlSecurity_Click(object sender, EventArgs e)
        {
          var dd =   FinamLoading.AddSecurity(textBoxUrlSecurity.Text);
        }

        private void buttonDownloadTxt_Click(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();


        }

        public void SaveToFile(string data, SecurityInfo security)
        {
            if (backgroundWorker1.CancellationPending)
                return;

            Directory.CreateDirectory(textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem);

            string filename = textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem + @"\" + security.Name + @"-" + DateTo.Day + @"." + DateTo.Month + @"." + DateTo.Year + @"-" + comboBoxPeriod.SelectedItem + @".txt";

            if (filename != "")
            {
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    sw.Write(data);
                    sw.Close();

                }
            }

        }

        public void ChangeFile(string data, FileSecurity fileSec)
        {
            string periodTheader = GetPeriodData();
            if (backgroundWorker1.CancellationPending)
                return;

            string filename = textBoxTXTDir.Text + @"\" + periodTheader + @"\" + fileSec.Sec + @"-" + fileSec.Dat.Day + @"." + fileSec.Dat.Month + @"." + fileSec.Dat.Year + @"-" + comboBoxPeriod.SelectedItem + @".txt";

            string newfilename = textBoxTXTDir.Text + @"\" + periodTheader + @"\" + fileSec.Sec + @"-" +
                                 DateTo.Day + @"." + DateTo.Month + @"." + DateTo.Year + @"-" +
                                 comboBoxPeriod.SelectedItem + @".txt";

            try
            {
                using (var destination = File.AppendText(filename))
                {

                    destination.Write(data); // записываем дату время и сотояние портфеля
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message);
                throw;
            }
            

            File.Move(filename,newfilename);
            }

        public string GetPeriodData()
        {
            string dd = Empty;
            if (comboBoxPeriod.InvokeRequired) comboBoxPeriod.Invoke(new Action<string>((s) => dd = comboBoxPeriod.SelectedItem.ToString()), comboBoxPeriod.SelectedItem.ToString());
            else dd = comboBoxPeriod.SelectedItem.ToString();
            return dd;
        }
        public List<FileSecurity> LoadTxtFile()
        {
            backgroundWorker1.ReportProgress(30);
            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(textBoxTXTDir.Text+@"\"+ PeriodItem); // папка с файлами 
            var filescount = dir.GetFiles();
             
            for (int i = 0; i < filescount.Length; i++)
            {
                Char delimiter = '-';
                String[] substrings = Path.GetFileNameWithoutExtension(filescount[i].FullName).Split(delimiter);
                
               fileHeader.Add(new FileSecurity() { Sec = substrings[0], Dat = DateTime.Parse(substrings[1]), Per = substrings[2] });
                 
            }
            backgroundWorker1.ReportProgress(30,fileHeader.Count);
            return fileHeader;
        }

        public class FileSecurity
        {
            public string Sec { get; set; }
            public DateTime Dat { get; set; }
            public string Per { get; set; }
        }

        private void LoadTreeview()
        {
            
            string checkname = Empty;
            for (int i = 0; i < Security.Count; i++)
            {
                if (checkname == "")
                {
                    checkname = Security[i].MarketName;
                    treeViewSecurity.Nodes.Add(Security[i].MarketName);
                   
                }
                else if (checkname == Security[i].MarketName)
                {
                    }
                else
                {
                    checkname = Security[i].MarketName;
                    treeViewSecurity.Nodes.Add(Security[i].MarketName);
                }
                
            }

            for (int i = 0; i < treeViewSecurity.Nodes.Count; i++)
            {
                for (int j = 0; j < Security.Count; j++)
                {
                    if (treeViewSecurity.Nodes[i].Text == Security[j].MarketName)
                    {
                        treeViewSecurity.Nodes[i].Nodes.Add(Security[j].Name).Checked = Security[j].Checed;
                         
                    }
                }
                
            }
        }

       private void treeViewSecurity_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!_firststart)
            {
                for (int i = 0; i < Security.Count; i++)
                {
                    
                    if (e.Node.Text == Security[i].Name)
                    {
                        Security[i].Checed = e.Node.Checked;
                        return;
                    }
                }
            }

        }

        private void checkBoxDateFromTxt_CheckStateChanged(object sender, EventArgs e)
        {
      dateTimePickerFrom.Enabled = !checkBoxDateFromTxt.Checked;
        }

        private void checkBoxYesterday_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxYesterday.Checked)
            {
                dateTimePickerTo.Value = DateTime.Today.AddDays(-1);
                dateTimePickerTo.Enabled = false;
            }
            else
            {
                dateTimePickerTo.Value = _props.Fields.TimeTo;
                dateTimePickerTo.Enabled = true;
            }
        }

        private void checkBoxMergeFiles_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxMergeFiles.Checked)
            {
                checkBoxFileheaderRow.Checked = false;
                checkBoxFileheaderRow.Enabled = false;

            }
            else{
                checkBoxFileheaderRow.Checked = true;
                checkBoxFileheaderRow.Enabled = true;
            }
        }

         
    }
}