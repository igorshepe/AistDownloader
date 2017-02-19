using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using log4net;

namespace FinamDownloader
{
    internal class FinamLoading
    {

       // static readonly Main Main = new Main(new[] { "_" });


        private static readonly ILog Log = LogManager.GetLogger(typeof(FinamLoading));
        private static WebClient InitWebClient()
        {
            WebClient webClient = new TimeoutWebClient(); return webClient;
        }

        public static string Urldownload = "http://export.finam.ru/";

         

        public static string DownloadData(SecurityInfo security, Main.FileSecurity filesSecurities, Main.SettingsMain settingsData, bool mergeFile)
        {
            string str;

            var securitySelect = security;
            var securityFile = filesSecurities;
            var settings = settingsData;
            var datformat = 5;
            if (settings.PeriodItem == "tics")
            {
                datformat = 6;
            }
            WebClient webClient = InitWebClient();
            string address;

            if (!mergeFile) // Создание нового файла
            {
                Log.Info("Start download: " + securitySelect.Name + " date from: " + settings.DateFrom.ToString("d") +
                     " to: " + settings.DateTo.ToString("d"));
                address =  $"{Urldownload}{securitySelect.Code}.{"txt"}?d=d&market={securitySelect.MarketId}&em={securitySelect.Id}&p={settings.Period + 1}&df={settings.DateFrom.Day}&mf={settings.DateFrom.Month - 1}&yf={settings.DateFrom.Year}&dt={settings.DateTo.Day}&mt={settings.DateTo.Month - 1}&yt={settings.DateTo.Year}&f={(object) securitySelect.Code}&e=.{"txt"}&datf={datformat}&cn={(object) securitySelect.Code}&dtf=1&tmf=1&MSOR={settings.TimeCandle}&sep={settings.SplitChar}&sep2=1&at={Convert.ToInt32(settings.FileheaderRow)}";
                Log.Debug("Скачиваю " + address);
                
                webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");
                }
            else // Докачка существующих файлов
            {
                Log.Info("Start merge file download: " + securitySelect.Name + " date from: " + securityFile.Dat.ToString("d") + " to: " + settings.DateTo.ToString("d"));
                address = $"{Urldownload}{securitySelect.Code}.{"txt"}?d=d&market={securitySelect.MarketId}&em={securitySelect.Id}&p={settings.Period + 1}&df={securityFile.Dat.Day}&mf={securityFile.Dat.Month - 1}&yf={securityFile.Dat.Year}&dt={settings.DateTo.Day}&mt={settings.DateTo.Month - 1}&yt={settings.DateTo.Year}&f={(object) securitySelect.Code}&e=.{"txt"}&datf={datformat}&cn={(object) securitySelect.Code}&dtf=1&tmf=1&MSOR={settings.TimeCandle}&sep={settings.SplitChar}&sep2=1&at={Convert.ToInt32(settings.FileheaderRow)}";
                Log.Debug("Скачиваю " + address);
                
                webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");

            }

            try
            {
                str = webClient.DownloadString(address);

            }
            catch (Exception ex)
            {
                str = "Exception";
                Log.Info("Ошибка при скачивании " + ex);
            }

            return str;
        }

    }
}
