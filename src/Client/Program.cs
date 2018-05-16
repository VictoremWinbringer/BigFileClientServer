using System;
using System.IO;
using System.Net;

namespace Client
{
    public class Client
    {
        public static void DownloadSafe(string id)
        {
            while (true)
            {
                try
                {
                    Download(id);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static void UploadSafe(string id)
        {
            var append = false;
            while (true)
            {
                try
                {
                    Upload(id, append);
                    break;
                }
                catch (Exception e)
                {
                    append = true;
                    Console.WriteLine(e);
                }
            }
        }

        public static void Download(string id)
        {
            using (var fileStream = File.Open(id, FileMode.Append))
            using (var client = new WebClient())
            using (var serverStream = client.OpenRead($"http://localhost:54680/api/v1/Files/{id}?seek={fileStream.Length}"))
            {
                Copy(serverStream, fileStream);
                fileStream.Close();
            }
        }

        public static void Upload(string id, bool append)
        {
            using (var fileStream = File.Open(id, FileMode.Open))
            using (var client = new WebClient())
            using (var serverStream = client.OpenWrite($"http://localhost:54680/api/v1/Files/{id}?append={append}", "PUT"))
            {
                var size = long.Parse(client.DownloadString("http://localhost:54680/api/v1/Files/{id}/Size"));
                if (size >= fileStream.Length)
                    return;
                client.Headers.Add("Content-Type", "application/octet-stream");
                client.Headers.Add("Content-Disposition", "attachment; filename=" + id);
                client.Headers.Add("Content-Length", (fileStream.Length - size).ToString());
                fileStream.Seek(size, SeekOrigin.Begin);
                Copy(fileStream, serverStream);
                fileStream.Close();
            }
        }

        static long Copy(Stream from, Stream to)
        {
            long copiedByteCount = 0;
            byte[] buffer = new byte[4096];
            var len = 0;
            while (from.CanRead && to.CanWrite && (len = from.Read(buffer, 0, buffer.Length)) > 0)
            {
                to.Write(buffer, 0, len);
                to.Flush();
                copiedByteCount += len;
            }
            return copiedByteCount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "File.mp4";
            Client.DownloadSafe(fileName);
            Client.UploadSafe(fileName);
        }
    }
}
