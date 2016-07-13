// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.Page
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System.Windows.Forms;

namespace owp.FDownloader
{
  public class Page : UserControl
  {
    protected Settings settings;
    protected Page previous;
    protected Page next;

    public Page()
    {
    }

    public Page(Page previous)
    {
      this.previous = previous;
    }

    public virtual void SetSetting(Settings settings)
    {
      this.settings = settings;
    }

    public virtual Settings GetSetting()
    {
      return this.settings;
    }

    public virtual Page PreviousControl()
    {
      return this.previous;
    }

    public virtual bool PreviousExists()
    {
      return this.previous != null;
    }

    public virtual Page NextControl()
    {
      return this.next;
    }

    public virtual bool NextExists()
    {
      return this.next != null;
    }
  }
}
