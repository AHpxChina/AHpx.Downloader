using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AHpx.Downloader.Data;

namespace AHpx.Downloader.Core
{
    public static class DownloaderCore
    {
        public static async Task Download(DownloadItem item, DownloadConfig config = null)
        {
            config ??= new DownloadConfig();

            var handle = new HttpClientHandler
            {
                AllowAutoRedirect = config.AutoRedirect
            };
            var client = new HttpClient(handle);
            client.DefaultRequestHeaders.UserAgent.Add(config.UserAgent);

            var response = await client.GetAsync(item.Url, HttpCompletionOption.ResponseHeadersRead);
            if (config.AutoRedirect)
            {
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    response = await client.GetAsync(response.Headers.Location, HttpCompletionOption.ResponseHeadersRead);
                }
            }

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[8192];
            var contentLength = response.Content.Headers.ContentLength;
            var info = new DownloadInfo {TotalBytesToReceive = contentLength};

            config.Progress.Report(info);

            var re = new List<byte>();
            int byteReader;
            
            while ((byteReader = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                re.AddRange(buffer.Take(byteReader));

                info.BytesReceived += byteReader;
                config.Progress.Report(info);
            }
            
            await File.WriteAllBytesAsync(item.File.FullName, re.ToArray());
        }
        
        public static async Task Download(IEnumerable<DownloadItem> items, DownloadConfig config = null)
        {
            // config ??= new DownloadConfig();
            //
            // var handle = new HttpClientHandler
            // {
            //     AllowAutoRedirect = config.AutoRedirect
            // };
            // var client = new HttpClient(handle);
            // client.DefaultRequestHeaders.UserAgent.Add(config.UserAgent);
            //
            // //Task.Factory.StartNew()
            //

            throw new NotImplementedException("Not now, use another one");
        }
    }
}