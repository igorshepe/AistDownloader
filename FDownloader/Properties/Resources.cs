// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.Properties.Resources
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace owp.FDownloader.Properties
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) owp.FDownloader.Properties.Resources.resourceMan, (object) null))
          owp.FDownloader.Properties.Resources.resourceMan = new ResourceManager("owp.FDownloader.Properties.Resources", typeof (owp.FDownloader.Properties.Resources).Assembly);
        return owp.FDownloader.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get
      {
        return owp.FDownloader.Properties.Resources.resourceCulture;
      }
      set
      {
        owp.FDownloader.Properties.Resources.resourceCulture = value;
      }
    }

    public static string log4net
    {
      get
      {
        return owp.FDownloader.Properties.Resources.ResourceManager.GetString("log4net", owp.FDownloader.Properties.Resources.resourceCulture);
      }
    }

    internal Resources()
    {
    }
  }
}
