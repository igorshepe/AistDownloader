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

namespace FinamDownloader
{
    public partial class Main : Form
    {
        readonly Props _props = new Props();
        public List<SecurityInfo> Security = new List<SecurityInfo>();
        public Main()
        {
            

            InitializeComponent();
            
            this.comboBoxPeriod.Items.AddRange(new object[]
            {
                (object) "тики",
                (object) "1 мин",
                (object) "5 мин",
                (object) "10 мин",
                (object) "15 мин",
                (object) "30 мин",
                (object) "1 час",
                (object) "1 день",
                (object) "1 неделя",
                (object) "1 месяц"
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

            Security = _props.Fields.Security;


            LoadTreeview();


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
            if (checkBoxDateFromTxt.Checked)
            {
                LoadTxtFile();
            }
            //FinamLoading.Download(Security);
        }

        public void SaveToFile(string data, SecurityInfo security)
        {
             
            string filename = textBoxTXTDir.Text+@"\"+security.Code+@"-"+DateTo.Day+@"."+DateTo.Month + @"."+ DateTo.Year+@".txt";
             
             
            if (filename != ""){
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                {

                    sw.WriteLine(data);

                }

            }

        }

        public void LoadTxtFile()
        {
            var fileName = Directory.GetFiles(textBoxTXTDir.Text, "*.txt");

            if (fileName.ToString() == @"{string[0]}")
            {
                MessageBox.Show(@"нет файлов");
            }
        }


        private void LoadTreeview()
        {
            var dd = Security;
            string checkname = String.Empty;
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

    }}
