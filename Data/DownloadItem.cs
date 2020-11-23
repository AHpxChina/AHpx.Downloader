using System.IO;

namespace AHpx.Downloader.Data
{
    public class DownloadItem
    {
        public FileInfo File { get; set; }
        public string Url { get; set; }
    }
}