using System;
using System.IO;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "File.mp4";
            Download(fileName);
            Upload(fileName);
        }

        static void Download(string id)
        {
            using (var fileStream = File.Open(id, FileMode.Append))
            using (var client = new WebClient())
            using (var serverStream = client.OpenRead($"http://localhost:54680/api/v1/Files/{id}?seek={fileStream.Length}"))
            {
                Copy(serverStream, fileStream);
            }
        }

        static void Upload(string id)
        {
            using (var fileStream = File.Open(id, FileMode.Open))
            using (var client = new WebClient())
            using (var serverStream = client.OpenWrite($"http://localhost:54680/api/v1/Files/{id}?append=false", "PUT"))
            {
                var size = long.Parse(client.DownloadString("http://localhost:54680/api/v1/Files/{id}/Size"));
                fileStream.Seek(size, SeekOrigin.Begin);
                Copy(fileStream, serverStream);
            }
        }

        static long Copy(Stream from, Stream to)
        {
            long copiedByteCount = 0;
            byte[] buffer = new byte[4096];
            var len = 0;
            while (from.CanRead && (len = from.Read(buffer, 0, buffer.Length)) > 0)
            {
                to.Write(buffer, 0, len);
                to.Flush();
                copiedByteCount += len;
            }
            return copiedByteCount;
        }
    }
}
