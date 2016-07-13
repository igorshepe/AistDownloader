// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.FinamTreeViewPage
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace owp.FDownloader
{
  public class FinamTreeViewPage : Page
  {
    private IContainer components;
    private Button buttonRefresh;
    private FinamTreeView finamTreeView;

    public FinamTreeViewPage()
    {
      this.InitializeComponent();
    }

    public FinamTreeViewPage(Page previous)
    {
      this.InitializeComponent();
      this.previous = previous;
      this.next = (Page) new DownloadPage((Page) this);
    }

    public override void SetSetting(Settings settings)
    {
      base.SetSetting(settings);
      if (settings.Emitents == null || settings.Emitents.Count == 0)
        this.buttonRefresh_Click((object) null, (EventArgs) null);
      else
        this.finamTreeView.SetEmitents(settings.Emitents);
    }

    public override Settings GetSetting()
    {
      this.settings.Emitents = this.finamTreeView.GetEmitents();
      return base.GetSetting();
    }

    private void buttonRefresh_Click(object sender, EventArgs e)
    {
      this.finamTreeView.SetEmitents(FinamHelper.DownloadEmitents(this.settings));
      this.buttonRefresh.Enabled = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.buttonRefresh = new Button();
      this.finamTreeView = new FinamTreeView();
      this.SuspendLayout();
      this.buttonRefresh.Location = new Point(4, 4);
      this.buttonRefresh.Name = "buttonRefresh";
      this.buttonRefresh.Size = new Size(75, 23);
      this.buttonRefresh.TabIndex = 0;
      this.buttonRefresh.Text = "Обновить";
      this.buttonRefresh.UseVisualStyleBackColor = true;
      this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
      this.finamTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.finamTreeView.ImageIndex = 0;
      this.finamTreeView.Location = new Point(4, 33);
      this.finamTreeView.Name = "finamTreeView";
      this.finamTreeView.SelectedImageIndex = 0;
      this.finamTreeView.Size = new Size(395, 391);
      this.finamTreeView.TabIndex = 1;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.finamTreeView);
      this.Controls.Add((Control) this.buttonRefresh);
      this.Name = "FinamTreeViewPage";
      this.Size = new Size(402, 436);
      this.ResumeLayout(false);
    }
  }
}
