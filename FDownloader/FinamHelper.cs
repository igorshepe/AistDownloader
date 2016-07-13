// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.FinamHelper
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using log4net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace owp.FDownloader
{
  internal class FinamHelper
  {
    private static readonly ILog l = LogManager.GetLogger(typeof (FinamHelper));

    private static WebClient InitWebClient(Settings settings)
    {
      FinamHelper.l.Debug((object) "Создаю WebClient");
      WebClient webClient = (WebClient) new TimeoutWebClient();
      if (settings.useProxy)
      {
        WebProxy webProxy = new WebProxy();
        webProxy.Address = new Uri(settings.proxy);
        if (settings.proxyWithPassword)
          webProxy.Credentials = (ICredentials) new NetworkCredential(settings.proxyUser, settings.proxyPassword);
        webClient.Proxy = (IWebProxy) webProxy;
      }
      return webClient;
    }

    public static List<EmitentInfo> DownloadEmitents(Settings settings)
    {
            l.Debug("Скачиваю список эмитентов из финама");
            System.Net.WebClient webClient = InitWebClient(settings);

            string marketsString = string.Empty;

            try
            {
                // скачиваю интерфейс
                //marketsString = webClient.DownloadString(@"http://www.finam.ru/analysis/export/default.asp");
                marketsString = webClient.DownloadString(@"http://www.finam.ru/analysis/profile041CA00007/default.asp");
            }
            catch (Exception e)
            {
                l.Error("Не смог скачать интерфейс с финама " + e);
                return null;
            }

            //String sOption = @"<option\s+?.*?value=""(?<id>[0-9]+)"".*?>(?<name>.+?)</option>";
            String sOption = @"\{\s+value:\s+(?<id>[0-9]+)\,\s+title\:\s+\'(?<name>.+?)\'\}(\,\s)*?";
            //System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(marketsString, sOption, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            //String sSelect = @"<select(.|\n)+?id=""market""(.|\n)+?(" + sOption + ")+?(.|\n)*?</select>";
            String sSelect = @"setMarkets\(\[(" + sOption + ")+?]";



            // Поиск нужного <select id="market">
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(marketsString, sSelect, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!m.Success)
            {
                l.Error("Ошибка парсинга <select id=market>");
                return null;
            }

            marketsString = m.Value;

            // поиск всех секций
            System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(marketsString, sOption, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            List<EmitentInfo> markets = new List<EmitentInfo>();

            foreach (System.Text.RegularExpressions.Match match in mc)
            {
                markets.Add(new EmitentInfo(Convert.ToInt32(match.Groups["id"].Value), match.Groups["name"].Value, -1, String.Empty, String.Empty));
            }

            // скачиваю js
            //            string instruments = webClient.DownloadString(@"http://www.finam.ru/scripts/export.js");

            string instruments = webClient.DownloadString(@"http://www.finam.ru/cache/icharts/icharts.js");

            //instruments = File.ReadAllText(@"d:\3.txt");            

            instruments = instruments.Replace("\\'", "-"); // убрать кавычки из списка эмитентов, в противном случае строка не парситься
            //instruments = instruments.Replace("�", "-"); // 
            instruments = instruments.Replace("[", "("); //
            instruments = instruments.Replace("{", "("); //
            instruments = instruments.Replace("]", ")"); //
            instruments = instruments.Replace("}", ")"); //
            instruments = instruments.Replace(":", ""); //
            instruments = instruments.Replace(" = ", "="); //

            String pattern = @"var\saEmitentIds=\((?<EmitentIds>.*?)\);" + "(.|\n)*?" +
                             @"var\saEmitentNames=\((?<EmitentNames>.*?)\);" + "(.|\n)*?" +
                             @"var\saEmitentCodes=\((?<EmitentCodes>.*?)\);" + "(.|\n)*?" +
                             @"var\saEmitentMarkets=\((?<EmitentMarkets>.*?)\);" + "(.|\n)*?" +
                             @"var\saEmitentDecp=\((?<EmitentDecp>.*?)\);" + "(.|\n)*?" +
                             @"var\saDataFormatStrs=\(.*?\);";

            /*            String pattern = @"var\saEmitentIds=new\sArray\((?<EmitentIds>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentNames=new\sArray\((?<EmitentNames>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentCodes=new\sArray\((?<EmitentCodes>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentMarkets=new\sArray\((?<EmitentMarkets>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentDecp=new\sArray\((?<EmitentDecp>.*?)\);" + "(.|\n)*?" +
                                         @"var\saDataFormatStrs=new Array\(.*?\);";
            */
            /*            String pattern = @"var\saEmitentIds\s=\((?<EmitentIds>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentNames\s=\((?<EmitentNames>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentCodes\s=\((?<EmitentCodes>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentMarkets\s=\((?<EmitentMarkets>.*?)\);" + "(.|\n)*?" +
                                         @"var\saEmitentDecp\s=\((?<EmitentDecp>.*?)\);" + "(.|\n)*?" +
                                         @"var\saDataFormatStrs\s=\(.*?\);";
            */
            System.Text.RegularExpressions.Match emitentMarkets
                = System.Text.RegularExpressions.Regex.Match(instruments, pattern);

            System.Text.RegularExpressions.MatchCollection sEmitentIds
                = System.Text.RegularExpressions.Regex.Matches(emitentMarkets.Groups["EmitentIds"].Value, @"[0-9]+");
            System.Text.RegularExpressions.MatchCollection sEmitentNames
                = System.Text.RegularExpressions.Regex.Matches(emitentMarkets.Groups["EmitentNames"].Value, @"'.+?'");
            System.Text.RegularExpressions.MatchCollection sEmitentCodes
                = System.Text.RegularExpressions.Regex.Matches(emitentMarkets.Groups["EmitentCodes"].Value, @"'.+?'");
            System.Text.RegularExpressions.MatchCollection sEmitentMarkets
                = System.Text.RegularExpressions.Regex.Matches(emitentMarkets.Groups["EmitentMarkets"].Value, @"[0-9]+");

            if ((sEmitentIds.Count != 0) && (sEmitentNames.Count != 0) && (sEmitentCodes.Count != 0) && (sEmitentMarkets.Count != 0))
            {
                int count = Math.Min(Math.Min(Math.Min(sEmitentIds.Count, sEmitentNames.Count), sEmitentCodes.Count), sEmitentMarkets.Count);
                List<EmitentInfo> emitents = new List<EmitentInfo>();
                for (int i = 0; i < count; ++i)
                {
                    foreach (EmitentInfo market in markets)
                    {
                        if (Convert.ToInt32(sEmitentMarkets[i].Value) == market.marketId)
                        {
                            String chars4trim = "' ";
                            String instrumentName = sEmitentNames[i].Value.Trim(chars4trim.ToCharArray());
                            emitents.Add(new EmitentInfo(
                                                            market.marketId,
                                                            market.marketName,
                                                            Convert.ToInt32(sEmitentIds[i].Value),
                                                            instrumentName,
                                                            sEmitentCodes[i].Value.Trim(chars4trim.ToCharArray())
                                        ));
                            break;
                        }
                    }
                }
                return emitents;
            }
            else
            {
                l.Error("Ошибка парсинга export.js");
                return null;
            }
        }

        public static string Download(Settings settings, EmitentInfo emitent)
    {
      int num = settings.period != 1 ? 5 : 11;
      string address = string.Format("http://195.128.78.52/{0}.{1}?d=d&market={2}&em={3}&p={4}&df={5}&mf={6}&yf={7}&dt={8}&mt={9}&yt={10}&f={11}&e=.{12}&datf={13}&cn={14}&dtf=1&tmf=1&MSOR=0&sep=1&sep2=1&at=1", (object) emitent.code, (object) "txt", (object) emitent.marketId, (object) emitent.id, (object) settings.period, (object) settings.from.Day, (object) (settings.from.Month - 1), (object) settings.from.Year, (object) settings.to.Day, (object) (settings.to.Month - 1), (object) settings.to.Year, (object) emitent.code, (object) "txt", (object) num, (object) emitent.code);
      FinamHelper.l.Debug((object) ("Скачиваю " + address));
      WebClient webClient = FinamHelper.InitWebClient(settings);
      webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");
      string str1 = string.Empty;
      string str2;
      try
      {
        str2 = webClient.DownloadString(address);
      }
      catch (Exception ex)
      {
        str2 = "Exception";
        FinamHelper.l.Info((object) ("Ошибка при скачивании " + (object) ex));
      }
      return str2;
    }
  }
}
