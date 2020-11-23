using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AHpx.Downloader.Core;
using AHpx.Downloader.Data;

namespace AHpx.Downloader.Test
{
    public static class Class1
    {
        public static async Task Main(string[] args)
        {
            await DownloaderCore.Download(new DownloadItem
            {
                File = new FileInfo(@"C:\Users\ahpx\Desktop\Test1\poxiao.jar"),
                Url = "https://launcher.mojang.com/v1/objects/e80d9b3bf5085002218d4be59e668bac718abbc6/client.jar"
            }, new DownloadConfig
            {
                Progress = new Progress<DownloadInfo>(info =>
                {
                    Console.WriteLine($"Progress: {info.BytesReceived / 1024}/{info.TotalBytesToReceive / 1024}kb");
                })
            });
        }
    }
}