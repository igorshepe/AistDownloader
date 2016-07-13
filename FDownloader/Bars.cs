// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.Bars
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace owp.FDownloader
{
  internal class Bars
  {
    private static readonly ILog l = LogManager.GetLogger(typeof (Bars));
    private List<Bar> list = new List<Bar>();
    private int last_bar;

    public EmitentInfo emitent { get; set; }

    public Bar this[int i]
    {
      get
      {
        return this.list[i];
      }
    }

    public int Count
    {
      get
      {
        return this.list.Count;
      }
    }

    public DateTime Last
    {
      get
      {
        if (this.Count > 0)
          return this.list[this.Count - 1].dt;
        return DateTime.Today.AddDays(-1.0);
      }
    }

    public Bars(EmitentInfo emitent)
    {
      this.emitent = emitent;
    }

    public void Save(string fileName)
    {
      if (this.list.Count == 0)
      {
        Bars.l.Debug((object) "Не сохраняю bars т.к. list.Count == 0");
      }
      else
      {
        Bars.l.Debug((object) ("Сохраняю bars в wl " + fileName));
        if (!Directory.Exists(Path.GetDirectoryName(fileName)))
          Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open(fileName, FileMode.Create, FileAccess.Write));
        binaryWriter.Write(this.list.Count);
        for (int index = 0; index < this.list.Count; ++index)
        {
          binaryWriter.Write(this.list[index].dt.ToOADate());
          binaryWriter.Write((float) this.list[index].open);
          binaryWriter.Write((float) this.list[index].high);
          binaryWriter.Write((float) this.list[index].low);
          binaryWriter.Write((float) this.list[index].close);
          binaryWriter.Write((float) this.list[index].volume);
        }
        binaryWriter.Close();
      }
    }

    public void SaveCSV(string fileName)
    {
        
            if (this.list.Count == 0)
      {
        Bars.l.Debug((object) "Не сохраняю bars т.к. list.Count == 0");
      }
      else
      {
        Bars.l.Debug((object) ("Сохраняю bars в csv " + fileName));
        if (!Directory.Exists(Path.GetDirectoryName(fileName)))
          Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        StreamWriter streamWriter = new StreamWriter((Stream) File.Open(fileName, FileMode.Create, FileAccess.Write));
        int num;
        if (this.list[0].id == 0L && this.list[0].low == this.list[0].high && 10<1)
        {
          num = 1;
          streamWriter.WriteLine("<DATE>,<TIME>,<LAST>,<VOL>,<ID>");
        }
        else
        {
          num = 2;
          streamWriter.WriteLine("<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>");
        }
        foreach (Bar bar in this.list)
        {
          if (num == 1)
          {
            streamWriter.Write(bar.dt.ToString("yyyyMMdd,HHmmss,"));
            streamWriter.Write(bar.close.ToString());
            streamWriter.Write(',');
            streamWriter.Write(bar.volume.ToString());
            streamWriter.Write(',');
            streamWriter.WriteLine(bar.id.ToString());
          }
          else
          {
            streamWriter.Write(bar.dt.ToString("yyyyMMdd,HHmmss,"));
            streamWriter.Write(Convert.ToString(bar.open, CultureInfo.InvariantCulture));
            streamWriter.Write(',');
            streamWriter.Write(Convert.ToString(bar.high, CultureInfo.InvariantCulture));
            streamWriter.Write(',');
            streamWriter.Write(Convert.ToString(bar.low, CultureInfo.InvariantCulture));
            streamWriter.Write(',');
            streamWriter.Write(Convert.ToString(bar.close, CultureInfo.InvariantCulture));
            streamWriter.Write(',');
            streamWriter.WriteLine(Convert.ToString(bar.volume, CultureInfo.InvariantCulture));
          }
        }
        streamWriter.Close();
      }
    }

    private void Add(Bar bar)
    {
      Bars.l.Debug((object) "Добавляю bar");
      if (this.list.Count == 0)
      {
        this.list.Add(bar);
      }
      else
      {
        if (this.last_bar < 0 || this.last_bar >= this.list.Count)
          this.last_bar = 0;
        while (this.last_bar > 0 && this.list[this.last_bar].dt > bar.dt)
          --this.last_bar;
        while (this.last_bar < this.list.Count && this.list[this.last_bar].dt < bar.dt)
          ++this.last_bar;
        if (this.last_bar != this.list.Count && (!(this.list[this.last_bar].dt != bar.dt) || bar.id != 0L) && this.list[this.last_bar].id == bar.id)
          return;
        this.list.Insert(this.last_bar, bar);
      }
    }

    public void LoadWL(string fileName)
    {
      Bars.l.Debug((object) ("Читаю bars из " + fileName));
      if (!File.Exists(fileName))
        return;
      BinaryReader binaryReader = new BinaryReader((Stream) File.Open(fileName, FileMode.Open, FileAccess.Read));
      try
      {
        for (int index = binaryReader.ReadInt32(); index > 0; --index)
          this.Add(new Bar()
          {
            dt = DateTime.FromOADate(binaryReader.ReadDouble()),
            open = (double) binaryReader.ReadSingle(),
            high = (double) binaryReader.ReadSingle(),
            low = (double) binaryReader.ReadSingle(),
            close = (double) binaryReader.ReadSingle(),
            volume = (double) binaryReader.ReadSingle()
          });
      }
      finally
      {
        binaryReader.Close();
      }
    }

    public void LoadCSV(TextReader csvFile)
    {
      Bars.l.Debug((object) "Читаю bars из csv");
      int num = 0;
      string str1 = csvFile.ReadLine();
      if (str1 == "<DATE>,<TIME>,<LAST>,<VOL>,<ID>")
        num = 1;
      if (str1 == "<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>")
        num = 2;
      if (num != 0)
      {
        string str2;
        while ((str2 = csvFile.ReadLine()) != null)
        {
          string[] strArray = str2.Split(',');
          Bar bar = new Bar();
          bar.dt = DateTime.ParseExact(strArray[0] + strArray[1], "yyyyMMddHHmmss", (IFormatProvider) null);
          NumberFormatInfo numberFormat = new CultureInfo("en-US", false).NumberFormat;
          numberFormat.NumberDecimalSeparator = ".";
          try
          {
            if (num == 1)
            {
              bar.close = Convert.ToDouble(strArray[2], (IFormatProvider) numberFormat);
              bar.volume = Convert.ToDouble(strArray[3], (IFormatProvider) numberFormat);
              bar.id = Convert.ToInt64(strArray[4], (IFormatProvider) numberFormat);
              bar.open = bar.close;
              bar.high = bar.close;
              bar.low = bar.close;
            }
            else
            {
              bar.open = Convert.ToDouble(strArray[2], (IFormatProvider) numberFormat);
              bar.high = Convert.ToDouble(strArray[3], (IFormatProvider) numberFormat);
              bar.low = Convert.ToDouble(strArray[4], (IFormatProvider) numberFormat);
              bar.close = Convert.ToDouble(strArray[5], (IFormatProvider) numberFormat);
              bar.volume = Convert.ToDouble(strArray[6], (IFormatProvider) numberFormat);
            }
            this.Add(bar);
          }
          catch (Exception ex)
          {
            Bars.l.Error((object) ("Ошибка при парсинге строки " + str2 + " " + (object) ex));
          }
        }
      }
      else
        Bars.l.Error((object) ("Неверный формат txt файла " + str1));
    }

    public void Clear()
    {
      Bars.l.Debug((object) "Clear");
      this.list.Clear();
    }
  }
}
