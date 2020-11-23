using System;

namespace AHpx.Downloader.Data
{
    public class DownloadInfo
    {
        public long BytesReceived { get; set; }

        public long? TotalBytesToReceive { get; set; }
    }
}