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

        


        //public static void Download(List<SecurityInfo> security, List<Main.FileSecurity> filesSecurities, List<Main.SettingsMain> settingsData)
        //{
        //    var settings = settingsData[0];
        //    string str2 = String.Empty;

        //    for (int i = 0; i < security.Count; i++)
        //    {

        //        //Main.backgroundWorker1.ReportProgress((i+1)*10);
        //        var securitySelect = security[i];


        //        if (!settings.DateFromeTxt)
        //        {

        //            if (securitySelect.Checed)
        //            {
        //                Main.backgroundWorker1.ReportProgress(70, "Start download: " + securitySelect.Name);
        //                Log.Info("Start download: " + securitySelect.Name + " date from: " + settings.DateFrom.ToString("d") + " date to: " + settings.DateTo.ToString("d"));
        //                //TODO: Проверить , почему верные значения fileheaderRow получаем только после сохранения настроек

        //                string address =
        //                    string.Format(
        //                        "http://195.128.78.52/{0}.{1}?d=d&market={2}&em={3}&p={4}&df={5}&mf={6}&yf={7}&dt={8}&mt={9}&yt={10}&f={11}&e=.{12}&datf={13}&cn={14}&dtf=1&tmf=1&MSOR={15}&sep={16}&sep2=1&at={17}",
        //                        (object)securitySelect.Code, "txt", (object)securitySelect.MarketId,
        //                        (object)securitySelect.Id, settings.Period + 1, settings.DateFrom.Day, settings.DateFrom.Month - 1,
        //                        settings.DateFrom.Year, settings.DateTo.Day, settings.DateTo.Month - 1, settings.DateTo.Year,
        //                        (object)securitySelect.Code, "txt", 5, (object)securitySelect.Code, settings.TimeCandle,
        //                        settings.SplitChar, Convert.ToInt32(settings.FileheaderRow));

        //                Log.Debug("Скачиваю " + address); WebClient webClient = InitWebClient();
        //                webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");
        //                try
        //                {
        //                    str2 = webClient.DownloadString(address);
        //                    Main.SaveToFile(str2, securitySelect, settings);

        //                }
        //                catch (Exception ex)
        //                {
        //                    str2 = "Exception";
        //                    Log.Info("Ошибка при скачивании " + ex);
        //                }
        //            }

        //        }
        //        else
        //        {
        //            for (int j = 0; j < filesSecurities.Count; j++)
        //            {
        //                if (securitySelect.Name == filesSecurities[j].Sec)
        //                {
        //                    if (!(filesSecurities[j].Dat.AddDays(-1) == settings.DateTo))
        //                    {

        //                        var securityFile = filesSecurities[j];
        //                        Log.Info("Start merge file download: " + securitySelect.Name + " date from: " + securityFile.Dat.ToString("d") + " date to: " + settings.DateTo.ToString("d"));
        //                        string address = string.Format(
        //                                "http://195.128.78.52/{0}.{1}?d=d&market={2}&em={3}&p={4}&df={5}&mf={6}&yf={7}&dt={8}&mt={9}&yt={10}&f={11}&e=.{12}&datf={13}&cn={14}&dtf=1&tmf=1&MSOR={15}&sep={16}&sep2=1&at={17}",
        //                                (object)securitySelect.Code, "txt", (object)securitySelect.MarketId,
        //                                (object)securitySelect.Id, settings.Period + 1, securityFile.Dat.Day,
        //                                securityFile.Dat.Month - 1, securityFile.Dat.Year, settings.DateTo.Day,
        //                                settings.DateTo.Month - 1, settings.DateTo.Year, (object)securitySelect.Code, "txt", 5,
        //                                (object)securitySelect.Code, settings.TimeCandle, settings.SplitChar,
        //                                Convert.ToInt32(settings.FileheaderRow));
        //                        Log.Debug("Скачиваю " + address);


        //                        WebClient webClient = InitWebClient();
        //                        webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");

        //                        try
        //                        {
        //                            str2 = webClient.DownloadString(address);
        //                            if (settings.MergeFile)
        //                            {
        //                                Main.ChangeFile(str2, securityFile, settings);
        //                            }
        //                            else
        //                            {
        //                                Main.SaveToFile(str2, securitySelect, settings);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            str2 = "Exception";
        //                            Log.Info("Ошибка при скачивании " + ex);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Log.Info("Дата файла и дата загрузки истории совпадают: " + filesSecurities[j].Sec + " Date from: " + filesSecurities[j].Dat.AddDays(-1).ToString("d") + ". Date to: " + settings.DateTo.ToString("d"));
        //                    }



        //                }

        //            }

        //        }




        //    }
        //}

        public static string DownloadData(SecurityInfo security, Main.FileSecurity filesSecurities, Main.SettingsMain settingsData, bool mergeFile)
        {
            string str;

            var securitySelect = security;
            var securityFile = filesSecurities;
            var settings = settingsData;

            //TODO: Проверить , почему верные значения fileheaderRow получаем только после сохранения настроек
            WebClient webClient = InitWebClient();
            string address;

            if (!mergeFile) // Создание нового файла
            {
                Log.Info("Start download: " + securitySelect.Name + " date from: " + settings.DateFrom.ToString("d") +
                     " to: " + settings.DateTo.ToString("d"));
                address =  $"http://195.128.78.52/{securitySelect.Code}.{"txt"}?d=d&market={securitySelect.MarketId}&em={securitySelect.Id}&p={settings.Period + 1}&df={settings.DateFrom.Day}&mf={settings.DateFrom.Month - 1}&yf={settings.DateFrom.Year}&dt={settings.DateTo.Day}&mt={settings.DateTo.Month - 1}&yt={settings.DateTo.Year}&f={(object) securitySelect.Code}&e=.{"txt"}&datf={5}&cn={(object) securitySelect.Code}&dtf=1&tmf=1&MSOR={settings.TimeCandle}&sep={settings.SplitChar}&sep2=1&at={Convert.ToInt32(settings.FileheaderRow)}";
                Log.Debug("Скачиваю " + address);
                
                webClient.Headers.Add("Referer", "http://www.finam.ru/analysis/export/default.asp");
                }
            else // Докачка существующих файлов
            {
                Log.Info("Start merge file download: " + securitySelect.Name + " date from: " + securityFile.Dat.ToString("d") + " to: " + settings.DateTo.ToString("d"));
                address = $"http://195.128.78.52/{ securitySelect.Code}.{"txt"}?d=d&market={securitySelect.MarketId}&em={securitySelect.Id}&p={settings.Period + 1}&df={securityFile.Dat.Day}&mf={securityFile.Dat.Month - 1}&yf={securityFile.Dat.Year}&dt={settings.DateTo.Day}&mt={settings.DateTo.Month - 1}&yt={settings.DateTo.Year}&f={(object) securitySelect.Code}&e=.{"txt"}&datf={5}&cn={(object) securitySelect.Code}&dtf=1&tmf=1&MSOR={settings.TimeCandle}&sep={settings.SplitChar}&sep2=1&at={Convert.ToInt32(settings.FileheaderRow)}";
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
