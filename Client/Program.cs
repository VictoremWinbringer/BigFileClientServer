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

            var buffer = new byte[4096];

            using (var file = File.Open(fileName, FileMode.Append))
            using (var client = new WebClient())
            using (var stream = client.OpenRead($"http://localhost:54680/api/v1/Files/{Guid.NewGuid()}?seek={file.Length}"))
            {
                foreach (string responseHeadersAllKey in client.ResponseHeaders.AllKeys)
                {
                  Console.WriteLine($"{responseHeadersAllKey} : {client.ResponseHeaders[responseHeadersAllKey]}"); 
                }

                while (stream.CanRead)
                {
                    var countRead = stream.Read(buffer, 0, buffer.Length);

                    if (countRead < 1)
                        break;

                    file.Write(buffer, 0, countRead);

                    file.Flush();
                }
            }

            Console.ReadLine();
        }
    }
}
