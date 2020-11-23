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
        private const int BufferSize = 8192;

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

            // await Download(@"C:\Users\ahpx\Desktop\Test1\tawduawdawsss.jar",
            //     "https://launcher.mojang.com/v1/objects/e80d9b3bf5085002218d4be59e668bac718abbc6/client.jar",
            //     progress =>
            //     {
            //         Console.WriteLine($"Progress: {progress.BytesReceived / 1024}/{progress.TotalBytesToReceive / 1024}kb");
            //     });
        }

        public static async Task Download(string savePath, string url, Action<HttpDownloadProgress> handler)
        {
            var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url, new Progress<HttpDownloadProgress>(handler), CancellationToken.None);

            await File.WriteAllBytesAsync(savePath, bytes);
            
            // var stop = new Stopwatch();
            // stop.Start();
            // var client = new HttpClient();
            // var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            // if (response.StatusCode == HttpStatusCode.Redirect)
            //     response = await client.GetAsync(response.Headers.Location);
            //
            // Console.WriteLine("Download start!");
            // Console.WriteLine($"Save path: {savePath}");
            //
            // //var stream = await response.Content.ReadAsStreamAsync();
            //
            // Console.WriteLine($"Total size: {response.Content.Headers.ContentLength / 1024}kb");
            //
            // var file = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
            // var bytes = await response.Content.ReadAsByteArrayAsync();
            //
            // //await file.WriteAsync(bytes, 0, bytes.Length);
            // stop.Stop();
            //
            // Console.WriteLine($"Download Completed, spend {stop.ElapsedMilliseconds / 1000:N2}s");
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient client, string requestUri,
            IProgress<HttpDownloadProgress> progress, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            using (var responseMessage = await client
                .GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false))
            {
                responseMessage.EnsureSuccessStatusCode();

                var content = responseMessage.Content;
                if (content == null) return Array.Empty<byte>();

                var headers = content.Headers;
                var contentLength = headers.ContentLength;
                
                await using (var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    var bytes = new List<byte>();

                    var downloadProgress = new HttpDownloadProgress();
                    
                    if (contentLength.HasValue) downloadProgress.TotalBytesToReceive = (ulong) contentLength.Value;
                    progress?.Report(downloadProgress);

                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, BufferSize, cancellationToken)
                        .ConfigureAwait(false)) > 0)
                    {
                        bytes.AddRange(buffer.Take(bytesRead));

                        downloadProgress.BytesReceived += (ulong) bytesRead;
                        progress?.Report(downloadProgress);
                    }

                    return bytes.ToArray();
                }
            }
        }
    }
}