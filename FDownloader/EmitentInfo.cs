// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.EmitentInfo
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

namespace owp.FDownloader
{
  public class EmitentInfo
  {
    public int marketId = -1;
    public int id = -1;
    public string marketName = string.Empty;
    public string name = string.Empty;
    public string code = string.Empty;
    public bool checed;

    public EmitentInfo()
    {
    }

    public EmitentInfo(int marketId, string marketName, int emitentId, string emitentName, string emitentCode)
    {
      this.marketId = marketId;
      this.id = emitentId;
      this.marketName = marketName;
      this.name = emitentName;
      this.code = emitentCode;
    }
  }
}
