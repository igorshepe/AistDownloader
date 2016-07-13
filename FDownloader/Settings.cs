// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.Settings
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace owp.FDownloader
{
  public class Settings
  {
    public bool autoFlag { get; set; }

    public bool useProxy { get; set; }

    public string proxy { get; set; }

    public bool proxyWithPassword { get; set; }

    public string proxyUser { get; set; }

    public string proxyPassword { get; set; }

    public bool loadFromFinam { get; set; }

    public bool fromYesterday { get; set; }

    public bool fromCSV { get; set; }

    public bool toToday { get; set; }

    public DateTime from { get; set; }

    public DateTime to { get; set; }

    public int period { get; set; }

    public string csvDir { get; set; }

    public bool margeCsv { get; set; }

    public bool convertCSV2WL { get; set; }

    public bool delCSV { get; set; }

    public string wlDir { get; set; }

    public List<EmitentInfo> Emitents { get; set; }

    public Settings()
    {
      this.useProxy = false;
      this.proxy = string.Empty;
      this.proxyWithPassword = false;
      this.proxyUser = string.Empty;
      this.proxyPassword = string.Empty;
      this.Emitents = new List<EmitentInfo>();
    }

    public static Settings Load(string FileName)
    {
      Settings settings;
      if (File.Exists(FileName))
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Settings));
        TextReader textReader = (TextReader) new StreamReader(FileName);
        settings = (Settings) xmlSerializer.Deserialize(textReader);
        textReader.Close();
      }
      else
        settings = new Settings();
      settings.autoFlag = false;
      return settings;
    }

    public void Save(string FileName)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (Settings));
      TextWriter textWriter = (TextWriter) new StreamWriter(FileName);
      xmlSerializer.Serialize(textWriter, (object) this);
      textWriter.Close();
    }
  }
}
