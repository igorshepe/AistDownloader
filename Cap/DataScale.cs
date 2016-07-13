// Decompiled with JetBrains decompiler
// Type: owp.Cap.DataScale
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System;

namespace owp.Cap
{
  public class DataScale
  {
    public readonly DataScaleEnum scale;
    public readonly int interval;
    public readonly DateTime from;

    public DataScale(DataScaleEnum scale, int interval)
      : this(scale, interval, DateTime.MinValue)
    {
    }

    public DataScale(DataScaleEnum scale, int interval, DateTime from)
    {
      this.from = from;
      this.scale = scale;
      this.interval = interval;
      if (interval < 1)
        throw new ArgumentException("interval не может быть меньше 0, а он равен " + (object) interval);
      if (scale == DataScaleEnum.volume && interval == 1)
        throw new ArgumentException("interval не может быть равен 1, для DataScaleEnum.volume");
    }

    public override string ToString()
    {
      string str = string.Empty;
      if (this.from != DateTime.MinValue)
        str = "From" + this.from.ToString("yy.MM.dd hh-mm-ss");
      if (this.interval == 1 && this.scale != DataScaleEnum.month)
        return this.scale.ToString() + str;
      if (this.scale == DataScaleEnum.sec)
      {
        if (this.interval >= 604800 && this.interval % 604800 == 0)
          return (this.interval / 604800).ToString() + "w" + str;
        if (this.interval >= 86400 && this.interval % 86400 == 0)
          return (this.interval / 86400).ToString() + "d" + str;
        if (this.interval >= 3600 && this.interval % 3600 == 0)
          return (this.interval / 3600).ToString() + "h" + str;
        if (this.interval >= 60 && this.interval % 60 == 0)
          return (this.interval / 60).ToString() + "min" + str;
        return this.interval.ToString() + "sec" + str;
      }
      if (this.scale == DataScaleEnum.month)
        return this.interval.ToString() + "m" + str;
      return this.interval.ToString() + this.scale.ToString() + str;
    }

    public bool CanConvertTo(DataScale dataScale)
    {
      if (this.from != dataScale.from)
        return false;
      if (this.Equals((object) dataScale) || this.interval == 1 && this.scale == DataScaleEnum.tick)
        return true;
      if (this.scale == dataScale.scale)
        return 0 == dataScale.interval % this.interval;
      return false;
    }

    public override bool Equals(object obj)
    {
      DataScale dataScale = obj as DataScale;
      if (dataScale == null)
        return base.Equals(obj);
      if (this.interval == dataScale.interval && this.scale == dataScale.scale)
        return this.from == dataScale.from;
      return false;
    }

    public override int GetHashCode()
    {
      return (this.interval ^ this.scale.GetHashCode()).GetHashCode();
    }
  }
}
