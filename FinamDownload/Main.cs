using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FinamDownloader.Properties;
using log4net;
using log4net.Config;
using static System.String;

namespace FinamDownloader
{
    public partial class Main : Form
    {
        private static readonly ILog L = LogManager.GetLogger(typeof(Main));
        readonly Props _props = new Props();
        public List<SecurityInfo> Security = new List<SecurityInfo>();
        public List<SettingsMain> SettingsData = new List<SettingsMain>();
        private bool _firststart = true;
        public int CancelAsync; // 1 нет файлов для объединения 2 отмена кнопкой

        private readonly string _settingsFolder = Environment.CurrentDirectory + "\\settings.xml";
        public Main(string[] args)
        {
            var arg = args;
            InitializeComponent();

            XmlDocument objDocument = new XmlDocument();
            objDocument.LoadXml(Resources.log4net);
            XmlConfigurator.Configure(objDocument.DocumentElement);

            L.Debug("Приложение стартовало");

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
                    L.Info("Start with an attribute (autoloading)");
                    AutoLoading();
                }
            }


        }


        public void AutoLoading() // запуск с ключом autoloading
        {
            L.Debug("AutoLoading()");
            CancelAsync = 3;
            checkBoxMergeFiles.Checked = true;
            checkBoxYesterday.Checked = true;
            backgroundWorker1.RunWorkerAsync();
        }

        public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            L.Info("Strat backgroundWorker1: " + backgroundWorker1.IsBusy);

            List<SettingsMain> settingsData = GetSettings();

            if (settingsData == null)
                backgroundWorker1.CancelAsync();


            backgroundWorker1.ReportProgress(10, "Start work");

            if (backgroundWorker1.CancellationPending)
                return;

            if (checkBoxDateFromTxt.Checked)
            {
                L.Debug("Date from TXT: " + checkBoxDateFromTxt.Checked);

                var fileData = LoadTxtFile(settingsData);


                if (backgroundWorker1.CancellationPending)
                    return;

                backgroundWorker1.ReportProgress(20, "Files in the folder: " + fileData.Count);

                FinamLoading.Download(Security, fileData, settingsData);

                backgroundWorker1.ReportProgress(50, "All files save");
            }
            else
            {
                backgroundWorker1.ReportProgress(20);
                var fileData = new List<FileSecurity>();
                FinamLoading.Download(Security, fileData, settingsData);
                backgroundWorker1.ReportProgress(50, "All files save");
            }
            backgroundWorker1.ReportProgress(100, "Download complete");

        }
        public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!IsNullOrEmpty(e.UserState as string))
            {
                var time = DateTime.Now.ToString("T") + ": ";
                TextBox textBox = textBoxLog;
                string str = textBox.Text + time + (e.UserState as string) + Environment.NewLine;
                textBox.Text = str;

            }
            textBoxLog.SelectionStart = textBoxLog.TextLength;
            textBoxLog.ScrollToCaret();
            progressBar1.Value = e.ProgressPercentage;
        }
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CancelAsync == 1)
            {
                L.Info("There are no files to merge");
                MessageBox.Show(this, @"There are no files to merge");
            }
            else if (CancelAsync == 2)
            {
                L.Info("Cancel download");
                MessageBox.Show(this, @"Cancel download");
            }

            else if (CancelAsync == 3)
            {
                L.Info("Exit after autoloading");
                Application.Exit();
            }
            else if (CancelAsync == 4)
            {
                L.Info("Incorrect date");
                MessageBox.Show(@"Incorrect date");
            }
            else
            {
                L.Info("Download complete");
                MessageBox.Show(this, @"Download complete");
            }

            CancelAsync = 0;
            progressBar1.Value = 0;
        }

        private void buttonCancelDownload_Click(object sender, EventArgs e)
        {
            CancelAsync = 2;
            backgroundWorker1.CancelAsync();
        }


        private void WriteSetting()
        {
            _props.Fields.Folder = textBoxTXTDir.Text;

            _props.Fields.TimeFrom = dateTimePickerFrom.Value;

            _props.Fields.TimeTo = dateTimePickerTo.Value;

            _props.Fields.Period = comboBoxPeriod.SelectedIndex;

            _props.Fields.SplitChar = comboBoxSplitChar.SelectedIndex;

            _props.Fields.TimeCandle = comboBoxTimeCandle.SelectedIndex;

            _props.Fields.FileheaderRow = checkBoxFileheaderRow.Checked;

            _props.Fields.DateFromTxt = checkBoxDateFromTxt.Checked;

            _props.Fields.MergeFiles = checkBoxMergeFiles.Checked;

            _props.Fields.Yesterday = checkBoxYesterday.Checked;

            _props.Fields.Security = Security;

            _props.WriteXml();
        }
        private void ReadSetting()
        {
            _props.ReadXml();

            if (_props.Fields.XmlFileName != _settingsFolder)
            {
                _props.Fields.XmlFileName = _settingsFolder;
                _props.WriteXml();
                L.Info(@"Сhange the settings folder");
                MessageBox.Show(this, @"Сhange the settings folder");
            }


            textBoxTXTDir.Text = _props.Fields.Folder;

            dateTimePickerFrom.Value = _props.Fields.TimeFrom;

            dateTimePickerTo.Value = _props.Fields.TimeTo;

            comboBoxPeriod.SelectedIndex = _props.Fields.Period;

            comboBoxSplitChar.SelectedIndex = _props.Fields.SplitChar;

            comboBoxTimeCandle.SelectedIndex = _props.Fields.TimeCandle;

            checkBoxFileheaderRow.Checked = _props.Fields.FileheaderRow;

            checkBoxDateFromTxt.Checked = _props.Fields.DateFromTxt;

            checkBoxMergeFiles.Checked = _props.Fields.MergeFiles;

            checkBoxYesterday.Checked = _props.Fields.Yesterday;

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
            L.Info(@"Save settings button");
            WriteSetting();
        }

        private void buttonAddUrlSecurity_Click(object sender, EventArgs e)
        {

        }

        private void buttonDownloadTxt_Click(object sender, EventArgs e)
        {
            L.Info(@"Start backgroundWorker");
            backgroundWorker1.RunWorkerAsync();

        }

        public void SaveToFile(string data, SecurityInfo security, SettingsMain settingsData)
        {
            CheckStringData(data);

            L.Info("Start SaveToFile: " + security.Name);

            if (backgroundWorker1.CancellationPending)
                return;

            var settings = settingsData;

            Directory.CreateDirectory(settings.DirTxt + @"\" + settings.PeriodItem);

            string filename = settings.DirTxt + @"\" + settings.PeriodItem + @"\" + security.Name + @"-" + settings.DateTo.Day + @"." + settings.DateTo.Month + @"." + settings.DateTo.Year + @"-" + settings.PeriodItem + @".txt";

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
            CheckStringData(data);
            L.Info("Start ChangeFile: " + fileSec.Sec);
            if (backgroundWorker1.CancellationPending)
                return;

            var settings2 = settingsData;

            DateTime datatrue = fileSec.Dat.AddDays(-1); // для устранения лишнего дня в имени файла

            string filename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" + datatrue.Day + @"." + datatrue.Month + @"." + datatrue.Year + @"-" + settings2.PeriodItem + @".txt";

            string newfilename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" +
                                 settings2.DateTo.Day + @"." + settings2.DateTo.Month + @"." + settings2.DateTo.Year + @"-" +
                                 settings2.PeriodItem + @".txt";

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


            File.Move(filename, newfilename);
        }


        public List<FileSecurity> LoadTxtFile(List<SettingsMain> settingsData3)

        {
            L.Debug("Load txt file in folder");
            var settings3 = settingsData3[0];

            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(settings3.DirTxt + @"\" + settings3.PeriodItem); // папка с файлами 
            L.Debug("Folder: " + dir);
            var filescount = dir.GetFiles();
            //L.Debug("Files: " + filescount);
            foreach (FileInfo t in filescount)
            {
                Char delimiter = '-';
                String[] substrings = Path.GetFileNameWithoutExtension(t.FullName).Split(delimiter);

                fileHeader.Add(new FileSecurity { Sec = substrings[0], Dat = DateTime.Parse(substrings[1]).AddDays(1), Per = substrings[2] });
            }
            if (fileHeader.Count == 0)
            {
                L.Info("There are no files to merge");
                backgroundWorker1.ReportProgress(20, "There are no files to merge");
                CancelAsync = 1;
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
            foreach (SecurityInfo t in Security)
            {
                if (checkname == "")
                {
                    checkname = t.MarketName;
                    treeViewSecurity.Nodes.Add(t.MarketName);

                }
                else if (checkname == t.MarketName)
                {
                }
                else
                {
                    checkname = t.MarketName;
                    treeViewSecurity.Nodes.Add(t.MarketName);
                }
            }

            for (int i = 0; i < treeViewSecurity.Nodes.Count; i++)
            {
                foreach (SecurityInfo t in Security)
                {
                    if (treeViewSecurity.Nodes[i].Text == t.MarketName)
                    {
                        treeViewSecurity.Nodes[i].Nodes.Add(t.Name).Checked = t.Checed;

                    }
                }
            }
        }

        private void treeViewSecurity_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
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
                foreach (SecurityInfo t in Security)
                {
                    if (e.Node.Text == t.Name)
                    {
                        t.Checed = e.Node.Checked;
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
            if (checkBoxMergeFiles.Checked)
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
            if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
            {

                CancelAsync = 4;
                return null;
            }

            SettingsData.Clear();
            SettingsData.Add(new SettingsMain
            {
                Period = comboBoxPeriod.SelectedIndex,
                PeriodItem = comboBoxPeriod.SelectedItem,
                DateFrom = dateTimePickerFrom.Value,
                DateTo = dateTimePickerTo.Value,
                SplitChar = comboBoxSplitChar.SelectedIndex + 1,
                TimeCandle = comboBoxTimeCandle.SelectedIndex,
                DateFromeTxt = checkBoxDateFromTxt.Checked,
                FileheaderRow = checkBoxFileheaderRow.Checked,
                MergeFile = checkBoxMergeFiles.Checked,
                DirTxt = textBoxTXTDir.Text

            });
            L.Debug("Setting: " + SettingsData[0].Period + " " + SettingsData[0].PeriodItem + " " + SettingsData[0].DateFrom + " " + SettingsData[0].DateTo + " " + SettingsData[0].SplitChar + " " + SettingsData[0].TimeCandle + " " + SettingsData[0].DateFromeTxt + " " + SettingsData[0].FileheaderRow + " " + SettingsData[0].MergeFile + " " + SettingsData[0].DirTxt);

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


            public bool DateFromeTxt;


            public bool FileheaderRow;


            public bool MergeFile;

            public string DirTxt = Empty;

        }


        private void CheckStringData(string str)
        {
            if (str == "Вы запросили данные за слишком большой временной период.")
            {
                L.Info(str);
                MessageBox.Show(this, str);
                return;
            }
            if (str == "Система уже обрабатывает Ваш запрос. Дождитесь окончания обработки.")
            {
                L.Info(str);
                MessageBox.Show(this, str);
                return;
            }
            if (str == "")
            {
                L.Info("За эту дату нет данных");
                MessageBox.Show(this, @"За эту дату нет данных");
            }
        }
    }
}