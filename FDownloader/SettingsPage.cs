// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.SettingsPage
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace owp.FDownloader
{
  public class SettingsPage : Page
  {
    private IContainer components;
    private CheckBox checkBoxProxy;
    private TextBox textBoxProxy;
    private Label label1;
    private CheckBox checkBoxProxyPassword;
    private TextBox textBoxLogin;
    private TextBox textBoxPassword;
    private Label label2;
    private Label label3;
    private OpenFileDialog openFileDialogLoadSetting;
    private Button buttonLoadSetting;
    private GroupBox groupBoxProxy;
    private GroupBox groupBoxLoadSetting;
    private Label label5;
    private Label label4;
    private CheckBox checkBoxFromCSV;
    private CheckBox checkBoxYesterday;
    private CheckBox checkBoxToday;
    private DateTimePicker dateTimePickerTo;
    private DateTimePicker dateTimePickerFrom;
    private CheckBox checkBoxMargeCSV;
    private CheckBox checkBoxLoadFromFinam;
    private CheckBox checkBoxConvertCSV2WL;
    private Button buttonCSVDir;
    private Label labelCSVDir;
    private TextBox textBoxCSVDir;
    private Button buttonWLDir;
    private Label labelWLDir;
    private TextBox textBoxWLDir;
    private CheckBox checkBoxDelCSV;
    private FolderBrowserDialog folderBrowserDialog1;
    private Label label6;
    private ComboBox comboBoxPeriod;
    private ComboBox aggregateComboBox;
    private CheckBox aggregateСheckBox;
    private NumericUpDown aggregateNumericUpDown;

    public SettingsPage()
    {
      this.InitializeComponent();
      this.next = (Page) new FinamTreeViewPage((Page) this);
    }

    public override void SetSetting(Settings settings)
    {
      base.SetSetting(settings);
      if (settings == null)
        return;
      this.checkBoxProxy.Checked = settings.useProxy;
      this.textBoxProxy.Text = settings.proxy;
      this.checkBoxProxyPassword.Checked = settings.proxyWithPassword;
      this.textBoxLogin.Text = settings.proxyUser;
      this.textBoxPassword.Text = settings.proxyPassword;
      this.dateTimePickerFrom.Value = !(settings.from > this.dateTimePickerFrom.MinDate) || !(settings.from < this.dateTimePickerFrom.MaxDate) ? DateTime.Today : settings.from;
      this.dateTimePickerTo.Value = !(settings.to > this.dateTimePickerTo.MinDate) || !(settings.to < this.dateTimePickerTo.MaxDate) ? DateTime.Today : settings.to;
      if (settings.period > 0)
        this.comboBoxPeriod.SelectedIndex = settings.period - 1;
      else
        this.comboBoxPeriod.SelectedIndex = 6;
      this.checkBoxLoadFromFinam.Checked = settings.loadFromFinam;
      this.checkBoxFromCSV.Checked = settings.fromCSV;
      this.checkBoxYesterday.Checked = settings.fromYesterday;
      this.checkBoxToday.Checked = settings.toToday;
      if (settings.fromYesterday)
        this.dateTimePickerFrom.Value = DateTime.Today.AddDays(-1.0);
      if (settings.toToday)
        this.dateTimePickerTo.Value = DateTime.Today;
      this.textBoxCSVDir.Text = settings.csvDir;
      this.checkBoxMargeCSV.Checked = settings.margeCsv;
      this.checkBoxConvertCSV2WL.Checked = settings.convertCSV2WL;
      this.checkBoxDelCSV.Checked = settings.delCSV;
      this.textBoxWLDir.Text = settings.wlDir;
    }

    public override Settings GetSetting()
    {
      this.settings.useProxy = this.checkBoxProxy.Checked;
      this.settings.proxy = this.textBoxProxy.Text;
      this.settings.proxyWithPassword = this.checkBoxProxyPassword.Checked;
      this.settings.proxyUser = this.textBoxLogin.Text;
      this.settings.proxyPassword = this.textBoxPassword.Text;
      this.settings.loadFromFinam = this.checkBoxLoadFromFinam.Checked;
      this.settings.fromYesterday = this.checkBoxYesterday.Checked;
      this.settings.fromCSV = this.checkBoxFromCSV.Checked;
      this.settings.toToday = this.checkBoxToday.Checked;
      this.settings.from = this.dateTimePickerFrom.Value;
      this.settings.to = this.dateTimePickerTo.Value;
      this.settings.period = this.comboBoxPeriod.SelectedIndex + 1;
      this.settings.csvDir = this.textBoxCSVDir.Text;
      this.settings.margeCsv = this.checkBoxMargeCSV.Checked;
      this.settings.convertCSV2WL = this.checkBoxConvertCSV2WL.Checked;
      this.settings.delCSV = this.checkBoxDelCSV.Checked;
      this.settings.wlDir = this.textBoxWLDir.Text;
      return base.GetSetting();
    }

    private void checkBoxProxy_CheckedChanged(object sender, EventArgs e)
    {
      this.groupBoxProxy.Enabled = this.checkBoxProxy.Checked;
    }

    private void checkBoxProxyPassword_CheckedChanged(object sender, EventArgs e)
    {
      this.label2.Enabled = this.checkBoxProxyPassword.Checked && this.checkBoxProxyPassword.Enabled;
      this.label3.Enabled = this.checkBoxProxyPassword.Checked && this.checkBoxProxyPassword.Enabled;
      this.textBoxLogin.Enabled = this.checkBoxProxyPassword.Checked && this.checkBoxProxyPassword.Enabled;
      this.textBoxPassword.Enabled = this.checkBoxProxyPassword.Checked && this.checkBoxProxyPassword.Enabled;
    }

    private void checkBoxYesterday_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBoxYesterday.Checked)
      {
        this.checkBoxFromCSV.Checked = false;
        this.dateTimePickerFrom.Enabled = false;
        this.dateTimePickerFrom.Value = DateTime.Today.AddDays(-1.0);
      }
      else
        this.dateTimePickerFrom.Enabled = !this.checkBoxFromCSV.Checked;
    }

    private void checkBoxFromCSV_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBoxFromCSV.Checked)
      {
        this.checkBoxYesterday.Checked = false;
        this.dateTimePickerFrom.Enabled = false;
        this.dateTimePickerFrom.Value = this.dateTimePickerFrom.MinDate;
      }
      else
        this.dateTimePickerFrom.Enabled = !this.checkBoxYesterday.Checked;
      if (!this.dateTimePickerFrom.Enabled)
        return;
      this.dateTimePickerFrom.Value = DateTime.Today.AddDays(-1.0);
    }

    private void checkBoxToday_CheckedChanged(object sender, EventArgs e)
    {
      this.dateTimePickerTo.Enabled = !this.checkBoxToday.Checked;
      this.dateTimePickerTo.Value = DateTime.Today;
    }

    private void checkBoxOnlyConvert_CheckedChanged(object sender, EventArgs e)
    {
      this.groupBoxLoadSetting.Enabled = this.checkBoxLoadFromFinam.Checked;
    }

    private void checkBoxDelCSV_CheckedChanged(object sender, EventArgs e)
    {
      this.checkBoxMargeCSV.Enabled = !this.checkBoxDelCSV.Checked;
    }

    private void buttonLoadSetting_Click(object sender, EventArgs e)
    {
      this.openFileDialogLoadSetting.InitialDirectory = Environment.CurrentDirectory;
      if (this.openFileDialogLoadSetting.ShowDialog() != DialogResult.OK)
        return;
      this.SetSetting(Settings.Load(this.openFileDialogLoadSetting.FileName));
    }

    private void buttonCSVDir_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
      if (DialogResult.OK != this.folderBrowserDialog1.ShowDialog())
        return;
      this.textBoxCSVDir.Text = this.folderBrowserDialog1.SelectedPath;
    }

    private void buttonWLDir_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
      if (DialogResult.OK != this.folderBrowserDialog1.ShowDialog())
        return;
      this.textBoxWLDir.Text = this.folderBrowserDialog1.SelectedPath;
    }

    private void comboBoxPeriod_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.comboBoxPeriod.SelectedIndex == 0)
      {
        this.checkBoxDelCSV.Checked = false;
        this.checkBoxDelCSV.Enabled = false;
      }
      else
        this.checkBoxDelCSV.Enabled = true;
    }

    private void checkBoxConvertCSV2WL_CheckedChanged(object sender, EventArgs e)
    {
      this.checkBoxDelCSV.Enabled = this.checkBoxDelCSV.Checked = this.textBoxWLDir.Enabled = this.labelWLDir.Enabled = this.buttonWLDir.Enabled = this.checkBoxConvertCSV2WL.Checked;
      this.comboBoxPeriod_SelectedIndexChanged(sender, e);
    }

    private void checkBoxMargeCSV_CheckedChanged(object sender, EventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.checkBoxProxy = new CheckBox();
      this.textBoxProxy = new TextBox();
      this.label1 = new Label();
      this.checkBoxProxyPassword = new CheckBox();
      this.textBoxLogin = new TextBox();
      this.textBoxPassword = new TextBox();
      this.label2 = new Label();
      this.label3 = new Label();
      this.openFileDialogLoadSetting = new OpenFileDialog();
      this.buttonLoadSetting = new Button();
      this.groupBoxProxy = new GroupBox();
      this.groupBoxLoadSetting = new GroupBox();
      this.label5 = new Label();
      this.label4 = new Label();
      this.checkBoxFromCSV = new CheckBox();
      this.checkBoxYesterday = new CheckBox();
      this.checkBoxToday = new CheckBox();
      this.dateTimePickerTo = new DateTimePicker();
      this.dateTimePickerFrom = new DateTimePicker();
      this.checkBoxMargeCSV = new CheckBox();
      this.checkBoxLoadFromFinam = new CheckBox();
      this.checkBoxConvertCSV2WL = new CheckBox();
      this.buttonCSVDir = new Button();
      this.labelCSVDir = new Label();
      this.textBoxCSVDir = new TextBox();
      this.buttonWLDir = new Button();
      this.labelWLDir = new Label();
      this.textBoxWLDir = new TextBox();
      this.checkBoxDelCSV = new CheckBox();
      this.folderBrowserDialog1 = new FolderBrowserDialog();
      this.label6 = new Label();
      this.comboBoxPeriod = new ComboBox();
      this.aggregateComboBox = new ComboBox();
      this.aggregateСheckBox = new CheckBox();
      this.aggregateNumericUpDown = new NumericUpDown();
      this.groupBoxProxy.SuspendLayout();
      this.groupBoxLoadSetting.SuspendLayout();
      this.aggregateNumericUpDown.BeginInit();
      this.SuspendLayout();
      this.checkBoxProxy.AutoSize = true;
      this.checkBoxProxy.Location = new Point(10, 38);
      this.checkBoxProxy.Name = "checkBoxProxy";
      this.checkBoxProxy.Size = new Size(138, 17);
      this.checkBoxProxy.TabIndex = 0;
      this.checkBoxProxy.Text = "Использовать прокси";
      this.checkBoxProxy.UseVisualStyleBackColor = true;
      this.checkBoxProxy.CheckedChanged += new EventHandler(this.checkBoxProxy_CheckedChanged);
      this.textBoxProxy.Location = new Point(13, 22);
      this.textBoxProxy.Name = "textBoxProxy";
      this.textBoxProxy.Size = new Size(168, 20);
      this.textBoxProxy.TabIndex = 1;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(187, 25);
      this.label1.Name = "label1";
      this.label1.Size = new Size(159, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Пример: http://ProxyHost:8080";
      this.checkBoxProxyPassword.AutoSize = true;
      this.checkBoxProxyPassword.Location = new Point(13, 48);
      this.checkBoxProxyPassword.Name = "checkBoxProxyPassword";
      this.checkBoxProxyPassword.Size = new Size(145, 17);
      this.checkBoxProxyPassword.TabIndex = 3;
      this.checkBoxProxyPassword.Text = "Прокси требует пароль";
      this.checkBoxProxyPassword.UseVisualStyleBackColor = true;
      this.checkBoxProxyPassword.CheckedChanged += new EventHandler(this.checkBoxProxyPassword_CheckedChanged);
      this.textBoxLogin.Enabled = false;
      this.textBoxLogin.Location = new Point(63, 71);
      this.textBoxLogin.Name = "textBoxLogin";
      this.textBoxLogin.Size = new Size(168, 20);
      this.textBoxLogin.TabIndex = 4;
      this.textBoxPassword.Enabled = false;
      this.textBoxPassword.Location = new Point(63, 97);
      this.textBoxPassword.Name = "textBoxPassword";
      this.textBoxPassword.PasswordChar = '*';
      this.textBoxPassword.Size = new Size(168, 20);
      this.textBoxPassword.TabIndex = 5;
      this.label2.AutoSize = true;
      this.label2.Enabled = false;
      this.label2.Location = new Point(10, 74);
      this.label2.Name = "label2";
      this.label2.Size = new Size(38, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Логин";
      this.label3.AutoSize = true;
      this.label3.Enabled = false;
      this.label3.Location = new Point(10, 100);
      this.label3.Name = "label3";
      this.label3.Size = new Size(45, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Пароль";
      this.buttonLoadSetting.Location = new Point(10, 7);
      this.buttonLoadSetting.Name = "buttonLoadSetting";
      this.buttonLoadSetting.Size = new Size(220, 23);
      this.buttonLoadSetting.TabIndex = 8;
      this.buttonLoadSetting.Text = "Загрузить настройки из файла";
      this.buttonLoadSetting.UseVisualStyleBackColor = true;
      this.groupBoxProxy.Controls.Add((Control) this.label1);
      this.groupBoxProxy.Controls.Add((Control) this.textBoxProxy);
      this.groupBoxProxy.Controls.Add((Control) this.textBoxLogin);
      this.groupBoxProxy.Controls.Add((Control) this.label3);
      this.groupBoxProxy.Controls.Add((Control) this.checkBoxProxyPassword);
      this.groupBoxProxy.Controls.Add((Control) this.label2);
      this.groupBoxProxy.Controls.Add((Control) this.textBoxPassword);
      this.groupBoxProxy.Enabled = false;
      this.groupBoxProxy.Location = new Point(10, 59);
      this.groupBoxProxy.Name = "groupBoxProxy";
      this.groupBoxProxy.Size = new Size(385, 130);
      this.groupBoxProxy.TabIndex = 9;
      this.groupBoxProxy.TabStop = false;
      this.groupBoxProxy.Text = "Прокси";
      this.groupBoxLoadSetting.Controls.Add((Control) this.label5);
      this.groupBoxLoadSetting.Controls.Add((Control) this.label4);
      this.groupBoxLoadSetting.Controls.Add((Control) this.checkBoxFromCSV);
      this.groupBoxLoadSetting.Controls.Add((Control) this.checkBoxYesterday);
      this.groupBoxLoadSetting.Controls.Add((Control) this.checkBoxToday);
      this.groupBoxLoadSetting.Controls.Add((Control) this.dateTimePickerTo);
      this.groupBoxLoadSetting.Controls.Add((Control) this.dateTimePickerFrom);
      this.groupBoxLoadSetting.Enabled = false;
      this.groupBoxLoadSetting.Location = new Point(10, 260);
      this.groupBoxLoadSetting.Name = "groupBoxLoadSetting";
      this.groupBoxLoadSetting.Size = new Size(385, 73);
      this.groupBoxLoadSetting.TabIndex = 10;
      this.groupBoxLoadSetting.TabStop = false;
      this.groupBoxLoadSetting.Text = "Параметры загрузки";
      this.label5.AutoSize = true;
      this.label5.Enabled = false;
      this.label5.Location = new Point(7, 49);
      this.label5.Name = "label5";
      this.label5.Size = new Size(19, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "по";
      this.label4.AutoSize = true;
      this.label4.Enabled = false;
      this.label4.Location = new Point(7, 19);
      this.label4.Name = "label4";
      this.label4.Size = new Size(13, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "с";
      this.checkBoxFromCSV.AutoSize = true;
      this.checkBoxFromCSV.Checked = true;
      this.checkBoxFromCSV.CheckState = CheckState.Checked;
      this.checkBoxFromCSV.Location = new Point(237, 22);
      this.checkBoxFromCSV.Name = "checkBoxFromCSV";
      this.checkBoxFromCSV.Size = new Size(143, 17);
      this.checkBoxFromCSV.TabIndex = 5;
      this.checkBoxFromCSV.Text = "Дата из CSV и/или WL";
      this.checkBoxFromCSV.UseVisualStyleBackColor = true;
      this.checkBoxFromCSV.CheckedChanged += new EventHandler(this.checkBoxFromCSV_CheckedChanged);
      this.checkBoxYesterday.AutoSize = true;
      this.checkBoxYesterday.Location = new Point(175, 22);
      this.checkBoxYesterday.Name = "checkBoxYesterday";
      this.checkBoxYesterday.Size = new Size(56, 17);
      this.checkBoxYesterday.TabIndex = 4;
      this.checkBoxYesterday.Text = "Вчера";
      this.checkBoxYesterday.UseVisualStyleBackColor = true;
      this.checkBoxYesterday.CheckedChanged += new EventHandler(this.checkBoxYesterday_CheckedChanged);
      this.checkBoxToday.AutoSize = true;
      this.checkBoxToday.Checked = true;
      this.checkBoxToday.CheckState = CheckState.Checked;
      this.checkBoxToday.Location = new Point(175, 48);
      this.checkBoxToday.Name = "checkBoxToday";
      this.checkBoxToday.Size = new Size(68, 17);
      this.checkBoxToday.TabIndex = 3;
      this.checkBoxToday.Text = "Сегодня";
      this.checkBoxToday.UseVisualStyleBackColor = true;
      this.checkBoxToday.CheckedChanged += new EventHandler(this.checkBoxToday_CheckedChanged);
      this.dateTimePickerTo.Enabled = false;
      this.dateTimePickerTo.Location = new Point(48, 45);
      this.dateTimePickerTo.Name = "dateTimePickerTo";
      this.dateTimePickerTo.Size = new Size(122, 20);
      this.dateTimePickerTo.TabIndex = 1;
      this.dateTimePickerFrom.Enabled = false;
      this.dateTimePickerFrom.Location = new Point(48, 19);
      this.dateTimePickerFrom.Name = "dateTimePickerFrom";
      this.dateTimePickerFrom.Size = new Size(122, 20);
      this.dateTimePickerFrom.TabIndex = 0;
      this.checkBoxMargeCSV.AutoSize = true;
      this.checkBoxMargeCSV.Checked = true;
      this.checkBoxMargeCSV.CheckState = CheckState.Checked;
      this.checkBoxMargeCSV.Location = new Point(17, 361);
      this.checkBoxMargeCSV.Name = "checkBoxMargeCSV";
      this.checkBoxMargeCSV.Size = new Size(234, 17);
      this.checkBoxMargeCSV.TabIndex = 0;
      this.checkBoxMargeCSV.Text = "Объединять CSV по одному инструменту";
      this.checkBoxMargeCSV.UseVisualStyleBackColor = true;
      this.checkBoxMargeCSV.CheckedChanged += new EventHandler(this.checkBoxMargeCSV_CheckedChanged);
      this.checkBoxLoadFromFinam.AutoSize = true;
      this.checkBoxLoadFromFinam.Location = new Point(10, 239);
      this.checkBoxLoadFromFinam.Name = "checkBoxLoadFromFinam";
      this.checkBoxLoadFromFinam.Size = new Size(164, 17);
      this.checkBoxLoadFromFinam.TabIndex = 12;
      this.checkBoxLoadFromFinam.Text = "Загрузить CSV c ФИНАМА";
      this.checkBoxLoadFromFinam.UseVisualStyleBackColor = true;
      this.checkBoxLoadFromFinam.CheckedChanged += new EventHandler(this.checkBoxOnlyConvert_CheckedChanged);
      this.checkBoxConvertCSV2WL.AutoSize = true;
      this.checkBoxConvertCSV2WL.Checked = true;
      this.checkBoxConvertCSV2WL.CheckState = CheckState.Checked;
      this.checkBoxConvertCSV2WL.Location = new Point(17, 384);
      this.checkBoxConvertCSV2WL.Name = "checkBoxConvertCSV2WL";
      this.checkBoxConvertCSV2WL.Size = new Size(158, 17);
      this.checkBoxConvertCSV2WL.TabIndex = 13;
      this.checkBoxConvertCSV2WL.Text = "Преобразовать CSV в WL";
      this.checkBoxConvertCSV2WL.UseVisualStyleBackColor = true;
      this.checkBoxConvertCSV2WL.CheckedChanged += new EventHandler(this.checkBoxConvertCSV2WL_CheckedChanged);
      this.buttonCSVDir.Location = new Point(355, 337);
      this.buttonCSVDir.Name = "buttonCSVDir";
      this.buttonCSVDir.Size = new Size(31, 19);
      this.buttonCSVDir.TabIndex = 16;
      this.buttonCSVDir.Text = "...";
      this.buttonCSVDir.UseVisualStyleBackColor = true;
      this.buttonCSVDir.Click += new EventHandler(this.buttonCSVDir_Click);
      this.labelCSVDir.AutoSize = true;
      this.labelCSVDir.Location = new Point(14, 340);
      this.labelCSVDir.Name = "labelCSVDir";
      this.labelCSVDir.Size = new Size(84, 13);
      this.labelCSVDir.TabIndex = 15;
      this.labelCSVDir.Text = "Папка для CSV";
      this.textBoxCSVDir.Location = new Point(104, 337);
      this.textBoxCSVDir.Name = "textBoxCSVDir";
      this.textBoxCSVDir.Size = new Size(245, 20);
      this.textBoxCSVDir.TabIndex = 14;
      this.buttonWLDir.Location = new Point(355, 410);
      this.buttonWLDir.Name = "buttonWLDir";
      this.buttonWLDir.Size = new Size(31, 19);
      this.buttonWLDir.TabIndex = 19;
      this.buttonWLDir.Text = "...";
      this.buttonWLDir.UseVisualStyleBackColor = true;
      this.buttonWLDir.Click += new EventHandler(this.buttonWLDir_Click);
      this.labelWLDir.AutoSize = true;
      this.labelWLDir.Location = new Point(14, 410);
      this.labelWLDir.Name = "labelWLDir";
      this.labelWLDir.Size = new Size(80, 13);
      this.labelWLDir.TabIndex = 18;
      this.labelWLDir.Text = "Папка для WL";
      this.textBoxWLDir.Location = new Point(104, 407);
      this.textBoxWLDir.Name = "textBoxWLDir";
      this.textBoxWLDir.Size = new Size(245, 20);
      this.textBoxWLDir.TabIndex = 17;
      this.checkBoxDelCSV.AutoSize = true;
      this.checkBoxDelCSV.Checked = true;
      this.checkBoxDelCSV.CheckState = CheckState.Checked;
      this.checkBoxDelCSV.Location = new Point(182, 384);
      this.checkBoxDelCSV.Name = "checkBoxDelCSV";
      this.checkBoxDelCSV.Size = new Size(213, 17);
      this.checkBoxDelCSV.TabIndex = 20;
      this.checkBoxDelCSV.Text = "Удалить CSV после преобразования";
      this.checkBoxDelCSV.UseVisualStyleBackColor = true;
      this.checkBoxDelCSV.CheckedChanged += new EventHandler(this.checkBoxDelCSV_CheckedChanged);
      this.label6.AutoSize = true;
      this.label6.Location = new Point(7, 196);
      this.label6.Name = "label6";
      this.label6.Size = new Size(60, 13);
      this.label6.TabIndex = 22;
      this.label6.Text = "Работаю с";
      this.comboBoxPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxPeriod.FormattingEnabled = true;
      this.comboBoxPeriod.Items.AddRange(new object[11]
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
        (object) "1 месяц",
        (object) "1 час (с 10-30)"
      });
      this.comboBoxPeriod.Location = new Point(70, 192);
      this.comboBoxPeriod.Name = "comboBoxPeriod";
      this.comboBoxPeriod.Size = new Size(121, 21);
      this.comboBoxPeriod.TabIndex = 21;
      this.comboBoxPeriod.SelectedIndexChanged += new EventHandler(this.comboBoxPeriod_SelectedIndexChanged);
      this.aggregateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.aggregateComboBox.Enabled = false;
      this.aggregateComboBox.FormattingEnabled = true;
      this.aggregateComboBox.Items.AddRange(new object[7]
      {
        (object) "Тикам",
        (object) "Секундам",
        (object) "Минутам",
        (object) "Дням",
        (object) "Неделям",
        (object) "Месяцам",
        (object) "Объемам"
      });
      this.aggregateComboBox.Location = new Point(265, 217);
      this.aggregateComboBox.Name = "aggregateComboBox";
      this.aggregateComboBox.Size = new Size(121, 21);
      this.aggregateComboBox.TabIndex = 23;
      this.aggregateСheckBox.AutoSize = true;
      this.aggregateСheckBox.Enabled = false;
      this.aggregateСheckBox.Location = new Point(10, 217);
      this.aggregateСheckBox.Name = "aggregateСheckBox";
      this.aggregateСheckBox.Size = new Size(187, 17);
      this.aggregateСheckBox.TabIndex = 24;
      this.aggregateСheckBox.Text = "Дополнительно агригировать к";
      this.aggregateСheckBox.UseVisualStyleBackColor = true;
      this.aggregateNumericUpDown.Enabled = false;
      this.aggregateNumericUpDown.Location = new Point(204, 217);
      this.aggregateNumericUpDown.Name = "aggregateNumericUpDown";
      this.aggregateNumericUpDown.Size = new Size(55, 20);
      this.aggregateNumericUpDown.TabIndex = 25;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.aggregateNumericUpDown);
      this.Controls.Add((Control) this.aggregateСheckBox);
      this.Controls.Add((Control) this.aggregateComboBox);
      this.Controls.Add((Control) this.label6);
      this.Controls.Add((Control) this.comboBoxPeriod);
      this.Controls.Add((Control) this.checkBoxDelCSV);
      this.Controls.Add((Control) this.buttonWLDir);
      this.Controls.Add((Control) this.labelWLDir);
      this.Controls.Add((Control) this.textBoxWLDir);
      this.Controls.Add((Control) this.buttonCSVDir);
      this.Controls.Add((Control) this.labelCSVDir);
      this.Controls.Add((Control) this.textBoxCSVDir);
      this.Controls.Add((Control) this.checkBoxMargeCSV);
      this.Controls.Add((Control) this.checkBoxConvertCSV2WL);
      this.Controls.Add((Control) this.checkBoxLoadFromFinam);
      this.Controls.Add((Control) this.groupBoxLoadSetting);
      this.Controls.Add((Control) this.checkBoxProxy);
      this.Controls.Add((Control) this.groupBoxProxy);
      this.Controls.Add((Control) this.buttonLoadSetting);
      this.Name = "SettingsPage";
      this.Size = new Size(402, 436);
      this.groupBoxProxy.ResumeLayout(false);
      this.groupBoxProxy.PerformLayout();
      this.groupBoxLoadSetting.ResumeLayout(false);
      this.groupBoxLoadSetting.PerformLayout();
      this.aggregateNumericUpDown.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
