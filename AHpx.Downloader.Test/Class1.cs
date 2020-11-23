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
using Newtonsoft.Json.Linq;

namespace AHpx.Downloader.Test
{
    public static class Class1
    {
        public static async Task Main(string[] args)
        {
            var list = await GetDownloadItems();
            var acs = new List<Task>();

            foreach (var item in list)
            {
                // await Task.Factory.StartNew(async () =>
                // {
                //     
                // });

                acs.Add(new Task(async () =>
                {
                    await DownloaderCore.Download(item, new DownloadConfig
                    {
                        Progress = new Progress<DownloadInfo>(info =>
                        {
                            Console.WriteLine($"File: {item.File.Name}");
                            Console.WriteLine(
                                $"Progress: {info.BytesReceived / 1024}/{info.TotalBytesToReceive / 1024}kb");
                        })
                    });
                }));
            }

            await Task.WhenAll(acs);
        }

        public static async Task<List<DownloadItem>> GetDownloadItems()
        {
            var list = new List<DownloadItem>();
            foreach (var jToken in JArray.Parse(await File.ReadAllTextAsync("downloadslist.json")))
            {
                list.Add(new DownloadItem
                {
                    File = new FileInfo(jToken["file"].ToString()),
                    Url = jToken["url"].ToString()
                });
            }

            return list;
        }
    }
}