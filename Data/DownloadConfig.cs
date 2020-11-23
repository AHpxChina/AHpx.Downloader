using System;
using System.Net.Http.Headers;

namespace AHpx.Downloader.Data
{
    public class DownloadConfig
    {
        public ProductInfoHeaderValue UserAgent { get; set; }
        public bool AutoRedirect { get; set; }
        public IProgress<DownloadInfo> Progress { get; set; }

        public DownloadConfig()
        {
            UserAgent = new ProductInfoHeaderValue("Downloader", "1.0");
            AutoRedirect = true;
            Progress = new Progress<DownloadInfo>(info => { });
        }
    }
}