using System;
using System.Net;

namespace FinamDownloader
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
