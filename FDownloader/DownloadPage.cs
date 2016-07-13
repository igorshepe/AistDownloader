// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.DownloadPage
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace owp.FDownloader
{
  public class DownloadPage : Page
  {
    private static readonly ILog l = LogManager.GetLogger(typeof (DownloadPage));
    private IContainer components;
    private BackgroundWorker backgroundWorker;
    private TextBox textBox;
    private ProgressBar progressBar;

    public DownloadPage()
    {
      this.InitializeComponent();
    }

    public DownloadPage(Page previous)
    {
      this.InitializeComponent();
      this.previous = previous;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.backgroundWorker = new BackgroundWorker();
      this.textBox = new TextBox();
      this.progressBar = new ProgressBar();
      this.SuspendLayout();
      this.backgroundWorker.WorkerReportsProgress = true;
      this.backgroundWorker.WorkerSupportsCancellation = true;
      this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
      this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
      this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
      this.textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.textBox.Location = new Point(12, 3);
      this.textBox.Multiline = true;
      this.textBox.Name = "textBox";
      this.textBox.ReadOnly = true;
      this.textBox.ScrollBars = ScrollBars.Vertical;
      this.textBox.Size = new Size(378, 401);
      this.textBox.TabIndex = 0;
      this.progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.progressBar.Location = new Point(12, 410);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new Size(378, 23);
      this.progressBar.TabIndex = 1;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.progressBar);
      this.Controls.Add((Control) this.textBox);
      this.Name = "DownloadPage";
      this.Size = new Size(402, 436);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public override void SetSetting(Settings settings)
    {
      base.SetSetting(settings);
      if (!this.backgroundWorker.IsBusy)
        this.backgroundWorker.RunWorkerAsync();
      else
        DownloadPage.l.Error((object) "backgroundWorker.IsBusy");
    }

    public override Settings GetSetting()
    {
      DownloadPage.l.Debug((object) "Останавливаю backgroundWorker");
      this.backgroundWorker.CancelAsync();
      while (this.backgroundWorker.IsBusy)
      {
        DownloadPage.l.Debug((object) "Жду освобождения backgroundWorker");
        Thread.Sleep(100);
        Application.DoEvents();
      }
      return this.settings;
    }

    private string VerifyFileName(string FN)
    {
      StringBuilder stringBuilder = new StringBuilder(FN);
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        stringBuilder.Replace(invalidFileNameChar.ToString(), string.Empty);
      return stringBuilder.ToString();
    }

    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      DownloadPage.l.Debug((object) "стартовал backgroundWorker_DoWork");
      Random random = new Random(DateTime.Now.Millisecond);
      List<Bars> barsList = new List<Bars>();
      this.backgroundWorker.ReportProgress(0, (object) "Формирую список эмитентов для обработки");
      DownloadPage.l.Debug((object) "Формирую список эмитентов для обработки");
      foreach (EmitentInfo emitent in this.settings.Emitents)
      {
        if (this.backgroundWorker.CancellationPending)
        {
          e.Cancel = true;
          return;
        }
        if (emitent.checed)
          barsList.Add(new Bars(emitent));
      }
      for (int index = 0; index < barsList.Count; ++index)
      {
        Bars bars = barsList[index];
        this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) ("==== Работаю с " + bars.emitent.code));
        this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Загружаю существующие TXT файлы");
        DownloadPage.l.Debug((object) ("==== Работаю с " + bars.emitent.code));
        DownloadPage.l.Debug((object) "Загружаю существующие TXT файлы");
        if (Directory.Exists(Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period))))
        {
          string path = Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period));
          string searchPattern = this.VerifyFileName(bars.emitent.marketName) + (object) '-' + bars.emitent.code + "-*.txt";
          foreach (string file in Directory.GetFiles(path, searchPattern))
          {
            if (this.backgroundWorker.CancellationPending)
            {
              e.Cancel = true;
              return;
            }
            if (File.Exists(file))
            {
              StreamReader streamReader = new StreamReader(file);
              bars.LoadCSV((TextReader) streamReader);
              streamReader.Close();
            }
          }
        }
        if (this.settings.period != 1)
        {
          this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Загружаю существующие WL файлы");
          DownloadPage.l.Debug((object) "Загружаю существующие WL файлы");
          if (this.backgroundWorker.CancellationPending)
          {
            e.Cancel = true;
            return;
          }
          bars.LoadWL(Path.Combine(Path.Combine(Path.Combine(this.settings.wlDir, FinamDataScale.ToString(this.settings.period)), this.VerifyFileName(bars.emitent.marketName)), bars.emitent.code + ".wl"));
        }
        if (this.settings.loadFromFinam)
        {
          this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Загружаю CSV с финама");
          DownloadPage.l.Debug((object) "Загружаю CSV с финама");
          if (this.backgroundWorker.CancellationPending)
          {
            e.Cancel = true;
            return;
          }
          if (this.settings.fromCSV)
            this.settings.from = bars.Last;
          DateTime from = this.settings.from;
          int days = (this.settings.to - this.settings.from).Days;
          int num1 = 0;
label_55:
          if (num1 <= days)
          {
            if (this.settings.period == 1)
            {
              this.settings.from = from.AddDays((double) num1);
              this.settings.to = from.AddDays((double) num1);
            }
            else
              num1 = 2147483646;
            this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) ("Загружаю " + bars.emitent.code + " " + this.settings.from.ToShortDateString() + "-" + this.settings.to.ToShortDateString()));
            DownloadPage.l.Debug((object) ("Загружаю " + bars.emitent.code + " " + this.settings.from.ToShortDateString() + "-" + this.settings.to.ToShortDateString()));
            while (!this.backgroundWorker.CancellationPending)
            {
              string str1;
              try
              {
                str1 = FinamHelper.Download(this.settings, bars.emitent);
              }
              catch
              {
                DownloadPage.l.Error((object) "Необробатываемый Exception в FinamHelper.Download");
                str1 = "Exception";
              }
              if (str1 == "Система уже обрабатывает Ваш запрос. Дождитесь окончания обработки.")
              {
                this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Финам просит подождать");
                Thread.Sleep(random.Next(30000));
                if (this.backgroundWorker.CancellationPending)
                {
                  e.Cancel = true;
                  return;
                }
                this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Пробую снова");
              }
              if (str1 == "Exception")
              {
                this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Ошибка при скачивании");
                Thread.Sleep(random.Next(30000));
                if (this.backgroundWorker.CancellationPending)
                {
                  e.Cancel = true;
                  return;
                }
                this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Пробую снова");
              }
              if (!(str1 == "Система уже обрабатывает Ваш запрос. Дождитесь окончания обработки.") && !(str1 == "Exception"))
              {
                if (str1 == string.Empty || str1.Length < 30)
                {
                  this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "нет данных");
                  DownloadPage.l.Debug((object) "нет данных");
                  if (this.backgroundWorker.CancellationPending)
                  {
                    e.Cancel = true;
                    return;
                  }
                }
                else
                {
                  this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Анализирую");
                  DownloadPage.l.Debug((object) "Анализирую");
                  StringReader stringReader = new StringReader(str1);
                  bars.LoadCSV((TextReader) stringReader);
                  stringReader.Close();
                  if (!this.settings.margeCsv && !this.settings.delCSV)
                  {
                    this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Сохраняю загруженный txt");
                    DownloadPage.l.Debug((object) "Сохраняю загруженный txt");
                    string str2 = Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period));
                    string str3 = this.VerifyFileName(bars.emitent.marketName) + (object) '-' + bars.emitent.code + (object) '-' + this.settings.to.ToString("yyyyMMdd");
                    if (!Directory.Exists(str2))
                      Directory.CreateDirectory(str2);
                    int num2 = 0;
                    object[] objArray;
                    for (; File.Exists(Path.Combine(str2, str3 + ".txt")); str3 = string.Concat(objArray))
                    {
                      ++num2;
                      objArray = new object[6]
                      {
                        (object) this.VerifyFileName(bars.emitent.marketName),
                        (object) '-',
                        (object) bars.emitent.code,
                        (object) '(',
                        (object) num2,
                        (object) ')'
                      };
                    }
                    File.WriteAllText(Path.Combine(str2, str3 + ".txt"), str1);
                  }
                }
                Thread.Sleep(500);
                ++num1;
                goto label_55;
              }
            }
            e.Cancel = true;
            return;
          }
          this.settings.from = from;
          this.settings.to = from.AddDays((double) days);
        }
        if (this.settings.convertCSV2WL)
        {
          this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Сохраняю WL файлы");
          DownloadPage.l.Debug((object) "Сохраняю WL файлы");
          if (this.backgroundWorker.CancellationPending)
          {
            e.Cancel = true;
            return;
          }
          bars.Save(Path.Combine(Path.Combine(Path.Combine(this.settings.wlDir, FinamDataScale.ToString(this.settings.period)), this.VerifyFileName(bars.emitent.marketName)), bars.emitent.code + ".wl"));
        }
        if (this.settings.margeCsv || this.settings.delCSV)
        {
          this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Удаляю CSV файлы");
          DownloadPage.l.Debug((object) "Удаляю CSV файлы");
          if (Directory.Exists(Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period))))
          {
            string path = Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period));
            string searchPattern = this.VerifyFileName(bars.emitent.marketName) + (object) '-' + bars.emitent.code + "(*).txt";
            foreach (string file in Directory.GetFiles(path, searchPattern))
              File.Delete(file);
          }
        }
        if (this.settings.margeCsv)
        {
          this.backgroundWorker.ReportProgress(100 * index / barsList.Count, (object) "Сохраняю объедененные CSV файлы");
          DownloadPage.l.Debug((object) "Сохраняю объедененные CSV файлы");
          if (this.backgroundWorker.CancellationPending)
          {
            e.Cancel = true;
            return;
          }
          bars.SaveCSV(Path.Combine(Path.Combine(this.settings.csvDir, FinamDataScale.ToString(this.settings.period)), this.VerifyFileName(bars.emitent.marketName) + (object) '-' + bars.emitent.code + ".txt"));
        }
        bars.Clear();
        Thread.Sleep(500);
      }
      this.backgroundWorker.ReportProgress(100, (object) "Всё!!!");
      DownloadPage.l.Info((object) "backgroundWorker_DoWork закончил");
    }

    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        int num = (int) MessageBox.Show(e.Error.Message);
        DownloadPage.l.Error((object) ("Ошибка в backgroundWorker " + (object) e));
      }
      else if (e.Cancelled)
      {
        TextBox textBox = this.textBox;
        string str = textBox.Text + "Отменено пользователем" + Environment.NewLine;
        textBox.Text = str;
        DownloadPage.l.Info((object) "Отменено пользователем");
      }
      else
      {
        if (!this.settings.autoFlag)
          return;
        Application.Exit();
      }
    }

    private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.UserState as string))
      {
        TextBox textBox = this.textBox;
        string str = textBox.Text + (e.UserState as string) + Environment.NewLine;
        textBox.Text = str;
        DownloadPage.l.Debug((object) ("backgroundWorker_ProgressChanged " + (e.UserState as string)));
      }
      this.textBox.SelectionStart = this.textBox.TextLength;
      this.textBox.ScrollToCaret();
      this.progressBar.Value = e.ProgressPercentage;
    }
  }
}
