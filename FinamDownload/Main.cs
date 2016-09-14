using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using FinamDownloader.Properties;
using HtmlAgilityPack;
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
        public bool AutoloadingStart = false ;
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

            buttonCancelDownload.Enabled = false;

        }


        public void AutoLoading() // запуск с ключом autoloading
        {
            L.Debug("AutoLoading()");
            AutoloadingStart = true;
            CancelAsync = 3;
            checkBoxMergeFiles.Checked = true;
            checkBoxYesterday.Checked = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private delegate void StateButtonDownload(bool state);
        
        private void StateButton(bool state)
        {
            if (buttonCancelDownload.InvokeRequired)
            {
                StateButtonDownload dd = StateButton;Invoke(dd, state);
            }
            else
            {
                buttonCancelDownload.Enabled = state;
            }

        }
        public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        { 
                StateButton(true);
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
                if (backgroundWorker1.CancellationPending)
                    return;


                backgroundWorker1.ReportProgress(20);

                    var fileData = new List<FileSecurity>();

                if (backgroundWorker1.CancellationPending)
                    return;

                FinamLoading.Download(Security, fileData, settingsData);
                backgroundWorker1.ReportProgress(50, "All files save");
                }
                backgroundWorker1.ReportProgress(100, "Download complete");
            
            


        }
        public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending)
            {
                
                return;
            }

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
                var time = DateTime.Now.ToString("T") + ": ";
                TextBox textBox = textBoxLog;
                string str = textBox.Text + time + @"Cancel download" + Environment.NewLine;
                textBox.Text = str;
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
            StateButton(false);
        }

        private void buttonCancelDownload_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            CancelAsync = 2;
            
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

        private void buttonAddUrlSecurity_Click(object sender, EventArgs e)// Добавление инструмента по ссылке с finam
        {
            if (textBoxUrlSecurity.Text == "")
            {
                return;
            }

            L.Info(@"Add new Security");
            var webGet = new HtmlWeb { OverrideEncoding = Encoding.GetEncoding(1251) };
            var doc = webGet.Load(textBoxUrlSecurity.Text);

            HtmlNode ournote = doc.DocumentNode.SelectSingleNode("//*[@id='content-block']/script[1]/text()");
            if (ournote == null)
            {
                L.Info(@"Нет данных для парсинга");
                MessageBox.Show(@"Нет данных для парсинга");
                return;
            }

            string dd = ournote.InnerText;
            var delimiter = ';';
            String[] substrings = dd.Split(delimiter);
            string bb = substrings[3];


            string re1 = "(\\{)";   // Any Single Character 1
            string re2 = "(\"quote\")"; // Double Quote String 1
            string re3 = "(:)"; // Any Single Character 2
            string re4 = "( )"; // White Space 1
            string re5 = "(\\{)";   // Any Single Character 3
            string re6 = "(\".*?\")";   // Double Quote String 2
            string re7 = "(:)"; // Any Single Character 4
            string re8 = "( )"; // White Space 2
            string re9 = "(\\d+)";  // Integer Number 1
            string re10 = "(,)";    // Any Single Character 5
            string re11 = "( )";    // White Space 3
            string re12 = "(\"code\")"; // Double Quote String 3
            string re13 = "(:)";    // Any Single Character 6
            string re14 = "( )";    // White Space 4
            string re15 = "(\".*?\")";  // Double Quote String 4
            string re16 = ".*?";    // Non-greedy match on filler
            string re17 = " ";  // Uninteresting: ws
            string re18 = ".*?";    // Non-greedy match on filler
            string re19 = " ";  // Uninteresting: ws
            string re20 = ".*?";    // Non-greedy match on filler
            string re21 = "( )";    // White Space 5
            string re22 = "(\"title\")";    // Double Quote String 5
            string re23 = "(:)";    // Any Single Character 7
            string re24 = "( )";    // White Space 6
            string re25 = "(\".*?\")";  // Double Quote String 6
            string re26 = ".*?";    // Non-greedy match on filler
            string re27 = " ";  // Uninteresting: ws
            string re28 = ".*?";    // Non-greedy match on filler
            string re29 = " ";  // Uninteresting: ws
            string re30 = ".*?";    // Non-greedy match on filler
            string re31 = " ";  // Uninteresting: ws
            string re32 = ".*?";    // Non-greedy match on filler
            string re33 = " ";  // Uninteresting: ws
            string re34 = ".*?";    // Non-greedy match on filler
            string re35 = "( )";    // White Space 7
            string re36 = "(\"market\")";   // Double Quote String 7
            string re37 = "(:)";    // Any Single Character 8
            string re38 = "( )";    // White Space 8
            string re39 = "(\\{)";  // Any Single Character 9
            string re40 = "(\"id\")";   // Double Quote String 8
            string re41 = "(:)";    // Any Single Character 10
            string re42 = "( )";    // White Space 9
            string re43 = "(\\d+)"; // Integer Number 2
            string re44 = "(,)";    // Any Single Character 11
            string re45 = "( )";    // White Space 10
            string re46 = "(\"title\")";    // Double Quote String 9
            string re47 = "(:)";    // Any Single Character 12
            string re48 = "( )";    // White Space 11
            string re49 = "(\".*?\")";  // Double Quote String 10
            String[] quoting = { };
            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8 + re9 + re10 + re11 + re12 + re13 + re14 + re15 + re16 + re17 + re18 + re19 + re20 + re21 + re22 + re23 + re24 + re25 + re26 + re27 + re28 + re29 + re30 + re31 + re32 + re33 + re34 + re35 + re36 + re37 + re38 + re39 + re40 + re41 + re42 + re43 + re44 + re45 + re46 + re47 + re48 + re49, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(bb);

            if (m.Success)
            {
                char[] charsToTrim = { '"', ' ', '\'' };
                 
                var id = Convert.ToInt32(m.Groups[9].Value); // Id 
                 
                var code = m.Groups[15].ToString().Trim(charsToTrim); // Code
                 
                var name = m.Groups[20].ToString().Trim(charsToTrim); // Name
                
                var marketId = Convert.ToInt32(m.Groups[29].Value);  // MarketId
                
               var marketName = m.Groups[35].ToString().Trim(charsToTrim); // MarketName

                if (Security.Any(t => t.Code == code))
                {
                    L.Info(@"Инструмент " + name + @" уже есть в коллекции");
                    MessageBox.Show(@"Инструмент "+ name +@" уже есть в коллекции"  );
                    textBoxUrlSecurity.Clear();
                    return;
                }

                Security.Add(new SecurityInfo { Checed = true, Code = code, Id = id, MarketId = marketId, MarketName = marketName,Name = name });
                L.Info($"New security:id {id},code {code},name {name},marketId {marketId}, marketName {marketName} ");

                var time = DateTime.Now.ToString("T") + ": ";
                TextBox textBox = textBoxLog;
                string str = textBox.Text + time + ($"New security: {id}, {code}, {name}, {marketId}, {marketName} ") + Environment.NewLine;
                textBox.Text = str;
            }
            textBoxUrlSecurity.Clear();
            WriteSetting();
            LoadTreeview();


        }

        private void buttonDownloadTxt_Click(object sender, EventArgs e)
        {
            L.Info(@"Start backgroundWorker");
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show(this, @"backgroundWorker is busy");
                L.Debug("backgroundWorker is busy"); 
            }
            
        }

        public void SaveToFile(string data, SecurityInfo security, SettingsMain settingsData)
        {
            CheckStringData(data, settingsData.Autostart, security.Name);

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
            CheckStringData(data, settingsData.Autostart, fileSec.Sec);
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
            if (treeViewSecurity != null)
            {
                treeViewSecurity.Nodes.Clear();
            }

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
                    bool compare = false;
                    checkname = t.MarketName;
                    for (int i = 0; i < treeViewSecurity.Nodes.Count; i++)
                    {
                       var dd =  treeViewSecurity.Nodes[i].Text;
                        if (dd == checkname)
                        {
                            compare = true;
                        }
                    
                    }

                    if (!compare)
                    {
                        treeViewSecurity.Nodes.Add(t.MarketName); 
                     }
                    
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
            try
            {
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
                    DirTxt = textBoxTXTDir.Text,
                    Autostart = AutoloadingStart


                });
                L.Debug("Setting: " + SettingsData[0].Period + " " + SettingsData[0].PeriodItem + " " + SettingsData[0].DateFrom.ToString("d") + " " + SettingsData[0].DateTo.ToString("d") + " " + SettingsData[0].SplitChar + " " + SettingsData[0].TimeCandle + " " + SettingsData[0].DateFromeTxt + " " + SettingsData[0].FileheaderRow + " " + SettingsData[0].MergeFile + " " + SettingsData[0].DirTxt);

            }
            catch (Exception e)
            {

                L.Debug(e);}
            
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

            public bool Autostart = false;
        }

        private delegate void DelegAutoLoading(bool state);
        private void CheckStringData(string str, bool auto, string sec)
        {
            if (str == "Вы запросили данные за слишком большой временной период.")
            {
                L.Info(str);
                if (!auto) MessageBox.Show(this, str, @"Security: " + sec);

                return;
            }
            if (str == "Система уже обрабатывает Ваш запрос. Дождитесь окончания обработки.")
            {
                L.Info(str);
                if (!auto)
                    MessageBox.Show(this, str,@"Security: "+sec);
                return;
            }
            if (str == "")
            {
                L.Info("За эту дату нет данных");
                
                if ( !auto)
                    MessageBox.Show(this, @"За эту дату нет данных", @"Security: " + sec);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.finam.ru/analysis/quotes/");
        }

        private void treeViewSecurity_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                // Remove the TreeNode under the mouse cursor 
                // if the right mouse button was clicked. 
                case MouseButtons.Right:
                {
                        var nameDelSec = treeViewSecurity.GetNodeAt(e.X, e.Y);

                    if (Security.Any(t => t.MarketName == nameDelSec.Text))
                    {
                        return;
                    }

                        string message = ($"Delete security: {nameDelSec.Text} ?");
                        const string caption = "Delete data";
                        var result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                        // If the no button was pressed ...
                        if (result == DialogResult.Yes)
                        {
                            
                            foreach (var t in Security)
                            {
                                if (t.Name == nameDelSec.Text)
                                {
                                    L.Info($"Delete {t.Name}, {t.Code}, {t.Id}, {t.MarketName}, {t.MarketId}");

                                    var time = DateTime.Now.ToString("T") + ": ";
                                    TextBox textBox = textBoxLog;
                                    string str = textBox.Text + time + ($"Delete {t.Name}, {t.Code}, {t.Id}, {t.MarketName}, {t.MarketId}") + Environment.NewLine;
                                    textBox.Text = str;

                                    Security.Remove(t);
                                   break;
                                }

                                 
                            }
                            LoadTreeview();
                        }

                        WriteSetting();
                }
                    break;

                 
            }
        }
    }
}