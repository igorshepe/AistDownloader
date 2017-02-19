using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        //public List<SettingsMain> SettingsData = new List<SettingsMain>();
        private bool _firststart = true;
        public int CancelAsync; // 1 нет файлов для объединения 2 отмена кнопкой
        public bool AutoloadingStart;
        private readonly string _settingsFolder = Environment.CurrentDirectory + "\\settings.xml";
        public SettingsMain SettingsData;
        public SettingsMain Changedata ;
        public SettingsMain Changedata2;
        public Main(string[] args)
        {
            var arg = args;
            InitializeComponent();

            Application.ApplicationExit += OnApplicationExit;

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
                    L.Info("Запуск с атрибутом (autoloading)");
                    AutoLoading();
                }
            }

            buttonCancelDownload.Enabled = false;
            buttonAddUrlSecurity.Enabled = false;

            dateTimePickerTo.MaxDate = DateTime.Now.AddDays(-1);
        }


        public void AutoLoading() // запуск с ключом autoloading
        {
            L.Debug("AutoLoading()");
            AutoloadingStart = true;
            CancelAsync = 3;
            checkBoxMergeFiles.Checked = true;
            checkBoxYesterday.Checked = true;
            SettingsData = GetSettings();
            backgroundWorker1.RunWorkerAsync();
        }

        private delegate void StateButtonDownload(bool state);

        private delegate void StateTreeViewSecurity(bool state);
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

        private void StateTreeView (bool state)
        {
            if (treeViewSecurity.InvokeRequired)
            {
                StateTreeViewSecurity dd = StateTreeView; Invoke(dd, state);
            }
            else
            {
                treeViewSecurity.Enabled = state;
            }

        }
        public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StateButton(true);
            StateTreeView(false);

            L.Info("Запускаем backgroundWorker1: " + backgroundWorker1.IsBusy);

            
            
            if (SettingsData == null)
                backgroundWorker1.CancelAsync();

            var settings = SettingsData;

            AddTextLog("Начинаем работать");

            if (backgroundWorker1.CancellationPending)
                return;

            if (checkBoxDateFromTxt.Checked) // Берем данные из существующих файлов
            {
                L.Debug("Дата из файла: " + checkBoxDateFromTxt.Checked);

                List<FileSecurity> fileData = new List<FileSecurity>();

                if (checkBoxNoData.Checked)
                {
                    fileData = ReadDateOfTheFile(SettingsData);

                }
                else
                {
                    fileData = LoadTxtFile(SettingsData);
                }
               

                

                if (backgroundWorker1.CancellationPending)
                    return;

                 

                

                var index = 1;

                foreach (var security in Security)
                {
                    
                    foreach (var filesSecurities in fileData)
                    {
                        if (backgroundWorker1.CancellationPending)
                            return;

                        if (security.Name == filesSecurities.Sec)
                        {
                            if (filesSecurities.Dat.AddDays(-1) > settings.DateTo)
                            {
                                L.Info(  $"{filesSecurities.Sec}: Дата файла {filesSecurities.Dat.AddDays(-1).ToString("d")} старше даты загружаемых данных {settings.DateTo.ToString("d")}");
                                AddTextLog(  $"{filesSecurities.Sec}: Дата файла {filesSecurities.Dat.AddDays(-1).ToString("d")} старше даты загружаемых данных {settings.DateTo.ToString("d")}");

                            }
                            else
                            {
                                if (!(filesSecurities.Dat.AddDays(-1) == settings.DateTo))
                                {
                                    AddTextLog($"Загружаю: {filesSecurities.Sec}");
                                    var str = FinamLoading.DownloadData(security, filesSecurities, SettingsData, true);
                                    int state = CheckStringData(str, filesSecurities.Sec);


                                    if (state == 0 || state == 3)
                                    {
                                        if (checkBoxMergeFiles.Checked)
                                        {
                                            ChangeFile(str, filesSecurities, settings);
                                        }
                                        else
                                        {
                                            //доделать сохранение файла 
                                            // SaveToFile(str, filesSecurities, settings); 
                                            L.Info("Выберите объединение файлов!");
                                            AddTextLog("Выберите объединение файлов!");
                                            backgroundWorker1.CancelAsync();
                                            CancelAsync = 5;

                                        }

                                    }
                                    else if (state == 2 || state == 4)
                                    {
                                        backgroundWorker1.CancelAsync();
                                        CancelAsync = 2;
                                    }
                                    else if (state == 1)
                                    {
                                        str = Empty;
                                        if (checkBoxMergeFiles.Checked)
                                        {
                                            ChangeFile(str, filesSecurities, settings);
                                        }
                                        else
                                        {
                                            //доделать сохранение файла 
                                            // SaveToFile(str, filesSecurities, settings);
                                            L.Info("Выберите объединение файлов");
                                            AddTextLog("Выберите объединение файлов!");
                                            backgroundWorker1.CancelAsync();
                                            CancelAsync = 5;
                                        }

                                    }

                                    backgroundWorker1.ReportProgress(100 * index / fileData.Count);
                                    ++index;

                                }
                                else
                                {
                                    AddTextLog("Дата файла и дата загрузки истории совпадают: " + filesSecurities.Sec + " Дата с: " + filesSecurities.Dat.AddDays(-1).ToString("d") + ". По: " + settings.DateTo.ToString("d"));
                                    L.Info("Дата файла и дата загрузки истории совпадают: " + filesSecurities.Sec + " Дата с: " + filesSecurities.Dat.AddDays(-1).ToString("d") + ". По: " + settings.DateTo.ToString("d"));
                                }
                            }

                            

                            
                        }


                    }

                    
                }
                
                //FinamLoading.Download(Security, fileData, settingsData);

               AddTextLog("Все файлы сохранены");
            }
            else // Создаем новые файлы
            {
                if (backgroundWorker1.CancellationPending)
                    return;

               

                var fileData = new FileSecurity();
                
                if (backgroundWorker1.CancellationPending)
                    return;
                var checedCount = Security.Count(sec => sec.Checed);

                var index = 1;

                foreach (var security in Security)
                {
                    if (backgroundWorker1.CancellationPending)
                        return;

                    if (security.Checed)
                    {

                        var firstDate = SettingsData.DateFrom;
                        var currentDate = SettingsData.DateTo;
                        var span = currentDate - firstDate;
                        decimal relative = span.Days;
                        if (!(relative >= 730))
                            // 730дней это два года, больше запрашивать нельзя с финама придет пустой файл
                        {
                            AddTextLog($"Загружаю: {security.Name}");
                            var str = FinamLoading.DownloadData(security, fileData, SettingsData, false);
                            var state = CheckStringData(str,  security.Name);

                            if (state == 0 || state == 3)
                            {
                                SaveToFile(str, security, settings);
                            }
                            else if (state == 2 || state == 4)
                            {
                                backgroundWorker1.CancelAsync();
                                CancelAsync = 2;
                            }
                            else if (state == 1)
                            {
                                str = Empty;
                                SaveToFile(str, security, settings);
                            }
                        }
                        else
                        {
                            AddTextLog($"Вы запросили информацию больше чем за 2 года. Ожидайте.");
                            L.Info($"Вы запросили информацию больше чем за 2 года. Ожидайте. Загружаю: {security.Name}");
                            decimal oneYear = Math.Floor(relative/365);
                            
                            //var changedata = Changedata;
                            //var changedata2 = Changedata2;

                            var changedata = GetSettings();
                            var changedata2 = GetSettings();

                            bool header = true;
                            var str = Empty;

                            for (int i = 1; i <= oneYear; i++)
                            {
                                if (backgroundWorker1.CancellationPending)
                                    return;

                                int per = (int) Math.Round(10*i/(oneYear));

                                if (index != 1)
                                {
                                    int d = (100*(index - 1)/checedCount)+per;
                                    per = d;
                                }
                                
                                backgroundWorker1.ReportProgress(per);

                                if (i == 1)
                                {
                                    //var date = changedata[0].DateFrom;
                                    changedata.DateTo = changedata.DateFrom.AddYears(+1);
                                    changedata.DateTo = changedata.DateTo.AddDays(-1);
                                    AddTextLog($"Загружаю: {security.Name} '{changedata.DateFrom.ToString("d")} - {changedata.DateTo.ToString("d")}'");
                                    L.Info($"Загружаю: {security.Name} '{changedata.DateFrom.ToString("d")} - {changedata.DateTo.ToString("d")}'");
                                    var str2 = FinamLoading.DownloadData(security, fileData, changedata, false);
                                    if (str2 != "")
                                    { 
                                        header = false;
                                    }
                                    str = str2;
                                }
                                else
                                {
                                    if (!header)
                                    {
                                        changedata.FileheaderRow = false;
                                    }

                                    var oldDateTo = changedata.DateTo;
                                    
                                    
                                    changedata.DateFrom = oldDateTo.AddDays(+1);
                                    changedata.DateTo = (changedata.DateFrom.AddYears(+1));
                                    changedata.DateTo = changedata.DateTo.AddDays(-1);
                                    AddTextLog($"Загружаю: {security.Name} '{changedata.DateFrom.ToString("d")} - {changedata.DateTo.ToString("d")}'.");
                                    L.Info($"Загружаю: {security.Name} '{changedata.DateFrom.ToString("d")} - {changedata.DateTo.ToString("d")}'.");
                                    var str3 = FinamLoading.DownloadData(security, fileData, changedata, false);
                                    if (str3 != "")
                                    {
                                        header = false;
                                    }

                                    str += str3;
                                }

                                

                            }

                            

                            changedata2.DateFrom = changedata.DateTo.AddDays(+1);
                            if (!header)
                            {
                                changedata2.FileheaderRow = false;
                            }
                            
                            AddTextLog($"Загружаю: {security.Name} '{changedata2.DateFrom.ToString("d")} - {changedata2.DateTo.ToString("d")}'.");
                            L.Info($"Загружаю: {security.Name} '{changedata2.DateFrom.ToString("d")} - {changedata2.DateTo.ToString("d")}'.");
                            var str4 = FinamLoading.DownloadData(security, fileData, changedata2, false);
                            str += str4;
                            
                            var state = CheckStringData(str, security.Name);

                                if (state == 0 || state == 3)
                                {
                                    SaveToFile(str, security, settings);
                                }
                                else if (state == 2 || state == 4)
                                {
                                    backgroundWorker1.CancelAsync();
                                    CancelAsync = 2;
                                }
                                else if (state == 1)
                                {
                                    str = Empty;
                                    SaveToFile(str, security, settings);
                                }

                            GC.Collect();
                        }
                        

                        backgroundWorker1.ReportProgress(100 * index / checedCount);
                        ++index;

                    }

                }
               AddTextLog( "Все файлы сохранены");
            }


            AddTextLog( "Загрузка завершена");
            
            


        }
        public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending)
            {
                
                return;
            }

            AddTextLog(e.UserState as string);

            if (e.ProgressPercentage > 100)
            {
                L.Info($"Значение больше 100 (progressBar) : {e.ProgressPercentage}  {e.UserState}");
                return;
            }
             
            progressBar1.Value = e.ProgressPercentage;
        }
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CancelAsync == 1)
            {
                L.Info("Нет файлов для объединения");
                MessageBox.Show(this, @"Нет файлов для объединения");
            }
            else if (CancelAsync == 2)
            {
                AddTextLog("Отмена загрузки");
                L.Info("Отмена загрузки");
                MessageBox.Show(this, @"Отмена загрузки");
            }

            else if (CancelAsync == 3)
            {
                L.Info("Выход после автозагрузки");
                Application.Exit();
            }
            else if (CancelAsync == 4)
            {
                L.Info("Неверная дата");
                MessageBox.Show(@"Неверная дата");
            }
            else if (CancelAsync == 5)
            {
                L.Info("Ошибка");
                MessageBox.Show(@"Ошибка. Смотри диалоговое окно");
            }
            else
            {
                L.Info("Загрузка завершена");
                MessageBox.Show(this, @"Загрузка завершена");
            }

            CancelAsync = 0;
            progressBar1.Value = 0;
            StateButton(false);
            StateTreeView(true);
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

            _props.Fields.NoData = checkBoxNoData.Checked;

            _props.Fields.Security = Security;

            _props.WriteXml();

            AddTextLog("Настройки сохранены");
        }
        private void ReadSetting()
        {
            _props.ReadXml();

            if (_props.Fields.XmlFileName != _settingsFolder)
            {
                _props.Fields.XmlFileName = _settingsFolder;
                _props.WriteXml();
                L.Info(@"Путь к файлу настроект исправлен");
                AddTextLog("Путь к файлу настроект исправлен");
                //MessageBox.Show(this, @"Сhange the settings folder");
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

            checkBoxNoData.Checked = _props.Fields.NoData;

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
            L.Info(@"Нажатие на кнопку 'Сохранить'");
            WriteSetting();
        }

        private void buttonAddUrlSecurity_Click(object sender, EventArgs e)// Добавление инструмента по ссылке с finam
        {
            if (textBoxUrlSecurity.Text == "")
            {
                return;
            }

            L.Info($"Добавляем новый инструмент. URL: {textBoxUrlSecurity.Text}");

            var webGet = new HtmlWeb { OverrideEncoding = Encoding.GetEncoding(1251) };
            var doc = webGet.Load(textBoxUrlSecurity.Text);

            HtmlNode ournote = doc.DocumentNode.SelectSingleNode("//*[@id='content-block']/script[1]/text()");
            if (ournote == null)
            {
                L.Info(@"Нет данных для парсинга");
                AddTextLog(@"Нет данных для парсинга");
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
                    AddTextLog(@"Инструмент "+ name +@" уже есть в коллекции"  );
                    textBoxUrlSecurity.Clear();
                    return;
                }

                Security.Add(new SecurityInfo
                {
                    Checed = true,
                    Code = code,
                    Id = id,
                    MarketId = marketId,
                    MarketName = marketName,
                    Name = name
                });
                L.Info($"New security:id-{id}, code-{code},name-{name}, marketId-{marketId}, marketName-{marketName} ");

                AddTextLog($"New security: {id}, {code}, {name}, {marketId}, {marketName} ");
                 
            }
            textBoxUrlSecurity.Clear();
            WriteSetting();
            LoadTreeview();
            
        }

        private void buttonDownloadTxt_Click(object sender, EventArgs e)
        {
             
            if (!backgroundWorker1.IsBusy)
            {
                SettingsData = GetSettings();
                //Changedata = GetSettings();
                //Changedata2 = GetSettings();

                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show(this, @"backgroundWorker уже работает");
                L.Debug("backgroundWorker уже работает"); 
            }
            
        }

        public void SaveToFile(string data, SecurityInfo security, SettingsMain settingsData)
        {

            if (backgroundWorker1.CancellationPending)
                return;

            L.Info("Сохраняем в файл: " + security.Name);

            var settings = settingsData;

            Directory.CreateDirectory(settings.DirTxt + @"\" + settings.PeriodItem);

            string filename = null;
            if (checkBoxNoData.Checked)
            {
                filename = settings.DirTxt + @"\" + settings.PeriodItem + @"\" + security.Name + @"-"+ settings.PeriodItem + @".txt";

            }
            else
            {
                 filename = settings.DirTxt + @"\" + settings.PeriodItem + @"\" + security.Name + @"-" + settings.DateTo.Day + @"." + settings.DateTo.Month + @"." + settings.DateTo.Year + @"-" + settings.PeriodItem + @".txt";

            }

            if (File.Exists(filename))
            {
                L.Info($"Файл уже существует {filename}, перезаписать ?");
                string message = ($"Файл уже существует {filename}, перезаписать ?");
                const string caption = "Save data";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                // If the no button was pressed ...
                if (result == DialogResult.No)
                {
                    L.Info("Перезаписать файл? - НЕТ");
                    return;
                }

                L.Info("Перезаписать файл? - ДА");
            }

            if (filename != "")
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                    {
                        sw.Write(data);
                        sw.Close();
                        AddTextLog($"Сохранили в файл: {security.Name}");

                    }
                }
                catch (Exception e)
                {
                     L.Error(e.Message);   
                    AddTextLog(e.Message);
                }
                
            }

        }

        public void ChangeFile(string data, FileSecurity fileSec, SettingsMain settingsData)
        {

            if (backgroundWorker1.CancellationPending)
                return;

            L.Info("Объединяем файлы: " + fileSec.Sec);
            
            var settings2 = settingsData;

            DateTime datatrue = fileSec.Dat.AddDays(-1); // для устранения лишнего дня в имени файла
            string filename = null;
            string newfilename = null;

            if (checkBoxNoData.Checked)
            {
                filename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" + settings2.PeriodItem + @".txt";

                newfilename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" + settings2.PeriodItem + @".txt";
            }
            else
            {
             filename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" + datatrue.Day + @"." + datatrue.Month + @"." + datatrue.Year + @"-" + settings2.PeriodItem + @".txt";

             newfilename = settings2.DirTxt + @"\" + settings2.PeriodItem + @"\" + fileSec.Sec + @"-" +
                                     settings2.DateTo.Day + @"." + settings2.DateTo.Month + @"." + settings2.DateTo.Year + @"-" +
                                     settings2.PeriodItem + @".txt";
            }
           

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

            if (!checkBoxNoData.Checked)
                File.Move(filename, newfilename);

            AddTextLog($"Файлы объединены: {fileSec.Sec }");
        }

        public List<FileSecurity> ReadDateOfTheFile(SettingsMain settingsData4)
        {
            var settings4 = settingsData4;
            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(settings4.DirTxt + @"\" + settings4.PeriodItem); // папка с файлами 

            var filescount = dir.GetFiles();
            L.Debug($"Файлов: {filescount.Count()} в папке: {dir}");
            try
            {
                foreach (FileInfo t in filescount)
                {
                    var lastString = File.ReadAllLines(t.FullName).Last();
                    Char delimiter = ',';
                    String[] substrings = lastString.Split(delimiter);
                    string dateStr;
                    if (settings4.PeriodItem == "tics")
                    {
                         dateStr = substrings[2];
                    }
                    else
                    {
                        dateStr = substrings[0];
                    }
                   
                   
                    int year =Convert.ToInt32($"{dateStr[0]}{dateStr[1]}{dateStr[2]}{dateStr[3]}");
                    int month = Convert.ToInt32($"{dateStr[4]}{dateStr[5]}");
                    int day = Convert.ToInt32($"{dateStr[6]}{dateStr[7]}");
                    DateTime date = new DateTime(year,month,day);
                    Char delimiterHeader = '-';
                    String[] substringsHeader = Path.GetFileNameWithoutExtension(t.FullName).Split(delimiterHeader);
                    if (substringsHeader.Length == 2)
                    {
                        fileHeader.Add(new FileSecurity { Sec = substringsHeader[0], Dat = date.AddDays(1), Per = settings4.PeriodItem.ToString() });
                    }
                   
                }
                if (fileHeader.Count == 0)
                {
                    L.Info("Нет файлов для объединения");
                    AddTextLog("Нет файлов для объединения");
                    CancelAsync = 1;
                    backgroundWorker1.CancelAsync();

                }

               
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }

            return fileHeader;

        }
        public List<FileSecurity> LoadTxtFile(SettingsMain settingsData3)

        {
             
            var settings3 = settingsData3;

            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(settings3.DirTxt + @"\" + settings3.PeriodItem); // папка с файлами 
           
            var filescount = dir.GetFiles();
            L.Debug($"Файлов: {filescount.Count()} в папке: {dir}"  );
            foreach (FileInfo t in filescount)
            {
                Char delimiter = '-';
                String[] substrings = Path.GetFileNameWithoutExtension(t.FullName).Split(delimiter);
                if (substrings.Length == 3)
                {
                    fileHeader.Add(new FileSecurity { Sec = substrings[0], Dat = DateTime.Parse(substrings[1]).AddDays(1), Per = substrings[2] });
                }
                
            }
            if (fileHeader.Count == 0)
            {
                L.Info("Нет файлов для объединения");
                AddTextLog("Нет файлов для объединения");
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

            var checkname = Empty;
            foreach (var t in Security)
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

        public SettingsMain GetSettings()
        {
            SettingsMain settingsData = null;

            if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
            {

                CancelAsync = 4;
                return null;
            }
           
            

            try
            {
                
                settingsData = new SettingsMain
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


                };
                L.Debug("Настройки: " + settingsData.Period + " " + settingsData.PeriodItem + " " + settingsData.DateFrom.ToString("d") + " " + settingsData.DateTo.ToString("d") + " " + settingsData.SplitChar + " " + settingsData.TimeCandle + " " + settingsData.DateFromeTxt + " " + settingsData.FileheaderRow + " " + settingsData.MergeFile + " " + settingsData.DirTxt);

            }
            catch (Exception e)
            {
                AddTextLog(e.Message);
                L.Debug(e);
             }
            
            return settingsData;
        }

        
        public class SettingsMain
        {
            public int Period { get; set; }


            public object PeriodItem { get; set; }

            public DateTime DateFrom { get; set; }


            public DateTime DateTo { get; set; }


            public int SplitChar { get; set; }


            public int TimeCandle { get; set; }


            public bool DateFromeTxt { get; set; }


            public bool FileheaderRow { get; set; }


            public bool MergeFile { get; set; }

            public string DirTxt { get; set; }

            public bool Autostart { get; set; }
        }

        public delegate void DelegAutoLoading(bool state);
        private int CheckStringData(string str, string sec)
        {
             

            if (str == "Вы запросили данные за слишком большой временной период.")
            {
                AddTextLog($"{sec} {str}");
                L.Info(str);
               return 1;
            }
            if (str == "Error")
            {
                AddTextLog($"{sec} {str} ");
                L.Info(str);
                return 1;
            }
            if (str == "Система уже обрабатывает Ваш запрос. Дождитесь окончания обработки.")
            {
                AddTextLog($"{sec} {str}");
                AddTextLog(str);
                L.Info(str);
               return 2;
            }
            if (str == "")
            {
                AddTextLog($"{sec} За эту дату нет данных");

                L.Info("За эту дату нет данных");

               return 3;
            }
            if (str== "Exception")
            {
                AddTextLog($"{sec} {str}");
                L.Info("Ошибка при попытке загрузки файла");

                return 4;
            }
            return 0;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            Process.Start("http://www.finam.ru/analysis/quotes/");
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

                        string message = ($"Удалить инструмент: {nameDelSec.Text} ?");
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
                                    L.Info($"Удалили {t.Name}, {t.Code}, {t.Id}, {t.MarketName}, {t.MarketId}");

                                     
                                    AddTextLog($"Удалили {t.Name}, {t.Code}, {t.Id}, {t.MarketName}, {t.MarketId}"); 
                                    

                                    Security.Remove(t);
                                   break;
                                }

                                 
                            }
                            LoadTreeview();
                            WriteSetting();
                        }

                       
                }
                    break;

                 
            }
        }

        private void textBoxUrlSecurity_TextChanged(object sender, EventArgs e)
        {
            buttonAddUrlSecurity.Enabled = textBoxUrlSecurity.Text != "";
        }

        public void AddTextLog(string message)
        {
            if (!IsNullOrEmpty(message))
            {
                var time = DateTime.Now.ToString("T") + ": ";

                if (InvokeRequired)
                {
                    Invoke(new Action<string>(AddTextLog), message);
                    return;
                }
                textBoxLog.AppendText($"{time} {message}{Environment.NewLine}"); // 


            }
             
        }

        private void dateTimePickerTo_ValueChanged(object sender, EventArgs e)
        {

            CheckDate();
          
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            CheckDate();
            
        }


        private void CheckDate()
        {
          
            dateTimePickerFrom.MaxDate = dateTimePickerTo.Value;
            dateTimePickerTo.MinDate = dateTimePickerFrom.Value;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            
            //if (_props.Fields.Security != Security)
            //{
            //    string message = ($"Данные по инструментам были изменены. Сохранить?");
            //    const string caption = "Save data";
            //    var result = MessageBox.Show(message, caption,
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Question);


            //    if (result == DialogResult.Yes)
            //    {
            //        WriteSetting();
            //    }
            //}
            
        }
    }
}

