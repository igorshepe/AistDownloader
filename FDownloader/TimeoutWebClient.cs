// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.TimeoutWebClient
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System;
using System.Net;

namespace owp.FDownloader
{
  public class TimeoutWebClient : WebClient
  {
    protected override WebRequest GetWebRequest(Uri address)
    {
      WebRequest webRequest = base.GetWebRequest(address);
      webRequest.Timeout = 600000;
      return webRequest;
    }
  }
}
