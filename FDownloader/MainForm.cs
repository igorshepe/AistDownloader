// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.MainForm
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using log4net;
using log4net.Config;
using owp.FDownloader.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace owp.FDownloader
{
  public class MainForm : Form
  {
    private static readonly ILog l = LogManager.GetLogger(typeof (MainForm));
    private string settingsFileName = string.Empty;
    private Mutex onlyOne = new Mutex(false, "FDownloader 57DCD6DC-0CB3-4162-B8FF-C7A95ABF00E9");
    private IContainer components;
    private Button buttonExit;
    private Button buttonNext;
    private Button buttonPrevious;
    private Label labelOnlyOne;
    private System.Windows.Forms.Timer timerOnlyOne;
    private Label labelStarting;
    private LinkLabel linkLabel1;
    private Settings settings;
    private Page сorrentPage;

    public MainForm()
    {
      this.InitializeComponent();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(Resources.log4net);
      XmlConfigurator.Configure(xmlDocument.DocumentElement);
      MainForm.l.Debug((object) "Приложение стартовало");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.buttonExit = new Button();
      this.buttonNext = new Button();
      this.buttonPrevious = new Button();
      this.labelOnlyOne = new Label();
      this.timerOnlyOne = new System.Windows.Forms.Timer(this.components);
      this.labelStarting = new Label();
      this.linkLabel1 = new LinkLabel();
      this.SuspendLayout();
      this.buttonExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.buttonExit.Location = new Point(331, 435);
      this.buttonExit.Name = "buttonExit";
      this.buttonExit.Size = new Size(75, 23);
      this.buttonExit.TabIndex = 0;
      this.buttonExit.Text = "Выход";
      this.buttonExit.UseVisualStyleBackColor = true;
      this.buttonExit.Click += new EventHandler(this.buttonExit_Click);
      this.buttonNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.buttonNext.Enabled = false;
      this.buttonNext.Location = new Point(250, 435);
      this.buttonNext.Name = "buttonNext";
      this.buttonNext.Size = new Size(75, 23);
      this.buttonNext.TabIndex = 1;
      this.buttonNext.Text = "Далее";
      this.buttonNext.UseVisualStyleBackColor = true;
      this.buttonNext.Click += new EventHandler(this.buttonNext_Click);
      this.buttonPrevious.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.buttonPrevious.Enabled = false;
      this.buttonPrevious.Location = new Point(169, 435);
      this.buttonPrevious.Name = "buttonPrevious";
      this.buttonPrevious.Size = new Size(75, 23);
      this.buttonPrevious.TabIndex = 2;
      this.buttonPrevious.Text = "Назад";
      this.buttonPrevious.UseVisualStyleBackColor = true;
      this.buttonPrevious.Click += new EventHandler(this.buttonPrevious_Click);
      this.labelOnlyOne.AutoSize = true;
      this.labelOnlyOne.Location = new Point(13, 13);
      this.labelOnlyOne.Name = "labelOnlyOne";
      this.labelOnlyOne.Size = new Size(190, 13);
      this.labelOnlyOne.TabIndex = 3;
      this.labelOnlyOne.Text = "Запущена другая копия программы";
      this.labelOnlyOne.Visible = false;
      this.timerOnlyOne.Enabled = true;
      this.timerOnlyOne.Tick += new EventHandler(this.timerOnlyOne_Tick);
      this.labelStarting.AutoSize = true;
      this.labelStarting.Location = new Point(13, 26);
      this.labelStarting.Name = "labelStarting";
      this.labelStarting.Size = new Size(82, 13);
      this.labelStarting.TabIndex = 4;
      this.labelStarting.Text = "Загружаюсь ...";
      this.linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Location = new Point(306, 462);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new Size(100, 13);
      this.linkLabel1.TabIndex = 5;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "open-wealth-project";
      this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(409, 480);
      this.Controls.Add((Control) this.linkLabel1);
      this.Controls.Add((Control) this.labelStarting);
      this.Controls.Add((Control) this.labelOnlyOne);
      this.Controls.Add((Control) this.buttonPrevious);
      this.Controls.Add((Control) this.buttonNext);
      this.Controls.Add((Control) this.buttonExit);
      this.Name = "MainForm";
      this.Text = "owp.FDownloader";
      this.FormClosed += new FormClosedEventHandler(this.MainForm_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void buttonExit_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void SetCorrentPage(Page newCorrentPage)
    {
      if (this.сorrentPage != null)
      {
        MainForm.l.Debug((object) ("Удаляю страницу " + this.сorrentPage.GetType().ToString()));
        this.Controls.Remove((Control) this.сorrentPage);
        this.settings = this.сorrentPage.GetSetting();
      }
      this.сorrentPage = newCorrentPage;
      if (this.сorrentPage != null)
      {
        MainForm.l.Debug((object) ("Добавляю страницу " + this.сorrentPage.GetType().ToString()));
        this.Controls.Add((Control) this.сorrentPage);
        try
        {
          this.сorrentPage.SetSetting(this.settings);
        }
        catch (Exception ex)
        {
          MainForm.l.Error((object) ("Необробатываемое исключение в " + (object) this.сorrentPage.GetType() + " " + (object) ex));
        }
        this.buttonNext.Enabled = this.сorrentPage.NextExists();
        this.buttonPrevious.Enabled = this.сorrentPage.PreviousExists();
      }
      else
      {
        this.buttonNext.Enabled = false;
        this.buttonPrevious.Enabled = false;
      }
    }

    private void buttonNext_Click(object sender, EventArgs e)
    {
      this.buttonNext.Enabled = false;
      this.SetCorrentPage(this.сorrentPage.NextControl());
    }

    private void buttonPrevious_Click(object sender, EventArgs e)
    {
      this.buttonPrevious.Enabled = false;
      this.SetCorrentPage(this.сorrentPage.PreviousControl());
    }

    private void timerOnlyOne_Tick(object sender, EventArgs e)
    {
      if (this.onlyOne.WaitOne(0))
      {
        this.timerOnlyOne.Enabled = false;
        this.labelOnlyOne.Visible = false;
        this.labelStarting.Visible = true;
        bool flag1 = false;
        bool flag2 = false;
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        this.settingsFileName = Path.ChangeExtension(commandLineArgs[0], ".config.xml");
        for (int index = 1; index < commandLineArgs.Length; ++index)
        {
          if (flag2)
            this.settingsFileName = commandLineArgs[index];
          flag1 = flag1 || commandLineArgs[index].ToUpper() == "/R";
          flag2 = commandLineArgs[index].ToUpper() == "/S";
        }
        this.settings = Settings.Load(this.settingsFileName);
        this.settings.autoFlag = flag1;
        if (flag1)
          this.SetCorrentPage((Page) new DownloadPage());
        else
          this.SetCorrentPage((Page) new SettingsPage());
        this.labelStarting.Visible = false;
      }
      else
      {
        this.labelOnlyOne.Visible = true;
        this.labelStarting.Visible = false;
      }
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      this.SetCorrentPage((Page) null);
      this.settings.Save(this.settingsFileName);
      if (this.timerOnlyOne.Enabled)
        return;
      this.onlyOne.ReleaseMutex();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://code.google.com/p/open-wealth-project/wiki/FDownloader");
    }
  }
}
