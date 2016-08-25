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

        private string[] arg;
        readonly Props _props = new Props();
        public List<SecurityInfo> Security = new List<SecurityInfo>();
        public List<SettingsMain> SettingsData = new List<SettingsMain>(); 
        private bool _firststart = true;
        public int cancelAsync = 0; // 1 нет файлов для объединения 2 отмена кнопкой

        public Main(string[] args)
        {
            arg = args;
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
                 "30 min","1 hour",
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
            if (arg.Length > 0)
            {
                if (arg[0] == "autoloading")
                {
                    AutoLoading();
                }
            }
           
           //AutoLoading();

        }


        public void AutoLoading()
        {
            cancelAsync = 3;
            checkBoxMergeFiles.Checked = true;
            checkBoxYesterday.Checked = true;
            backgroundWorker1.RunWorkerAsync();
        }

        public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
       {
            //TODO: Разобраться с доступом к обьектам из другого потока и с прогрессбаром из другого потока

           List<SettingsMain>  SettingsData = GetSettings();

            backgroundWorker1.ReportProgress(10);

            if (backgroundWorker1.CancellationPending)
                return;

            if (checkBoxDateFromTxt.Checked)
            {
                var fileData = LoadTxtFile();if (backgroundWorker1.CancellationPending)
                    return;
                backgroundWorker1.ReportProgress(20);
                FinamLoading.Download(Security, fileData, SettingsData);
                backgroundWorker1.ReportProgress(50);

            }
            else
            {
                backgroundWorker1.ReportProgress(20);
                var fileData = new List<FileSecurity>();
                FinamLoading.Download(Security, fileData,  SettingsData);
                backgroundWorker1.ReportProgress(50);
            }
           backgroundWorker1.ReportProgress(100);

        }
       public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
       {
            

            progressBar1.Value = e.ProgressPercentage;
        }
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (cancelAsync == 1)
                MessageBox.Show(this, @"There are no files to merge");
            else if (cancelAsync == 2)
                MessageBox.Show(this, @"Cancel");
            else if (cancelAsync == 3)
                Application.Exit();
            else 
                MessageBox.Show(this, @"Download complite");

            cancelAsync = 0;
            progressBar1.Value = 0;}
        
        private void buttonCancelDownload_Click(object sender, EventArgs e)
        {
            cancelAsync = 2;
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

        public void SaveToFile(string data, SecurityInfo security, SettingsMain settingsData)
        {
            if (backgroundWorker1.CancellationPending)
                return;

            var settings = settingsData;
            

            Directory.CreateDirectory(textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem);

            string filename = textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem + @"\" + security.Name + @"-" + settings.DateTo.Day + @"." + settings.DateTo.Month + @"." + settings.DateTo.Year + @"-" + comboBoxPeriod.SelectedItem + @".txt";

            if (filename != "")
            {
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    sw.Write(data);
                    sw.Close();

                }
            }

        }

        public void ChangeFile(string data, FileSecurity fileSec, SettingsMain settingsData)
        {
            if (backgroundWorker1.CancellationPending)
                return;
            var settings2 = settingsData;
            
            DateTime datatrue = fileSec.Dat.AddDays(-1); // для устранения лишнего дня в имени файла

            string filename = textBoxTXTDir.Text + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" + datatrue.Day + @"." + datatrue.Month + @"." + datatrue.Year + @"-" + comboBoxPeriod.SelectedItem + @".txt";

            string newfilename = textBoxTXTDir.Text + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" +
                                 settings2.DateTo.Day + @"." + settings2.DateTo.Month + @"." + settings2.DateTo.Year + @"-" +
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

       
        public List<FileSecurity> LoadTxtFile()
        {
             
            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(textBoxTXTDir.Text+@"\"+ SettingsData[0].PeriodItem); // папка с файлами 
            var filescount = dir.GetFiles();
             
            for (int i = 0; i < filescount.Length; i++)
            {
                Char delimiter = '-';
                String[] substrings = Path.GetFileNameWithoutExtension(filescount[i].FullName).Split(delimiter);
                
               fileHeader.Add(new FileSecurity() { Sec = substrings[0], Dat = DateTime.Parse(substrings[1]).AddDays(1), Per = substrings[2] });
                 
            }
            if (fileHeader.Count == 0)
            {
                cancelAsync = 1;
                backgroundWorker1.CancelAsync();

            }
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
            if (e.Node.Checked == true)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    e.Node.Nodes[i].Checked = true;
                    for (int j = 0; j < e.Node.Nodes[i].Nodes.Count; j++)
                    {
                        e.Node.Nodes[i].Nodes[j].Checked = true;
                        for (int k = 0; k < e.Node.Nodes[i].Nodes[j].Nodes.Count; k++)
                        {
                            e.Node.Nodes[i].Nodes[j].Nodes[k].Checked = true;
                        }
                    }
                }
            }

            if (e.Node.Checked == false)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    e.Node.Nodes[i].Checked = false;
                    for (int j = 0; j < e.Node.Nodes[i].Nodes.Count; j++)
                    {
                        e.Node.Nodes[i].Nodes[j].Checked = false;
                        for (int k = 0; k < e.Node.Nodes[i].Nodes[j].Nodes.Count; k++)
                        {
                            e.Node.Nodes[i].Nodes[j].Nodes[k].Checked = false;
                        }
                    }
                }
            }
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
                checkBoxDateFromTxt.Checked = true;
                checkBoxDateFromTxt.Enabled = false;

            }
            else
            {
                checkBoxFileheaderRow.Checked = _props.Fields.FileheaderRow;
                checkBoxFileheaderRow.Enabled = true;
                checkBoxDateFromTxt.Checked = _props.Fields.DateFromTxt;
                checkBoxDateFromTxt.Enabled = true;
            }
        }

        public List<SettingsMain> GetSettings()
        {
            SettingsData.Clear();
            SettingsData.Add(new SettingsMain
            {
                Period = comboBoxPeriod.SelectedIndex,
                PeriodItem = comboBoxPeriod.SelectedItem,
                DateFrom = dateTimePickerFrom.Value,
                DateTo = dateTimePickerTo.Value,
                SplitChar = comboBoxSplitChar.SelectedIndex+1,
                TimeCandle = comboBoxTimeCandle.SelectedIndex,
                DateFromeTxt = checkBoxDateFromTxt.Checked,
                FileheaderRow = checkBoxFileheaderRow.Checked,
                MergeFile = checkBoxMergeFiles.Checked

            });

            return SettingsData;
        } 
        public class SettingsMain
        {
            public int Period = 1;


            public object PeriodItem = Empty;

            public DateTime DateFrom = DateTime.Now;
            

            public DateTime DateTo = DateTime.Now;


            public int SplitChar = 1;


            public int TimeCandle = 1;


            public bool DateFromeTxt = false;



            public bool FileheaderRow = false;


            public bool MergeFile = false;

        }

        

        private void treeViewSecurity_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           }
    }
}