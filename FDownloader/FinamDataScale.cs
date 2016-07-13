// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.FinamDataScale
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using owp.Cap;
using System;

namespace owp.FDownloader
{
  public static class FinamDataScale
  {
    private static DataScaleEnum Finam2scale(int i)
    {
      if (i < 1 || i > 11)
        throw new IndexOutOfRangeException("FinamDataScale поддерживает только периоды от 1 до 11");
      switch (i)
      {
        case 1:
          return DataScaleEnum.tick;
        case 10:
          return DataScaleEnum.month;
        default:
          return DataScaleEnum.sec;
      }
    }

    private static int Finam2interval(int i)
    {
      if (i < 1 || i > 11)
        throw new IndexOutOfRangeException("FinamDataScale поддерживает только периоды от 1 до 11");
      switch (i)
      {
        case 1:
          return 1;
        case 2:
          return 60;
        case 3:
          return 300;
        case 4:
          return 600;
        case 5:
          return 900;
        case 6:
          return 1800;
        case 7:
        case 11:
          return 3600;
        case 8:
          return 86400;
        case 9:
          return 604800;
        case 10:
          return 1;
        default:
          return 0;
      }
    }

    private static DateTime Finam2from(int i)
    {
      if (i == 11)
        return new DateTime(1, 1, 1, 10, 30, 0);
      return DateTime.MinValue;
    }

    public static string ToString(int i)
    {
      return new DataScale(FinamDataScale.Finam2scale(i), FinamDataScale.Finam2interval(i), FinamDataScale.Finam2from(i)).ToString();
    }
  }
}
