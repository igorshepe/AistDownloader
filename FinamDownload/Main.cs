using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using FinamDownloader.Properties;
using static System.String;

namespace FinamDownloader
{
    public partial class Main : Form
    {
        readonly Props _props = new Props();
        public List<SecurityInfo> Security = new List<SecurityInfo>();

        private bool firststart = true;
        public Main()
        {
            

            InitializeComponent();
            
            this.comboBoxPeriod.Items.AddRange(new object[]
            {
                (object) "tics",
                (object) "1 min",
                (object) "5 min",
                (object) "10 min",
                (object) "15 min",
                (object) "30 min",
                (object) "1 hour",
                (object) "1 day",
                (object) "1 weak",
                (object) "1 month"
            });
            this.comboBoxSplitChar.Items.AddRange(new object[]
            {
                (object) "запятая (,)",
                (object) "точка (.)",
                (object) "точка с запятой (;)",
                (object) "табуляция (>)",
                (object) "пробел ( )"
                
            });
            this.comboBoxTimeCandle.Items.AddRange(new object[]
            {
                (object) "Open time",
                (object) "Close time"
                 

            });
            ReadSetting();
            
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

            Security = _props.Fields.Security;
            
            LoadTreeview();
            
            firststart = false; // для проверки изменения состояния чекбокса treeview
        }

         
        public int Period
        {
            get { return comboBoxPeriod.SelectedIndex; }
        }

        public DateTime DateFrom
        {
            get { return dateTimePickerFrom.Value; }
        }

        public DateTime DateTo
        {
            get { return dateTimePickerTo.Value; }
        }

        public int SplitChar
        {
            get { return comboBoxSplitChar.SelectedIndex+1; }// нет значения 0
        }

        public int TimeCandle
        {
            get { return comboBoxTimeCandle.SelectedIndex; }
        }

        public bool DateFromeTxt
        {
            get { return checkBoxDateFromTxt.Checked; }
        }
        public bool FileheaderRow
        {
            get { return checkBoxFileheaderRow.Checked; }
        }

        public bool MergeFile
        {
            get { return checkBoxMergeFiles.Checked; }
        }
        public List<SecurityInfo> SecInfo;
        //{
        //    //get { return 1; }
        //} 
        private void buttonTXTDir_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
            if (DialogResult.OK != this.folderBrowserDialog1.ShowDialog())
                return;
            this.textBoxTXTDir.Text = this.folderBrowserDialog1.SelectedPath;
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
            var fileData = LoadTxtFile();

            if (checkBoxDateFromTxt.Checked)
            {
                FinamLoading.Download(Security, fileData);

            }
            else
            {
                fileData = null;
                FinamLoading.Download(Security,fileData);
            }
             
        }

        public void SaveToFile(string data, SecurityInfo security)
        {
            Directory.CreateDirectory(textBoxTXTDir.Text+ @"\" + comboBoxPeriod.SelectedItem);

            string filename = textBoxTXTDir.Text+@"\"+comboBoxPeriod.SelectedItem + @"\" + security.Name + @"-"+DateTo.Day+@"."+DateTo.Month + @"."+DateTo.Year+@"-"+comboBoxPeriod.SelectedItem+@".txt";
             
             
            if (filename != "")
            {
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    string newdata = data;
                    sw.Write(data);
                    sw.Close();

                }
                }

        }

        public void ChangeFile(string data, FileSecurity fileSec)
        {
            
            string filename = textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem + @"\" + fileSec.Sec + @"-" + fileSec.Dat.Day + @"." + fileSec.Dat.Month + @"." + fileSec.Dat.Year + @"-" + comboBoxPeriod.SelectedItem + @".txt";

            string newfilename = textBoxTXTDir.Text + @"\" + comboBoxPeriod.SelectedItem + @"\" + fileSec.Sec + @"-" +
                                 DateTo.Day + @"." + DateTo.Month + @"." + DateTo.Year + @"-" +
                                 comboBoxPeriod.SelectedItem + @".txt";
            using (var destination = File.AppendText(filename))
            {
                 
                destination.Write(data); // записываем дату время и сотояние портфеля
            }

            File.Move(filename,newfilename);
            }
        public List<FileSecurity> LoadTxtFile()
        {
            List<FileSecurity> fileHeader = new List<FileSecurity>();
            var dir = new DirectoryInfo(textBoxTXTDir.Text+@"\"+comboBoxPeriod.SelectedItem); // папка с файлами 
            var filescount = dir.GetFiles();
            //foreach (FileInfo file in dir.GetFiles()) // извлекаем все файлы и кидаем их в список 
            //{
            //    Char delimiter = '-';
            //    String[] substrings = Path.GetFileNameWithoutExtension(file.FullName).Split(delimiter);
            //    fileHeader[0].Sec = substrings[0].ToString();

            //    files.Add(Path.GetFileNameWithoutExtension(file.FullName)); // получаем полный путь к файлу и потом вычищаем ненужное, оставляем только имя файла. 
            //}
            
            for (int i = 0; i < filescount.Length; i++)
            {
                Char delimiter = '-';
                String[] substrings = Path.GetFileNameWithoutExtension(filescount[i].FullName).Split(delimiter);
                
               fileHeader.Add(new FileSecurity() { Sec = substrings[0], Dat = DateTime.Parse(substrings[1]), Per = substrings[2] });
                 
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
            if (!firststart)
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
    }
}
